using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace OpenDSS.IndependentControls
{
    /// <summary>
    /// Логика взаимодействия для DateTimePicker.xaml
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        public delegate void DateTimePickerEventHandler(DateTimePicker s, DateTime val);

        public event DateTimePickerEventHandler OnValueChanged;

        [Description("Видимость заголовка"), Category("Common Properties")]
        public bool HeaderVisibility
        {
            get => (bool)GetValue(HeaderVisibilityProperty);
            set => SetValue(HeaderVisibilityProperty, value);
        }

        [Description("Текст заголовка"), Category("Common Properties")]
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public CalendarBlackoutDatesCollection BlackoutDates => calendar.BlackoutDates;
        public DateTime Value
        {
            get
            {
                return (DateTime)GetValue(ValueProperty);
            }
            private set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register(nameof(HeaderVisibility), typeof(bool), typeof(DateTimePicker),
            new PropertyMetadata(true));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(DateTimePicker),
            new PropertyMetadata("Select a date"));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(DateTime), typeof(DateTimePicker),
            new PropertyMetadata(DateTime.Now));

        public DateTimePicker()
        {
            InitializeComponent();
            calendar.SelectedDate = DateTime.Today;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Value = GenerateDateTime();
            OnValueChanged?.Invoke(this, Value);
        }

        private DateTime GenerateDateTime()
        {
            DateTime date = calendar.SelectedDate.Value.Date;
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            int hour = timePicker.Hour;
            int minute = timePicker.Minute;
            int second = timePicker.Second;

            return new DateTime(year, month, day, hour, minute, second);
        }

        private void calendar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }
    }
}
