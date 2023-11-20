using OpenDSS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static OpenDSS.Wrapper.Model.Channel;

namespace OpenDSS.IndependentControls
{
    public partial class IntervalSelector : UserControl
    {
        public static readonly int SECONDS_IN_DAY = 86400;

        #region ScaleProperty
        public event PropertyChangedCallback ScaleChanged;
        public event PropertyChangedCallback MaxScaleChanged;
        public event PropertyChangedCallback MinScaleChanged;

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(IntervalSelector), new PropertyMetadata(0.25d,
                (d, e) =>
                {
                    IntervalSelector sender = (IntervalSelector)d;
                    sender.ScaleChanged?.Invoke(sender, e);
                },
                (d, v) =>
                {
                    IntervalSelector sender = (IntervalSelector)d;
                    double val = (double)v;
                    double result = val > sender.MaxScale ? sender.MaxScale : val < sender.MinScale ? sender.MinScale : val;

                    return result;
                }));

        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(double), typeof(IntervalSelector), new PropertyMetadata(1d,
                (d, e) =>
                {
                    IntervalSelector sender = (IntervalSelector)d;
                    sender.MaxScaleChanged?.Invoke(sender, e);
                }));

        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register("MinScale", typeof(double), typeof(IntervalSelector), new PropertyMetadata(.001d,
                (d, e) =>
                {
                    IntervalSelector sender = (IntervalSelector)d;
                    sender.MinScaleChanged?.Invoke(sender, e);
                }));

        #endregion ScaleProperty

        public int Position
        {
            get { return (int)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // TODO: Преобразование целочисленной позиции в позицию на экране
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(int), typeof(IntervalSelector), new PropertyMetadata(0));

        #region OnScreenPosition

        private event PropertyChangedCallback OnScreenPositionChanged;

        private double OnScreenPosition
        {
            get { return (double)GetValue(OnScreenPositionProperty); }
            set { SetValue(OnScreenPositionProperty, value); }
        }

        private static readonly DependencyProperty OnScreenPositionProperty =
            DependencyProperty.Register("OnScreenPosition", typeof(double), typeof(IntervalSelector), new PropertyMetadata(0d,
                (d, e) =>
                {
                    IntervalSelector sender = (IntervalSelector)d;
                    sender.OnScreenPositionChanged?.Invoke(sender, e);
                },
                (d, v) =>
                {
                    IntervalSelector sender = (IntervalSelector)d;
                    double value = (double)v;
                    double maxPosition = sender.ActualWidth - sender.ActualWidth * sender.Scale;
                    return value > maxPosition ? maxPosition : value < 0d ? 0d : value;
                }));
        #endregion OnScreenPosition

        public List<TimeInterval> AvailableIntervals
        {
            get { return (List<TimeInterval>)GetValue(AvailableIntervalsProperty); }
            set { SetValue(AvailableIntervalsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailableIntervalsProperty =
            DependencyProperty.Register("AvailableIntervals",
                typeof(List<TimeInterval>),
                typeof(IntervalSelector),
                new PropertyMetadata(new List<TimeInterval>()));
    }
}
