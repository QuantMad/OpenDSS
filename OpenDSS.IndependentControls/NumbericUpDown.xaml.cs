using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenDSS.IndependentControls
{
    /// <summary>
    /// Логика взаимодействия для NumbericUpDown.xaml
    /// </summary>
    public partial class NumbericUpDown : UserControl
    {
        public enum LoopType
        {
            None,
            Always,
            OnAlterIncrement
        }

        private static readonly Regex numbersRegex = new Regex("[^0-9.-]+");

        public delegate void NumbericUpDownEventHandler(NumbericUpDown s, int oldVal, int newVal);

        public event NumbericUpDownEventHandler OnValueChanged;
        public event NumbericUpDownEventHandler OnMaxValueChanged;
        public event NumbericUpDownEventHandler OnMinValueChanged;
        public event NumbericUpDownEventHandler OnLoopTriggered;

        private bool isAlterIncrementAllowed
            => AlterIncrementEnabled && Keyboard.IsKeyDown(AlterIncrementKey);

        private int instantIncrement =>
            !isAlterIncrementAllowed ? Increment : AlterIncrement;

        private bool isLooped =>
            Loop == LoopType.Always || (isAlterIncrementAllowed && Loop == LoopType.OnAlterIncrement);

        #region public_properties
        [Description("Значение"), Category("Common Properties")]
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        [Description("Максимальное значение"), Category("Common Properties")]
        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        [Description("Минимальное значение"), Category("Common Properties")]
        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        [Description("Инкремент"), Category("Common Properties")]
        public int Increment
        {
            get => (int)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        [Description("Альтернативный инкремент "), Category("Common Properties")]
        public bool AlterIncrementEnabled
        {
            get => (bool)GetValue(AlterIncrementEnabledProperty);
            set => SetValue(AlterIncrementEnabledProperty, value);
        }

        [Description("Альтернативный инкремент"), Category("Common Properties")]
        public int AlterIncrement
        {
            get => (int)GetValue(AlterIncrementProperty);
            set => SetValue(AlterIncrementProperty, value);
        }

        [Description("Клавиша активации альтернативного инкремента"), Category("Common Properties")]
        public Key AlterIncrementKey
        {
            get => (Key)GetValue(AlterIncrementKeyProperty);
            set => SetValue(AlterIncrementKeyProperty, value);
        }

        [Description("Показывать ли кнопки инкремента"), Category("Common Properties")]
        public bool IncrementorsVisibility
        {
            get => (bool)GetValue(IncrementorsVisibilityProperty);
            set => SetValue(IncrementorsVisibilityProperty, value);
        }

        [Description("Тип зацикливания значения"), Category("Common Properties")]
        public LoopType Loop
        {
            get => (LoopType)GetValue(LoopProperty);
            set => SetValue(LoopProperty, value);
        }
        #endregion public_properties

        #region dependency_property
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumbericUpDown),
            new PropertyMetadata(0, ValuePropertyChanged, Ceorce));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(NumbericUpDown),
            new PropertyMetadata(int.MaxValue, MaxValuePropertyChanged));

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(NumbericUpDown),
            new PropertyMetadata(int.MinValue, MinValuePropertyChanged));

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(nameof(Increment), typeof(int), typeof(NumbericUpDown),
            new PropertyMetadata(1));

        public static readonly DependencyProperty AlterIncrementEnabledProperty =
            DependencyProperty.Register(nameof(AlterIncrementEnabled), typeof(bool), typeof(NumbericUpDown),
            new PropertyMetadata(true));

        public static readonly DependencyProperty AlterIncrementProperty =
            DependencyProperty.Register(nameof(AlterIncrement), typeof(int), typeof(NumbericUpDown),
            new PropertyMetadata(5));

        public static readonly DependencyProperty AlterIncrementKeyProperty =
            DependencyProperty.Register(nameof(AlterIncrementKey), typeof(Key), typeof(NumbericUpDown),
            new PropertyMetadata(Key.LeftShift));

        public static readonly DependencyProperty IncrementorsVisibilityProperty =
            DependencyProperty.Register(nameof(IncrementorsVisibility), typeof(bool), typeof(NumbericUpDown),
            new PropertyMetadata(true));

        public static readonly DependencyProperty LoopProperty =
            DependencyProperty.Register(nameof(Loop), typeof(LoopType), typeof(NumbericUpDown),
            new PropertyMetadata(LoopType.OnAlterIncrement));
        #endregion dependency_property

        #region static_handlers
        private static void ValuePropertyChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            NumbericUpDown sender = (s as NumbericUpDown);
            sender.OnValueChanged?.Invoke(sender, (int)e.OldValue, (int)e.NewValue);
        }

        private static void MaxValuePropertyChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            NumbericUpDown sender = (s as NumbericUpDown);
            sender.OnMaxValueChanged?.Invoke(sender, (int)e.OldValue, (int)e.NewValue);
        }

        private static void MinValuePropertyChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            NumbericUpDown sender = (s as NumbericUpDown);
            sender.OnMinValueChanged?.Invoke(sender, (int)e.OldValue, (int)e.NewValue);
        }

        #endregion static_handlers

        public static object Ceorce(DependencyObject s, object v)
        {
            NumbericUpDown sender = s as NumbericUpDown;
            int val = (int)v;

            return val > sender.MaxValue ? sender.MaxValue : val < sender.MinValue ? sender.MinValue : val; ;
        }

        public NumbericUpDown() => InitializeComponent();

        #region input_filters
        private void NumbersInputFilter(object sender, TextCompositionEventArgs e)
        {
            e.Handled = numbersRegex.IsMatch(e.Text);
        }
        // едят ли кошки мошек, едят ли кошки мошек, едят ли мошки кошек 
        // FIXME: Не работает валидация вставки из буфера обмена
        private void NumbersPastingFilter(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(typeof(string));
            if (isText)
            {
                string text = (string)e.DataObject.GetData(typeof(string));

                if (!numbersRegex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        #endregion input_filters

        #region interactivity
        private void IncrementIf(bool incrementСondition, bool decrementСondition = false)
        {
            int increment = incrementСondition ? instantIncrement : decrementСondition ? -instantIncrement : 0;
            int oldVal = Value;
            if (isLooped && Value == MaxValue && increment > 0)
            {
                Value = MinValue;
                OnLoopTriggered?.Invoke(this, oldVal, Value);
                return;
            }
            else if (isLooped && Value == MinValue && increment < 0)
            {
                Value = MaxValue;
                OnLoopTriggered?.Invoke(this, oldVal, Value);
                return;
            }
            Value += increment;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) =>
            IncrementIf(e.Key == Key.Up, e.Key == Key.Down);

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e) =>
            IncrementIf(e.Delta > 0, e.Delta < 0);

        private void IncButton_Click(object sender, RoutedEventArgs e) =>
            IncrementIf(true, false);

        private void DecButton_Click(object sender, RoutedEventArgs e) =>
            IncrementIf(false, true);
        #endregion interactivity
    }
}
