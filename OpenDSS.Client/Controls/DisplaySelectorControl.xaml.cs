using OpenDSS.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenDSS.Client.Controls
{
    /// <summary>
    /// Набор пресетов просмотра
    /// </summary>
    public enum PreviewPresets
    {
        v1h1 = 1,
        v2h2 = 4,
        v3h3 = 9,
        v4h4 = 16,
        v5h5 = 25,
        v6h6 = 36
    }

    /// <summary>
    /// Логика взаимодействия для RealPlaySelector.xaml
    /// </summary>
    public partial class DisplaySelectorControl : UserControl
    {
        private readonly List<Display> displays = new List<Display>();

        #region selected_view
        private Display selectedDisplay = null;
        public Display SelectedDisplay
        {
            get => selectedDisplay;
            set
            {
                if (selectedDisplay != null) selectedDisplay.Background = Brushes.Green;
                selectedDisplay = value;
                if (selectedDisplay != null) selectedDisplay.Background = Brushes.Aquamarine;
            }
        }
        #endregion selected_view

        #region preset_selection_handling
        [Description("Кол-во отображаемых каналов"), Category("Common Properties")]
        public PreviewPresets Preset
        {
            get => (PreviewPresets)GetValue(PresetProperty);
            set => SetValue(PresetProperty, value);
        }

        public static readonly DependencyProperty PresetProperty =
        DependencyProperty.Register("Preset", typeof(PreviewPresets), typeof(DisplaySelectorControl), new
           PropertyMetadata(PreviewPresets.v2h2, new PropertyChangedCallback(OnPresetChanged)));

        private static void OnPresetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DisplaySelectorControl realPlaySelector = d as DisplaySelectorControl;
            realPlaySelector.OnPresetChanged(e);
        }

        private void OnPresetChanged(DependencyPropertyChangedEventArgs e)
        {
            int oldVal = (int)(PreviewPresets)e.OldValue;
            int newVal = (int)(PreviewPresets)e.NewValue;

            if (oldVal == newVal) return;

            RefreshRows();
            RefreshDisplays();
            tbDebug.Text = $"{displays.Count}";
        }
        #endregion preset_selection_handling

        public DisplaySelectorControl()
        {
            InitializeComponent();
            RefreshRows();
            RefreshDisplays();
            tbDebug.Text = $"{displays.Count}";
        }

        #region helpers
        private Display SpawnDisplay()
        {
            Display newDisplay = new Display();
            ((System.Windows.Forms.PictureBox)newDisplay.Child).Click += (s, e) => SelectedDisplay = newDisplay;

            return newDisplay;
        }

        private void RefreshRows()
        {
            previewGrid.RowDefinitions.Clear();
            previewGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < Math.Sqrt((int)Preset); i++)
            {
                previewGrid.RowDefinitions.Add(new RowDefinition());
                previewGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void RefreshDisplays()
        {
            // Удаляет лищние дисплеи в случае
            // если их кол-во больше чем предуспотрено пресетов
            while (displays.Count > (int)Preset)
            {
                Display display = displays[displays.Count - 1];
                display.Destroy();
                previewGrid.Children.Remove(display);
                displays.Remove(display);
            }

            // Добавляет недостающие дисплеи в случае
            // если их кол-во меньше чем предуспотрено пресетов
            while (displays.Count < (int)Preset)
            {
                Display display = SpawnDisplay();
                previewGrid.Children.Add(display);
                displays.Add(display);
            }

            // Обновляет позицию дисплеев на сетке
            int row = -1;
            for (int i = 0; i < displays.Count(); i++)
            {
                if (i % previewGrid.ColumnDefinitions.Count == 0)
                    row++;
                int col = i % previewGrid.ColumnDefinitions.Count;

                displays[i].SetValue(Grid.RowProperty, row);
                displays[i].SetValue(Grid.ColumnProperty, col);
            }

            SelectedDisplay = displays[0];
        }
    }
    #endregion helpers
}
