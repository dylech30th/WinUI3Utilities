using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using static WinUI3Utilities.SourceGenerator.Utilities;

namespace WinUI3Utilities.SourceGenerator;

[Generator]
public class LocalizedStringResourcesGenerator : IIncrementalGenerator
{
    private const string AttributeName = "WinUI3Utilities.Attributes.LocalizedStringResourcesAttribute";
    private const string SourceItemGroupMetadata = "build_metadata.AdditionalFiles.SourceItemGroup";
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var attributes = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeName,
                (_, _) => true,
                (syntaxContext, _) => syntaxContext)
            .Select((t, _) => t.Attributes[0])
            .Combine(context.AdditionalTextsProvider
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Where(t => t.Right.GetOptions(t.Left).TryGetValue(SourceItemGroupMetadata, out var sourceItemGroup)
                            && sourceItemGroup is "PRIResource"
                            && t.Left.Path.EndsWith(".resw"))
                .Select((tuple, _) => tuple.Left).Collect());

        context.RegisterSourceOutput(attributes, (spc, source) =>
        {
            if (Execute(source.Left, source.Right) is { } s)
                spc.AddSource($"WinUI3Utilities.Generator.LocalizedStringResources.g.cs", s);
        });
    }

    public string? Execute(AttributeData attribute, ImmutableArray<AdditionalText> additionalTexts)
    {
        if (attribute.ConstructorArguments.Length < 1 || attribute.ConstructorArguments[0].Value is not string specifiedNamespace)
            return null;

        var source = new StringBuilder($@"#nullable enable

using Microsoft.Windows.ApplicationModel.Resources;

namespace {specifiedNamespace};

");
        foreach (var additionalText in additionalTexts)
        {
            var fileName = Path.GetFileNameWithoutExtension(additionalText.Path)!;

            var doc = XDocument.Parse(additionalText.GetText()?.ToString()!);

            _ = source.AppendLine($@"
public static class {fileName}Resources
{{
    private static readonly ResourceLoader _resourceLoader = new(ResourceLoader.GetDefaultResourceFilePath(), ""{Path.GetFileNameWithoutExtension(additionalText.Path)}"");
");
            if (doc.XPathSelectElements("//data") is { } elements)
                foreach (var node in elements)
                {
                    var name = node.Attribute("name")!.Value;
                    if (name.Contains("["))
                        continue;

                    _ = source.AppendLine(@$"{Spacing(1)}public static readonly string {new Regex(@"\.|\:|\[|\]").Replace(name, "")} = _resourceLoader.GetString(""{name.Replace('.', '/')}"");");
                }

            _ = source.AppendLine("}").AppendLine();
        }
        return source.ToString();
    }
}
