using System;
using System.Windows;
using System.Windows.Controls;

namespace MediaPlayer.View.Extensions
{
    /// <summary>
    /// MediaElement Position cannot be bound to by default in XAML. This allows us a one-way binding to media element position so we can 
    /// change the position via an event when the user moves the seek bar or presses it to jump to a specific position.
    /// </summary>
    public class MediaElementExtension
    {
        private static readonly TimeSpan DefaultValue = new(0);

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position",
            typeof(TimeSpan), typeof(MediaElementExtension),
            new FrameworkPropertyMetadata(DefaultValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PositionPropertyChanged));

        private static void PositionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is MediaElement mediaElement)
                mediaElement.Position = (TimeSpan)e.NewValue;
        }

        public static void SetPosition(UIElement element, TimeSpan value)
        {
            element.SetValue(PositionProperty, value);
        }

        public static TimeSpan GetPosition(UIElement element)
        {
            return (TimeSpan)element.GetValue(PositionProperty);
        }
    }
}
