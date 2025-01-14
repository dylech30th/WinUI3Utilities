using Microsoft.UI.Xaml;

namespace WinUI3Utilities;

/// <summary>
/// A set of method for <see cref="FrameworkElement"/>
/// </summary>
public static class FrameworkElementHelper
{
    /// <summary>
    /// Get <see cref="FrameworkElement.DataContext"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetDataContext<T>(this FrameworkElement frameworkElement)
        => (T)frameworkElement.DataContext;

    /// <summary>
    /// Get <see cref="FrameworkElement.DataContext"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetDataContext<T>(this object frameworkElement)
        => ((FrameworkElement)frameworkElement).GetDataContext<T>();

    /// <summary>
    /// Get <see cref="FrameworkElement.Tag"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetTag<T>(this FrameworkElement frameworkElement)
        => (T)frameworkElement.Tag;

    /// <summary>
    /// Get <see cref="FrameworkElement.Tag"/> and cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="frameworkElement"></param>
    /// <returns></returns>
    public static T GetTag<T>(this object frameworkElement)
        => (T)((FrameworkElement)frameworkElement).Tag;
}
