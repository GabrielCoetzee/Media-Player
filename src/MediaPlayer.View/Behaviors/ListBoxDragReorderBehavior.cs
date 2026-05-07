using MediaPlayer.Model.BusinessEntities.Abstract;
using MediaPlayer.Model.Collections;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MediaPlayer.View.Behaviors
{
    public static class ListBoxDragReorderBehavior
    {
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached(
                "Enabled",
                typeof(bool),
                typeof(ListBoxDragReorderBehavior),
                new PropertyMetadata(false, OnEnabledChanged));

        public static bool GetEnabled(DependencyObject d) => (bool)d.GetValue(EnabledProperty);
        public static void SetEnabled(DependencyObject d, bool value) => d.SetValue(EnabledProperty, value);

        private static Point _dragStart;
        private static MediaItem _draggedItem;

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox lb)
                return;

            if ((bool)e.NewValue)
            {
                lb.AllowDrop = true;
                lb.PreviewMouseLeftButtonDown += OnPreviewMouseDown;
                lb.MouseMove += OnMouseMove;
                lb.Drop += OnDrop;
            }
            else
            {
                lb.PreviewMouseLeftButtonDown -= OnPreviewMouseDown;
                lb.MouseMove -= OnMouseMove;
                lb.Drop -= OnDrop;
            }
        }

        private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBox lb)
                return;

            _dragStart = e.GetPosition(null);
            _draggedItem = GetItemAt(lb, e.GetPosition(lb));
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _draggedItem == null)
                return;

            var pos = e.GetPosition(null);

            if (Math.Abs(pos.X - _dragStart.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(pos.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            if (sender is not ListBox lb)
                return;

            DragDrop.DoDragDrop(lb, _draggedItem, DragDropEffects.Move);
            _draggedItem = null;
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            if (sender is not ListBox lb)
                return;

            if (lb.ItemsSource is not MediaItemObservableCollection collection)
                return;

            if (e.Data.GetData(typeof(MediaItem)) is not MediaItem source &&
                e.Data.GetData(typeof(Model.BusinessEntities.Concrete.AudioItem)) is not MediaItem &&
                e.Data.GetData(typeof(Model.BusinessEntities.Concrete.VideoItem)) is not MediaItem)
                return;

            var dragged = ResolveDragged(e);
            if (dragged == null)
                return;

            var target = GetItemAt(lb, e.GetPosition(lb));
            var oldIndex = collection.IndexOf(dragged);
            var newIndex = target != null ? collection.IndexOf(target) : collection.Count - 1;

            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
                return;

            collection.Move(oldIndex, newIndex);

            for (var i = 0; i < collection.Count; i++)
                collection[i].Id = i;
        }

        private static MediaItem ResolveDragged(DragEventArgs e)
        {
            foreach (var format in e.Data.GetFormats())
            {
                var data = e.Data.GetData(format);
                if (data is MediaItem item)
                    return item;
            }

            return null;
        }

        private static MediaItem GetItemAt(ListBox lb, Point position)
        {
            var hit = lb.InputHitTest(position);
            if (hit is not DependencyObject d)
                return null;

            var container = FindAncestor<ListBoxItem>(d);
            return container?.DataContext as MediaItem;
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null && current is not T)
                current = VisualTreeHelper.GetParent(current);

            return current as T;
        }
    }
}
