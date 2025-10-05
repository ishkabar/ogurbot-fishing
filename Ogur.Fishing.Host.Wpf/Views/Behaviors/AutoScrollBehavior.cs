// File: Ogur.Fishing.Host.Wpf/Behaviors/AutoScroll.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Behaviors

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Ogur.Fishing.Host.Wpf.Behaviors
{
    /// <summary>
    /// Provides an attached property to automatically scroll a ListBox to the last item when its items change.
    /// </summary>
    public static class AutoScroll
    {
        /// <summary>
        /// Identifies the IsEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(AutoScroll),
                new PropertyMetadata(false, OnIsEnabledChanged));

        /// <summary>
        /// Gets the value indicating whether auto scrolling is enabled for the specified ListBox.
        /// </summary>
        /// <param name="obj">Target dependency object.</param>
        /// <returns>True if enabled; otherwise false.</returns>
        public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);

        /// <summary>
        /// Sets the value indicating whether auto scrolling is enabled for the specified ListBox.
        /// </summary>
        /// <param name="obj">Target dependency object.</param>
        /// <param name="value">True to enable; otherwise false.</param>
        public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        /// <summary>
        /// Handles the IsEnabled property changes and wires or unwires required event handlers.
        /// </summary>
        /// <param name="d">Target dependency object.</param>
        /// <param name="e">Dependency property changed event args.</param>
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox) return;

            listBox.Loaded -= ListBoxOnLoaded;
            listBox.Unloaded -= ListBoxOnUnloaded;

            if (e.NewValue is true)
            {
                listBox.Loaded += ListBoxOnLoaded;
                listBox.Unloaded += ListBoxOnUnloaded;
                TryHookCollection(listBox);
            }
            else
            {
                UnhookCollection(listBox);
            }
        }

        /// <summary>
        /// Hooks collection change events when the ListBox is loaded.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private static void ListBoxOnLoaded(object sender, RoutedEventArgs e) => TryHookCollection((ListBox)sender);

        /// <summary>
        /// Unhooks collection change events when the ListBox is unloaded.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private static void ListBoxOnUnloaded(object sender, RoutedEventArgs e) => UnhookCollection((ListBox)sender);

        /// <summary>
        /// Subscribes to the current items collection changes of the ListBox.
        /// </summary>
        /// <param name="listBox">Target ListBox.</param>
        private static void TryHookCollection(ListBox listBox)
        {
            UnhookCollection(listBox);

            var source = (listBox.ItemsSource as INotifyCollectionChanged) ??
                         (listBox.Items as INotifyCollectionChanged);

            if (source is null) return;

            source.CollectionChanged += listBox_AutoScroll_CollectionChanged;
            listBox.SetValue(_subscriptionProperty, source);
        }

        /// <summary>
        /// Unsubscribes from previously hooked collection changes.
        /// </summary>
        /// <param name="listBox">Target ListBox.</param>
        private static void UnhookCollection(ListBox listBox)
        {
            if (listBox.GetValue(_subscriptionProperty) is INotifyCollectionChanged previous)
            {
                previous.CollectionChanged -= listBox_AutoScroll_CollectionChanged;
                listBox.ClearValue(_subscriptionProperty);
            }
        }

        /// <summary>
        /// Handles collection changes and scrolls to the last item when appropriate.
        /// </summary>
        /// <param name="sender">Collection sender.</param>
        /// <param name="e">Collection changed args.</param>
        private static void listBox_AutoScroll_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is null) return;

            if (_ownerFromSubscription(sender) is not ListBox listBox) return;

            if (listBox.Items.Count == 0) return;

            if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Reset or NotifyCollectionChangedAction.Replace or NotifyCollectionChangedAction.Move)
            {
                var last = listBox.Items[^1];
                listBox.ScrollIntoView(last);
            }
        }

        /// <summary>
        /// Retrieves the owner ListBox for the given subscription collection.
        /// </summary>
        /// <param name="subscription">Subscribed collection.</param>
        /// <returns>Owner ListBox if found; otherwise null.</returns>
        private static ListBox? _ownerFromSubscription(object subscription)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (FindOwnerInVisualTree(w) is ListBox lb && ReferenceEquals(lb.GetValue(_subscriptionProperty), subscription))
                    return lb;
            }
            return null;
        }

        /// <summary>
        /// Traverses the visual tree to find a ListBox with an active subscription.
        /// </summary>
        /// <param name="root">Root dependency object.</param>
        /// <returns>ListBox if found; otherwise null.</returns>
        private static ListBox? FindOwnerInVisualTree(DependencyObject root)
        {
            if (root is ListBox lb && lb.ReadLocalValue(_subscriptionProperty) is INotifyCollectionChanged)
                return lb;

            var count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < count; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(root, i);
                if (FindOwnerInVisualTree(child) is ListBox found)
                    return found;
            }
            return null;
        }

        /// <summary>
        /// Holds the active subscription for a ListBox instance.
        /// </summary>
        private static readonly DependencyProperty _subscriptionProperty =
            DependencyProperty.RegisterAttached("_subscription",
                typeof(INotifyCollectionChanged),
                typeof(AutoScroll),
                new PropertyMetadata(null));
    }
}
