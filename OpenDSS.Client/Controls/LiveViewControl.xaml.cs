using OpenDSS.Wrapper.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OpenDSS.Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для LiveViewControl.xaml
    /// </summary>
    public partial class LiveViewControl : UserControl
    {
        [Description("Список доступных для воспроизведения устройств"), Category("Common Properties")]
        public List<Device> Devices
        {
            get => (List<Device>)GetValue(DevicesProperty);
            set => SetValue(DevicesProperty, value);
        }

        public static readonly DependencyProperty DevicesProperty =
        DependencyProperty.Register("Devices", typeof(List<Device>), typeof(LiveViewControl), new
           PropertyMetadata(null, new PropertyChangedCallback(OnDevicesChanged)));

        private static void OnDevicesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LiveViewControl realPlaySelector = d as LiveViewControl;
            realPlaySelector.OnDevicesChanged(e);
        }

        private void OnDevicesChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public LiveViewControl()
        {
            InitializeComponent();
        }

        private void StackPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                dscMain.SelectedDisplay.Channel = (Channel)tvDevices.SelectedItem;
            }
        }
    }
}
