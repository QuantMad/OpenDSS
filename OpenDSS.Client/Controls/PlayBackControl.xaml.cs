using OpenDSS.Common;
using OpenDSS.Wrapper.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OpenDSS.Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для PlayBackControl.xaml
    /// </summary>
    public partial class PlayBackControl : UserControl
    {
        public PlayBackControl()
        {
            InitializeComponent();
        }

        public List<TimeInterval> Intervals;

        [Description("Список доступных для воспроизведения устройств"), Category("Common Properties")]
        public List<Device> Devices
        {
            get => (List<Device>)GetValue(DevicesProperty);
            set => SetValue(DevicesProperty, value);
        }

        public static readonly DependencyProperty DevicesProperty =
        DependencyProperty.Register("Devices", typeof(List<Device>), typeof(PlayBackControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnDevicesChanged)));

        private static void OnDevicesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlayBackControl playBackControl = d as PlayBackControl;
            playBackControl.OnDevicesChanged(e);
        }

        private void OnDevicesChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        private void TextBlock_MouseLeftButtonUp_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var ch = (tvDevices.SelectedItem as Channel);

            var intervals = ch.GetIntervals();

            if (intervals != null)
            {
                intervalsSelector.AvailableIntervals = intervals;
            }
        }
    }
}
