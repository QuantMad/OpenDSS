using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OpenDSS.IndependentControls
{
    /// <summary>
    /// Логика взаимодействия для TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public delegate void TimePickerEventHandler(TimePicker s, int oldVal, int newVal);

        #region events
        public event TimePickerEventHandler OnHourChanged;
        public event TimePickerEventHandler OnMinuteChanged;
        public event TimePickerEventHandler OnSecondChanged;
        #endregion events

        #region public_property
        [Description("Час"), Category("Common Properties")]
        public int Hour
        {
            get => (int)GetValue(HourProperty);
            set => SetValue(HourProperty, value);
        }

        [Description("Минута"), Category("Common Properties")]
        public int Minute
        {
            get => (int)GetValue(MinuteProperty);
            set => SetValue(MinuteProperty, value);
        }

        [Description("Секунда"), Category("Common Properties")]
        public int Second
        {
            get => (int)GetValue(SecondProperty);
            set => SetValue(SecondProperty, value);
        }
        #endregion public_property

        #region dependency_property
        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register(nameof(Hour), typeof(int), typeof(TimePicker),
            new PropertyMetadata(DateTime.Now.Hour, HourPropertyChanged, Ceorce24));

        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register(nameof(Minute), typeof(int), typeof(TimePicker),
            new PropertyMetadata(DateTime.Now.Minute, MinutePropertyChanged, Ceorce60));

        public static readonly DependencyProperty SecondProperty =
            DependencyProperty.Register(nameof(Second), typeof(int), typeof(TimePicker),
            new PropertyMetadata(DateTime.Now.Second, SecondPropertyChanged, Ceorce60));

        #endregion dependency_property

        #region dependency_property_changed
        private static void HourPropertyChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            TimePicker sender = (s as TimePicker);
            sender.OnHourChanged?.Invoke(sender, (int)e.OldValue, (int)e.NewValue);
        }

        private static void MinutePropertyChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            TimePicker sender = (s as TimePicker);
            sender.OnMinuteChanged?.Invoke(sender, (int)e.OldValue, (int)e.NewValue);
        }

        private static void SecondPropertyChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            TimePicker sender = (s as TimePicker);
            sender.OnSecondChanged?.Invoke(sender, (int)e.OldValue, (int)e.NewValue);
        }

        #endregion dependency_property_changed

        #region ceorce
        public static object Ceorce24(DependencyObject _, object s)
        {
            int val = (int)s;
            if (val >= 24) val = 23;
            if (val < 0) val = 0;
            return val;
        }

        public static object Ceorce60(DependencyObject _, object s)
        {
            int val = (int)s;
            if (val >= 60) val = 59;
            if (val < 0) val = 0;
            return val;
        }
        #endregion ceorce

        public TimePicker()
        {
            InitializeComponent();
            nudMinutes.OnLoopTriggered += (s, ov, nv) =>
            {
                if (nv == 0)
                {
                    Hour++;
                }
                else if (nv == 59)
                {
                    Hour--;
                }
            };

            nudSeconds.OnLoopTriggered += (s, ov, nv) =>
            {
                if (nv < ov)
                {
                    if (Minute == 59)
                    {
                        Hour++;
                        Minute = 0;
                        return;
                    }
                    Minute++;
                }
                else if (nv > ov)
                {
                    if (Minute == 0)
                    {
                        Hour--;
                        Minute = 59;
                        return;
                    }
                    Minute--;
                }
            };
        }
    }
}
