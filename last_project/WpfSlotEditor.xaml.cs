using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using InputMouseEventArgs = System.Windows.Input.MouseEventArgs;
using InputMouseButtonEventArgs = System.Windows.Input.MouseButtonEventArgs;
using InputMouseButtonState = System.Windows.Input.MouseButtonState;
using MediaBrushes = System.Windows.Media.Brushes;
using MediaColor = System.Windows.Media.Color;
using WpfPoint = System.Windows.Point;
using WpfRectangle = System.Windows.Shapes.Rectangle;

namespace last_project
{
    public partial class WpfSlotEditor : System.Windows.Controls.UserControl
    {
        // 라즈베리파이 스트림 주소 (main.cs와 맞게 수정)
        private const string CAM1_URL = "http://192.168.0.79:8000/stream.mjpg";
        private const string CAM2_URL = "http://192.168.0.112:8000/stream.mjpg";
        private const string CAM3_URL = "http://192.168.0.28:8000/stream.mjpg";

        private WpfPoint startPoint;
        private WpfRectangle rect;

        public event EventHandler<SlotDrawnEventArgs> SlotDrawn;

        public WpfSlotEditor()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                await SlotWebView.EnsureCoreWebView2Async(null);
                SlotWebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                SlotWebView.CoreWebView2.Navigate(CAM1_URL);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"웹뷰 초기화 실패: {ex.Message}");
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess || SlotWebView.CoreWebView2 == null) return;

            string css = @"
                body {
                    margin: 0;
                    padding: 0;
                    overflow: hidden;
                    background-color: black;
                }
                img, video {
                    width: 100%;
                    height: 100%;
                    object-fit: fill;
                    display: block;
                }";

            string script =
                "var style = document.createElement('style');" +
                "style.type = 'text/css';" +
                $"style.innerHTML = `{css}`;" +
                "document.head.appendChild(style);";

            await SlotWebView.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void CamSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SlotWebView?.CoreWebView2 == null) return;

            if (CamSelector.SelectedItem is ComboBoxItem item)
            {
                string url = CAM1_URL;
                string tag = item.Tag as string;

                if (tag == "cam2") url = CAM2_URL;
                else if (tag == "cam3") url = CAM3_URL;

                SlotWebView.CoreWebView2.Navigate(url);
            }
        }

        // ---------- 드래그로 사각형 그리기(영상 위 Canvas) ----------

        private void DrawingCanvas_MouseLeftButtonDown(object sender, InputMouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(DrawingCanvas);

            // 이전 사각형 제거
            DrawingCanvas.Children.Clear();

            rect = new WpfRectangle
            {
                Stroke = MediaBrushes.Red,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(MediaColor.FromArgb(50, 255, 0, 0)) // 반투명 빨강
            };

            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y);
            DrawingCanvas.Children.Add(rect);
        }

        private void DrawingCanvas_MouseMove(object sender, InputMouseEventArgs e)
        {
            if (e.LeftButton == InputMouseButtonState.Released || rect == null)
                return;

            WpfPoint pos = e.GetPosition(DrawingCanvas);

            double x = Math.Min(pos.X, startPoint.X);
            double y = Math.Min(pos.Y, startPoint.Y);
            double w = Math.Max(pos.X, startPoint.X) - x;
            double h = Math.Max(pos.Y, startPoint.Y) - y;

            rect.Width = w;
            rect.Height = h;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        private void DrawingCanvas_MouseLeftButtonUp(object sender, InputMouseButtonEventArgs e)
        {
            if (rect == null) return;

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
