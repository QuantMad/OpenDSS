using OpenDSS.Wrapper.Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace OpenDSS.Client.Controls
{
    public class Display : WindowsFormsHost
    {

        private readonly System.Windows.Forms.PictureBox display;

        [Description("Отображаемый канал дисплея"), Category("Common Properties")]
        public Channel Channel
        {
            get => (Channel)GetValue(ChannelProperty);
            set => SetValue(ChannelProperty, value);
        }

        public static readonly DependencyProperty ChannelProperty =
        DependencyProperty.Register("Channel", typeof(Channel), typeof(Display), new
           PropertyMetadata(Channel.Empty, new PropertyChangedCallback(OnChannelChanged)));

        private static void OnChannelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Display display = d as Display;
            display.OnChannelChanged(e);
        }

        private void OnChannelChanged(DependencyPropertyChangedEventArgs e)
        {
            Channel oldChannel = e.OldValue as Channel;
            Debug.WriteLine($"old: {oldChannel.RealPlayID}");
            if (oldChannel != Channel.Empty)
                oldChannel.StopRealPlay.Execute(null);
            display.Refresh();
            Channel.StartRealPlay.Execute(display.Handle);
        }

        public void Destroy()
        {
            Channel.StopRealPlay.Execute(null);
            display.Refresh();
            display.Dispose();
        }

        public Display(int row = 0, int col = 0) : base()
        {
            display = new System.Windows.Forms.PictureBox();
            Child = display;
            Background = Brushes.Green;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Margin = new Thickness(1.5, 1.5, 1.5, 1.5);
            SetValue(Grid.RowProperty, row);
            SetValue(Grid.ColumnProperty, col);

        }
    }
}
