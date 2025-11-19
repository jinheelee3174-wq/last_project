using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

// ▼▼▼ [핵심 1] 색상 충돌 방지 코드 (이게 있어야 빨간 줄이 안 뜹니다) ▼▼▼
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace last_project
{
    /// <summary>
    /// WpfStatistics.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WpfStatistics : System.Windows.Controls.UserControl
    {
        // --- 1. 상단 원형(도넛) 차트 데이터 ---
        public SeriesCollection SeriesCollectionA { get; set; }
        public SeriesCollection SeriesCollectionB { get; set; }
        public string A_Percent { get; set; }
        public string B_Percent { get; set; }

        // --- 2. 중간 차트 데이터 ---
        public SeriesCollection InOutSeries { get; set; }
        public string[] DateLabels { get; set; }

        public double AiSuccessRate { get; set; }

        public SeriesCollection AvgTimeSeries { get; set; }
        public string[] TaskLabels { get; set; }

        // --- 3. 하단 차트 데이터 ---
        public SeriesCollection BrandStockSeries { get; set; }
        public string[] BrandLabels { get; set; }

        public SeriesCollection CabinetSeries { get; set; }
        public string[] CabinetLabels { get; set; }

        public SeriesCollection CommandSeries { get; set; }
        public string[] CommandLabels { get; set; }

        public WpfStatistics()
        {
            InitializeComponent();

            // 1. [상단] 수납장 적재율
            A_Percent = "70%";
            SeriesCollectionA = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "사용 중",
                    Values = new ChartValues<double> { 70 },
                    Fill = Brushes.DodgerBlue,
                    StrokeThickness = 0,
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "빈 공간",
                    Values = new ChartValues<double> { 30 },
                    Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                    StrokeThickness = 0,
                    DataLabels = false
                }
            };

            B_Percent = "45%";
            SeriesCollectionB = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "사용 중",
                    Values = new ChartValues<double> { 45 },
                    Fill = Brushes.HotPink,
                    StrokeThickness = 0,
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "빈 공간",
                    Values = new ChartValues<double> { 55 },
                    Fill = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                    StrokeThickness = 0,
                    DataLabels = false
                }
            };

            // 2. [중간] 날짜별 입고/출고 추이
            InOutSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "입고",
                    Values = new ChartValues<double> { 10, 18, 12, 25, 20 },
                    PointGeometrySize = 10,
                    Stroke = Brushes.Cyan,
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0
                },
                new LineSeries
                {
                    Title = "출고",
                    Values = new ChartValues<double> { 5, 10, 8, 15, 12 },
                    PointGeometrySize = 10,
                    Stroke = Brushes.Orange,
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0
                }
            };
            DateLabels = new[] { "11/15", "11/16", "11/17", "11/18", "11/19" };

            // 3. [중간] AI 빈 슬롯 감지 성공률
            AiSuccessRate = 92.5;

            // 4. [중간] 평균 작업시간
            AvgTimeSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "작업 시간",
                    Values = new ChartValues<double> { 45, 30 },
                    Fill = Brushes.MediumPurple,
                    DataLabels = true,
                    MaxColumnWidth = 50
                }
            };
            TaskLabels = new[] { "적재", "출고" };

            // 5. [하단] 브랜드별 재고 상태
            BrandStockSeries = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title = "창고 A",
                    Values = new ChartValues<double> { 10, 15, 5, 20 },
                    Fill = Brushes.DodgerBlue,
                    DataLabels = true
                },
                new StackedColumnSeries
                {
                    Title = "창고 B",
                    Values = new ChartValues<double> { 5, 7, 3, 10 },
                    Fill = Brushes.CornflowerBlue,
                    DataLabels = true
                }
            };
            BrandLabels = new[] { "Nike", "Adidas", "Puma", "Fila" };

            // 6. [하단] 수납장별 브랜드 분류
            CabinetSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Nike",
                    Values = new ChartValues<double> { 8, 12 },
                    Fill = Brushes.Cyan
                },
                new ColumnSeries
                {
                    Title = "Adidas",
                    Values = new ChartValues<double> { 10, 8 },
                    Fill = Brushes.HotPink
                },
                new ColumnSeries
                {
                    Title = "Other",
                    Values = new ChartValues<double> { 4, 5 },
                    Fill = Brushes.Yellow
                }
            };
            CabinetLabels = new[] { "A 수납장", "B 수납장" };

            // 7. [하단] 수동제어 명령어 사용빈도
            CommandSeries = new SeriesCollection
            {
                new RowSeries
                {
                    Title = "사용 횟수",
                    Values = new ChartValues<double> { 50, 35, 12 },
                    Fill = Brushes.SpringGreen,
                    DataLabels = true
                    // ▼▼▼ [수정 완료] 오류가 나던 MaxRowHeight 줄을 삭제했습니다! ▼▼▼
                }
            };
            CommandLabels = new[] { "전진(F)", "후진(B)", "정지(S)" };

            this.DataContext = this;
        }
    }
}