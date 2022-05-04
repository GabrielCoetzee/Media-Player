using System;
using System.Windows;
using System.Windows.Controls;

namespace MediaPlayer.View.Extensions
{
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
