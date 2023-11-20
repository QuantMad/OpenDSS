using OpenDSS.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace OpenDSS.Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для SavedDevicesControl.xaml
    /// </summary>
    public partial class SavedDevicesControl : UserControl, INotifyPropertyChanged
    {
        [Description("Доступные устройства"), Category("Common Properties")]
        public List<Device> Devices
        {
            get => (List<Device>)GetValue(DevicesProperty);
            set => SetValue(DevicesProperty, value);
        }

        public static readonly DependencyProperty DevicesProperty =
        DependencyProperty.Register("Devices", typeof(List<Device>), typeof(SavedDevicesControl), new
           PropertyMetadata(null, new PropertyChangedCallback(OnDevicesChanged)));

        private static void OnDevicesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => (d as SavedDevicesControl).OnDevicesChanged(e);

        private int online = 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public int Total => Devices.Count;
        public int Online => online;

        private void OnDevicesChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var device in Devices)
            {
                device.PropertyChanged += (s, a) =>
                {
                    if (a.PropertyName == nameof(Device.ConnectionInfo.LoginID))
                    {
                        online += device.ConnectionInfo.LoginID != IntPtr.Zero ? 1 : -1;
                        OnPropertyChanged(nameof(Online));
                    }
                };
            }
        }

        public SavedDevicesControl()
        {
            InitializeComponent();
        }
    }
}
