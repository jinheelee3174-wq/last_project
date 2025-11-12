using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using InputMouseEventArgs = System.Windows.Input.MouseEventArgs;
using InputMouseButtonEventArgs = System.Windows.Input.MouseButtonEventArgs;
using InputMouseButtonState = System.Windows.Input.MouseButtonState;
using MediaBrushes = System.Windows.Media.Brushes;
using MediaColor = System.Windows.Media.Color;
using WpfPoint = System.Windows.Point;
using WpfRectangle = System.Windows.Shapes.Rectangle;

namespace last_project
{
    /// <summary>
    /// WpfSlotEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WpfSlotEditor : System.Windows.Controls.UserControl
    {
        private WpfPoint startPoint;
        private WpfRectangle rect;

        /// <summary>
        /// Raised when the user finishes drawing a rectangle on the canvas.
        /// </summary>
        public event EventHandler<SlotDrawnEventArgs> SlotDrawn;

        public WpfSlotEditor()
        {
            InitializeComponent();
        }

        private void CamSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Placeholder for future camera selection logic.
        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender, InputMouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(DrawingCanvas);

            if (rect != null)
            {
                DrawingCanvas.Children.Remove(rect);
            }

            rect = new WpfRectangle
            {
                Stroke = MediaBrushes.Red,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(MediaColor.FromArgb(50, 255, 0, 0))
            };

            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y);

            DrawingCanvas.Children.Add(rect);
        }

        private void DrawingCanvas_MouseMove(object sender, InputMouseEventArgs e)
        {
            if (e.LeftButton == InputMouseButtonState.Released || rect == null)
            {
                return;
            }

            WpfPoint pos = e.GetPosition(DrawingCanvas);

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            rect.Width = w;
            rect.Height = h;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        private void DrawingCanvas_MouseLeftButtonUp(object sender, InputMouseButtonEventArgs e)
        {
            if (rect == null)
            {
                return;
            }

            double x = Canvas.GetLeft(rect);
            double y = Canvas.GetTop(rect);
            double w = rect.Width;
            double h = rect.Height;

            SlotDrawn?.Invoke(this, new SlotDrawnEventArgs(x, y, w, h));

            rect = null;
        }
    }

    public class SlotDrawnEventArgs : EventArgs
    {
        public double X { get; }
        public double Y { get; }
        public double W { get; }
        public double H { get; }

        public SlotDrawnEventArgs(double x, double y, double w, double h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
