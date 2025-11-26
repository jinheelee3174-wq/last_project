using System;
using System.Collections.Generic;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace last_project
{
    public partial class WpfStatistics : System.Windows.Controls.UserControl
    {
        // --- 차트 데이터 속성들 (v2.0은 ISeries 배열을 사용함) ---

        public ISeries[] SeriesA { get; set; }
        public ISeries[] SeriesB { get; set; }
        public ISeries[] CategorySeries { get; set; }

        public ISeries[] HourlySeries { get; set; }
        public Axis[] HourXAxes { get; set; }

        public ISeries[] InOutSeries { get; set; }
        public Axis[] DateXAxes { get; set; }

        public ISeries[] AiGaugeSeries { get; set; }

        public ISeries[] TimeSeries { get; set; }
        public Axis[] TaskXAxes { get; set; }

        public ISeries[] BrandSeries { get; set; }
        public Axis[] BrandXAxes { get; set; }

        public ISeries[] CabinetSeries { get; set; }
        public Axis[] CabinetXAxes { get; set; }

        public ISeries[] CommandSeries { get; set; }
        public Axis[] CommandYAxes { get; set; }

        // 공통 Y축 디자인 (눈금선 얇게)
        public Axis[] CommonYAxes { get; set; }

        public WpfStatistics()
        {
            InitializeComponent();

            // 1. A 수납장 (도넛)
            SeriesA = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 70 }, Name = "사용 중", InnerRadius = 60, Fill = new SolidColorPaint(SKColors.DodgerBlue) },
                new PieSeries<double> { Values = new double[] { 30 }, Name = "빈 공간", InnerRadius = 60, Fill = new SolidColorPaint(new SKColor(40, 40, 40)) }
            };

            // 2. B 수납장 (도넛)
            SeriesB = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 45 }, Name = "사용 중", InnerRadius = 60, Fill = new SolidColorPaint(SKColors.HotPink) },
                new PieSeries<double> { Values = new double[] { 55 }, Name = "빈 공간", InnerRadius = 60, Fill = new SolidColorPaint(new SKColor(40, 40, 40)) }
            };

            // 3. 카테고리 (도넛)
            CategorySeries = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 40 }, Name = "Top", InnerRadius = 40, Fill = new SolidColorPaint(SKColors.OrangeRed) },
                new PieSeries<double> { Values = new double[] { 30 }, Name = "Bottom", InnerRadius = 40, Fill = new SolidColorPaint(SKColors.DeepSkyBlue) },
                new PieSeries<double> { Values = new double[] { 20 }, Name = "Outer", InnerRadius = 40, Fill = new SolidColorPaint(SKColors.MediumSeaGreen) },
                new PieSeries<double> { Values = new double[] { 10 }, Name = "Shoes", InnerRadius = 40, Fill = new SolidColorPaint(SKColors.Gold) }
            };

            // 4. 시간대별 (막대)
            HourlySeries = new ISeries[]
            {
                new ColumnSeries<double> { Values = new double[] { 5, 12, 25, 18, 10, 4 }, Name = "건수", Fill = new SolidColorPaint(SKColors.MediumPurple) }
            };
            HourXAxes = new Axis[] { new Axis { Labels = new[] { "09시", "11시", "13시", "15시", "17시", "19시" }, LabelsPaint = new SolidColorPaint(SKColors.Gray) } };

            // 5. 입출고 (선)
            InOutSeries = new ISeries[]
            {
                new LineSeries<double> { Values = new double[] { 10, 18, 12, 25, 20 }, Name = "입고", Stroke = new SolidColorPaint(SKColors.Cyan) { StrokeThickness = 3 }, Fill = null, GeometrySize = 10 },
                new LineSeries<double> { Values = new double[] { 5, 10, 8, 15, 12 }, Name = "출고", Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 3 }, Fill = null, GeometrySize = 10 }
            };
            DateXAxes = new Axis[] { new Axis { Labels = new[] { "11/15", "11/16", "11/17", "11/18", "11/19" }, LabelsPaint = new SolidColorPaint(SKColors.Gray) } };

            // 6. AI 인식률 (게이지형 파이)
            AiGaugeSeries = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 92.5 }, Name = "성공", InnerRadius = 80, Fill = new SolidColorPaint(SKColors.SpringGreen) },
                new PieSeries<double> { Values = new double[] { 7.5 }, Name = "실패", InnerRadius = 80, Fill = new SolidColorPaint(new SKColor(40, 40, 40)) }
            };

            // 7. 평균 작업 시간 (막대)
            TimeSeries = new ISeries[]
            {
                new ColumnSeries<double> { Values = new double[] { 45, 30 }, Name = "시간(초)", Fill = new SolidColorPaint(SKColors.MediumSlateBlue) }
            };
            TaskXAxes = new Axis[] { new Axis { Labels = new[] { "적재", "출고" }, LabelsPaint = new SolidColorPaint(SKColors.Gray) } };

            // 8. 브랜드별 재고 (스택 막대)
            BrandSeries = new ISeries[]
            {
                new StackedColumnSeries<double> { Values = new double[] { 10, 15, 5, 20 }, Name = "창고 A", StackGroup = 0, Fill = new SolidColorPaint(SKColors.DodgerBlue) },
                new StackedColumnSeries<double> { Values = new double[] { 5, 7, 3, 10 }, Name = "창고 B", StackGroup = 0, Fill = new SolidColorPaint(SKColors.CornflowerBlue) }
            };
            BrandXAxes = new Axis[] { new Axis { Labels = new[] { "Nike", "Adidas", "Puma", "Fila" }, LabelsPaint = new SolidColorPaint(SKColors.Gray) } };

            // 9. 수납장별 브랜드 (그룹 막대)
            CabinetSeries = new ISeries[]
            {
                new ColumnSeries<double> { Values = new double[] { 8, 12 }, Name = "Nike", Fill = new SolidColorPaint(SKColors.Cyan) },
                new ColumnSeries<double> { Values = new double[] { 10, 8 }, Name = "Adidas", Fill = new SolidColorPaint(SKColors.HotPink) }
            };
            CabinetXAxes = new Axis[] { new Axis { Labels = new[] { "A 수납장", "B 수납장" }, LabelsPaint = new SolidColorPaint(SKColors.Gray) } };

            // 10. 명령어 빈도 (가로 막대 - RowSeries)
            CommandSeries = new ISeries[]
            {
                new RowSeries<double> { Values = new double[] { 50, 35, 12 }, Name = "횟수", Fill = new SolidColorPaint(SKColors.LimeGreen) }
            };
            CommandYAxes = new Axis[] { new Axis { Labels = new[] { "전진(F)", "후진(B)", "정지(S)" }, LabelsPaint = new SolidColorPaint(SKColors.White) } };

            // 공통 Y축 설정
            CommonYAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(60, 60, 60)) // 어두운 회색 눈금선
                }
            };

            // 데이터 바인딩 필수!
            this.DataContext = this;
        }
    }
}