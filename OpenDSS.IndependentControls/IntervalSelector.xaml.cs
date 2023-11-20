using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenDSS.IndependentControls
{
    public partial class IntervalSelector : UserControl
    {
        private Brush backgroundBrush = Brushes.Azure.Clone();
        private Brush intervalBrush = Brushes.Blue.Clone();
        private Brush previewBrush = Brushes.Gray.Clone();
        private Pen mainPen = new Pen(Brushes.Black, 1.0);
        private DrawingGroup dynamicStore = new DrawingGroup();
        private DrawingGroup staticStore = new DrawingGroup();
        private readonly Typeface tf = new Typeface("Lucida Console");

        private readonly (double scale, (int grads, int subgrads) correspondence)[] graduations =  {
            (0.0051d, (1440, 1)),
            (0.059d, (288, 4)),
            (0.120d, (144, 9)),
            (0.198d, (96, 2)),
            (0.302d, (48, 2)),
            (0.594d, (24, 3)),
            (0d, (12, 3))
        };

        private void calcGraduations()
        {
            if (Scale > graduations[graduations.Length - 2].scale)
            {
                graduationsCount = graduations[graduations.Length - 1].correspondence.grads;
                subGraduationsCount = graduations[graduations.Length - 1].correspondence.subgrads;
                timestampsRatio = 1440 / graduationsCount;
                return;
            }

            for (int i = 0; i < graduations.Length - 2; i++)
            {
                if (Scale < graduations[i].scale)
                {
                    graduationsCount = graduations[i].correspondence.grads;
                    subGraduationsCount = graduations[i].correspondence.subgrads;
                    timestampsRatio = 1440 / graduationsCount;
                    return;
                }
            }
        }

        private int graduationsCount = 24;
        private int subGraduationsCount = 5;

        double scalingWidth => ActualWidth * Scale;
        double graduationStep => ActualWidth / (graduationsCount);
        int passedGraduations => (int)(OnScreenPosition / graduationStep) + Convert.ToInt32(OnScreenPosition > 0);
        double scaledGraduationStep => graduationStep / Scale;
        int visibleGraduations => (int)((OnScreenPosition + scalingWidth) / graduationStep - passedGraduations);
        double offset => (OnScreenPosition / Scale) % scaledGraduationStep;
        double OnScreenEnd => OnScreenPosition + scalingWidth;
        double timestampHalfWidth, timestampHalfHeight;
        double unit;
        private FormattedText[] timestamps;
        int timestampsRatio;
        bool RenderRequired = true;

        public IntervalSelector()
        {
            backgroundBrush.Freeze();
            intervalBrush.Freeze();
            previewBrush.Opacity = 0.7;
            previewBrush.Freeze();
            mainPen.Freeze();

            CalculateTimestamps();
            calcGraduations();
            InitializeComponent();

            #region event_subscriptions
            SizeChanged += (s, e) =>
            {
                unit = ActualHeight / 5d;
                if (OnScreenPosition != 0)
                {
                    OnScreenPosition = e.NewSize.Width / (e.PreviousSize.Width / OnScreenPosition);
                    return;
                }
                RenderRequired = true;
            };

            ScaleChanged += (s, e) =>
            {
                calcGraduations();
                if (OnScreenPosition + scalingWidth > ActualWidth)
                {
                    OnScreenPosition = ActualWidth - scalingWidth;
                    return;
                }
                RenderRequired = true;
            };

            OnScreenPositionChanged += (s, e) =>
            {
                if (e.OldValue != e.NewValue) RenderRequired = true;
            };

            MouseWheel += (s, e) =>
            {
                Scale += (e.Delta > 0d ? -(0.02d * Scale) : e.Delta < 0 ? 0.02d * Scale : 0);
            };

            double _oldX = -1;

            PreviewMouseMove += (s, e) =>
            {
                double newX = e.MouseDevice.GetPosition(s as IInputElement).X;

                if (_oldX == -1)
                {
                    _oldX = newX;
                    return;
                }

                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                    OnScreenPosition = OnScreenPosition + (_oldX - newX) * Scale;

                _oldX = newX;
            };

            CompositionTarget.Rendering += (s, e) => { if (RenderRequired) RenderDynamic(); };
            #endregion event_subscriptions
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawDrawing(dynamicStore);
        }

        public void RenderDynamic()
        {
            var dc = dynamicStore.Open();
            RenderDynamic(dc);
            dc.Close();
            RenderRequired = false;
        }

        private void RenderDynamic(DrawingContext dc)
        {
            double ay = 0, ah = unit;
            double by = ah, bh = unit;
            double cy = by + bh, ch = unit;
            double dy = cy + ch, dh = unit;

            Rect arect = new Rect(0, ay, ActualWidth, ah);
            Rect brect = new Rect(0, by, ActualWidth, bh);
            Rect crect = new Rect(0, cy, ActualWidth, ch);
            Rect drect = new Rect(0, dy, ActualWidth, dh);

            dc.DrawRectangle(backgroundBrush, mainPen, arect);
            dc.DrawRectangle(backgroundBrush, mainPen, brect);
            dc.DrawRectangle(backgroundBrush, mainPen, crect);
            dc.DrawRectangle(backgroundBrush, mainPen, drect);

            double subgraduationWidth = scaledGraduationStep / (subGraduationsCount + 1);

            for (int i = 0; i <= visibleGraduations; i++)
            {
                bool IsShiftNeeded = (offset > 0 || (OnScreenPosition >= (ActualWidth - ActualWidth * Scale) && OnScreenPosition != 0));
                double x = i * scaledGraduationStep + (IsShiftNeeded ? scaledGraduationStep : 0) - offset;
                dc.DrawLine(mainPen, new Point(x, by), new Point(x, by + bh));

                #region subgraduations
                for (int j = 0; i == 0 && j < subGraduationsCount; j++)
                {
                    double x2 = x - subgraduationWidth - j * subgraduationWidth;
                    if (x2 < 0) break;
                    dc.DrawLine(mainPen, new Point(x2, by + bh / 2), new Point(x2, by + bh));
                }

                for (int j = 0; j < subGraduationsCount; j++)
                {
                    double x2 = x + subgraduationWidth + j * subgraduationWidth;
                    if (x2 > ActualWidth) break;
                    dc.DrawLine(mainPen, new Point(x2, by + bh / 2), new Point(x2, by + bh));
                }
                #endregion subgraduations

                #region availableIntervals
                foreach (var interval in AvailableIntervals)
                {
                    double screenStart = ActualWidth / (SECONDS_IN_DAY / interval.Start.TimeOfDay.TotalSeconds);
                    double screenEnd = ActualWidth / (SECONDS_IN_DAY / interval.End.TimeOfDay.TotalSeconds);
                    double screenWidth = ActualWidth / (SECONDS_IN_DAY / interval.Length.TotalSeconds);

                    if (OnScreenPosition > screenEnd) continue;
                    if (OnScreenEnd < screenStart) continue;

                    double xStart = screenStart / Scale - OnScreenPosition / Scale;
                    double rectW = screenWidth / Scale;

                    if (OnScreenPosition > screenStart)
                    {
                        rectW = (screenWidth - (OnScreenPosition - screenStart)) / Scale;
                        xStart = 0;
                    }

                    if (OnScreenEnd < screenEnd)
                    {
                        rectW -= (screenEnd - OnScreenEnd) / Scale;
                    }

                    Rect r = new Rect(xStart, cy, rectW, ch);
                    dc.DrawRectangle(intervalBrush, mainPen, r);
                }
                #endregion availableIntervals

                dc.DrawText(timestamps[(passedGraduations + i) * timestampsRatio], new Point(x - timestampHalfWidth, ah / 2 - timestampHalfHeight));
            }

            #region preview
            foreach (var interval in AvailableIntervals)
            {
                double start = interval.Start.TimeOfDay.TotalSeconds;
                double width = interval.Length.TotalSeconds;
                double screenStart = ActualWidth / (SECONDS_IN_DAY / start);
                double screenWidth = ActualWidth / (SECONDS_IN_DAY / width);
                Rect r = new Rect(screenStart, dy + dh / 2, screenWidth, dh / 2);
                dc.DrawRectangle(intervalBrush, mainPen, r);
            }

            dc.DrawRectangle(previewBrush, mainPen, new Rect(OnScreenPosition, dy, scalingWidth, dh));

            for (int i = 0; i < 24; i++)
            {
                double x = i * (ActualWidth / 24);
                dc.DrawLine(mainPen, new Point(x, dy), new Point(x, dy + dh));
            }
            #endregion preview
        }

        #region helpers
        private (int, int, int) SecondToTime(int second)
        {
            int hour = second / 3600;
            int min = (second - hour * 3600) / 60;
            int sec = second - hour * 3600 - min * 60;

            return (hour, min, sec);
        }

        private readonly StringBuilder timestampBuilder = new StringBuilder(5, 8);
        private string TimeToString((int hour, int min, int sec) time)
        {
            timestampBuilder.Clear();
            if (time.hour < 10) timestampBuilder.Append('0');
            timestampBuilder.Append(time.hour);
            timestampBuilder.Append(':');
            if (time.min < 10) timestampBuilder.Append('0');
            timestampBuilder.Append(time.min);
            if (time.sec != -1)
            {
                timestampBuilder.Append(':');
                if (time.sec < 10) timestampBuilder.Append('0');
                timestampBuilder.Append(time.sec);
            }

            return timestampBuilder.ToString();
        }

        private void CalculateTimestamps()
        {
            timestamps = new FormattedText[1440 + 1];

            for (int i = 0; i < timestamps.Length; i++)
            {
                int second = (SECONDS_IN_DAY / 1440) * i;
                (int hour, int min, int sec) time = SecondToTime(second);
                time.sec = -1;
                string timeString = time.hour != 24 ? TimeToString(time) : "00:00";

                timestamps[i] = new FormattedText(
                    timeString,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    tf, 10, Brushes.Black,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);
            }

            timestampHalfWidth = timestamps[0].Width / 2;
            timestampHalfHeight = timestamps[0].Height / 2;
        }
        #endregion helpers
    }
}
