using System;
using System.Collections.Generic;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp; // [필수]

namespace last_project
{
    public partial class WpfStatistics : System.Windows.Controls.UserControl
    {
        // --- 차트 데이터 속성들 ---
        public ISeries[] SeriesA { get; set; }
        public ISeries[] SeriesB { get; set; }
        public ISeries[] CategorySeries { get; set; }
        public ISeries[] HourlySeries { get; set; }
        public ISeries[] InOutSeries { get; set; }
        public ISeries[] AiGaugeSeries { get; set; }
        public ISeries[] TimeSeries { get; set; }
        public ISeries[] BrandSeries { get; set; }
        public ISeries[] CabinetSeries { get; set; }
        public ISeries[] CommandSeries { get; set; }

        // --- 축 설정 ---
        public Axis[] HourXAxes { get; set; }
        public Axis[] DateXAxes { get; set; }
        public Axis[] TaskXAxes { get; set; }
        public Axis[] BrandXAxes { get; set; }
        public Axis[] CabinetXAxes { get; set; }
        public Axis[] CommandYAxes { get; set; }
        public Axis[] CommonYAxes { get; set; }

        // 툴팁 폰트 설정
        public SolidColorPaint TooltipTextPaint { get; set; }

        public WpfStatistics()
        {
            InitializeComponent();

            // 1. 한글 폰트(맑은 고딕)
            var koreanTypeface = SKTypeface.FromFamilyName("Malgun Gothic");

            // 2. 축 라벨용 페인트 (회색)
            var grayTextPaint = new SolidColorPaint
            {
                Color = SKColors.Gray,
                SKTypeface = koreanTypeface
            };

            // 3. 축 라벨용 페인트 (흰색)
            var whiteTextPaint = new SolidColorPaint
            {
                Color = SKColors.White,
                SKTypeface = koreanTypeface
            };

            // 4. [수정 완료] 툴팁용 페인트 (진한 검은색 계열)
            // 배경이 밝은 회색이므로 글씨는 진해야 잘 보입니다!
            TooltipTextPaint = new SolidColorPaint
            {
                Color = new SKColor(30, 30, 30), // 진한 회색 (거의 검정)
                SKTypeface = koreanTypeface
            };

            // ---------------------------------------------------------
            // 데이터 설정
            // ---------------------------------------------------------

            // 1. A 수납장
            SeriesA = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 70 }, Name = "사용 중", InnerRadius = 60, Fill = new SolidColorPaint(SKColors.DodgerBlue) },
                new PieSeries<double> { Values = new double[] { 30 }, Name = "빈 공간", InnerRadius = 60, Fill = new SolidColorPaint(new SKColor(40, 40, 40)) }
            };

            // 2. B 수납장
            SeriesB = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 45 }, Name = "사용 중", InnerRadius = 60, Fill = new SolidColorPaint(SKColors.HotPink) },
                new PieSeries<double> { Values = new double[] { 55 }, Name = "빈 공간", InnerRadius = 60, Fill = new SolidColorPaint(new SKColor(40, 40, 40)) }
            };

            // 3. 카테고리
            CategorySeries = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 40 }, Name = "상의", InnerRadius = 40, Fill = new SolidColorPaint(SKColors.OrangeRed) },
                new PieSeries<double> { Values = new double[] { 30 }, Name = "하의", InnerRadius = 40, Fill = new SolidColorPaint(SKColors.DeepSkyBlue) },
                new PieSeries<double> { Values = new double[] { 20 }, Name = "아우터", InnerRadius = 40, Fill = new SolidColorPaint(SKColors.MediumSeaGreen) },
                new PieSeries<double> { Values = new double[] { 10 }, Name = "신발", InnerRadius = 40, Fill = new SolidColorPaint(SKColors.Gold) }
            };

            // 4. 시간대별
            HourlySeries = new ISeries[] { new ColumnSeries<double> { Values = new double[] { 5, 12, 25, 18, 10, 4 }, Name = "건수", Fill = new SolidColorPaint(SKColors.MediumPurple) } };
            HourXAxes = new Axis[] { new Axis { Labels = new[] { "09시", "11시", "13시", "15시", "17시", "19시" }, LabelsPaint = grayTextPaint } };

            // 5. 입출고
            InOutSeries = new ISeries[] {
                new LineSeries<double> { Values = new double[] { 10, 18, 12, 25, 20 }, Name = "입고", Stroke = new SolidColorPaint(SKColors.Cyan) { StrokeThickness = 3 }, Fill = null, GeometrySize = 10 },
                new LineSeries<double> { Values = new double[] { 5, 10, 8, 15, 12 }, Name = "출고", Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 3 }, Fill = null, GeometrySize = 10 }
            };
            DateXAxes = new Axis[] { new Axis { Labels = new[] { "11/15", "11/16", "11/17", "11/18", "11/19" }, LabelsPaint = grayTextPaint } };

            // 6. AI 인식률
            AiGaugeSeries = new ISeries[] {
                new PieSeries<double> { Values = new double[] { 92.5 }, Name = "성공", InnerRadius = 80, Fill = new SolidColorPaint(SKColors.SpringGreen) },
                new PieSeries<double> { Values = new double[] { 7.5 }, Name = "실패", InnerRadius = 80, Fill = new SolidColorPaint(new SKColor(40, 40, 40)) }
            };

            // 7. 평균 작업 시간
            TimeSeries = new ISeries[] { new ColumnSeries<double> { Values = new double[] { 45, 30 }, Name = "시간(초)", Fill = new SolidColorPaint(SKColors.MediumSlateBlue) } };
            TaskXAxes = new Axis[] { new Axis { Labels = new[] { "적재", "출고" }, LabelsPaint = grayTextPaint } };

            // 8. 브랜드별 재고
            BrandSeries = new ISeries[] {
                new StackedColumnSeries<double> { Values = new double[] { 10, 15, 5, 20 }, Name = "창고 A", StackGroup = 0, Fill = new SolidColorPaint(SKColors.DodgerBlue) },
                new StackedColumnSeries<double> { Values = new double[] { 5, 7, 3, 10 }, Name = "창고 B", StackGroup = 0, Fill = new SolidColorPaint(SKColors.CornflowerBlue) }
            };
            BrandXAxes = new Axis[] { new Axis { Labels = new[] { "Nike", "Adidas", "Puma", "Fila" }, LabelsPaint = grayTextPaint } };

            // 9. 수납장별 브랜드
            CabinetSeries = new ISeries[] {
                new ColumnSeries<double> { Values = new double[] { 8, 12 }, Name = "Nike", Fill = new SolidColorPaint(SKColors.Cyan) },
                new ColumnSeries<double> { Values = new double[] { 10, 8 }, Name = "Adidas", Fill = new SolidColorPaint(SKColors.HotPink) }
            };
            CabinetXAxes = new Axis[] { new Axis { Labels = new[] { "A 수납장", "B 수납장" }, LabelsPaint = grayTextPaint } };

            // 10. 명령어 빈도
            CommandSeries = new ISeries[] { new RowSeries<double> { Values = new double[] { 50, 35, 12 }, Name = "횟수", Fill = new SolidColorPaint(SKColors.LimeGreen) } };
            CommandYAxes = new Axis[] { new Axis { Labels = new[] { "전진(F)", "후진(B)", "정지(S)" }, LabelsPaint = whiteTextPaint } };

            // 공통 Y축 설정
            CommonYAxes = new Axis[] { new Axis { LabelsPaint = grayTextPaint, SeparatorsPaint = new SolidColorPaint(new SKColor(60, 60, 60)) } };

            this.DataContext = this;
        }
    }
}