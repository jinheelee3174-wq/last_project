using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

// ▼▼▼ 추가된 using 문 ▼▼▼
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
// ▲▲▲ --- ▲▲▲
using System.Windows.Forms.Integration;
using System.Text;



namespace last_project
{

    public partial class main : Form
    {
        private static readonly HttpClient client = new HttpClient();

        // ▼▼▼ WebRTC 카메라 스트림 주소 ▼▼▼
        // (★★★★★ 님 환경의 MediaMTX 서버 IP로 수정하세요 ★★★★★)
        private const string MEDIAMTX_SERVER_IP = "192.168.0.72";
        private const string STREAM_NAME = "mystream";
        private const string WEBRTC_URL = $"http://{MEDIAMTX_SERVER_IP}:8889/{STREAM_NAME}";
        // ▲▲▲ --- ▲▲▲
        private PictureLogViewModel pictureLogViewModel = new PictureLogViewModel();
       

        public main()
        {
            InitializeComponent();
            splitContainer1.Panel1.BackColor = System.Drawing.Color.Gray;
            splitContainer1.BackColor = System.Drawing.Color.Black;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        // =============================================================
        //  ▼▼▼ [신규 추가] 발주 관리(Order Confirmation) 관련 함수들 ▼▼▼
        // =============================================================

        /// <summary>
        /// (1) 메뉴에서 'Order' 버튼 클릭 시 실행 -> 발주 관리 창 띄우기
        /// </summary>
        private async void WpfMenu_BaljuButtonClicked(object sender, EventArgs e)
        {
            // 1. (그릇) ElementHost 생성
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // 2. (내용물) 방금 만든 WpfOrderConfirmation 생성
            WpfOrderConfirmation wpfOrder = new WpfOrderConfirmation();

            // 3. (이벤트 연결) WPF 화면에서 버튼을 눌렀을 때 실행할 C# 로직 연결
            //    - 새로고침
            wpfOrder.RefreshClicked += async (s, ev) => await LoadOrderDataAsync(wpfOrder);

            //    - 승인 (ID를 받아서 처리)
            wpfOrder.ApproveClicked += async (s, orderId) =>
            {
                await UpdateOrderStatusAsync(orderId, "승인됨");
                await LoadOrderDataAsync(wpfOrder); // 처리 후 목록 갱신
            };

            //    - 취소 (ID를 받아서 처리)
            wpfOrder.CancelOrderClicked += async (s, orderId) =>
            {
                await UpdateOrderStatusAsync(orderId, "취소");
                await LoadOrderDataAsync(wpfOrder); // 처리 후 목록 갱신
            };

            // 4. (조립) 그릇에 내용물 담기
            host.Child = wpfOrder;

            // 5. (새 창) 폼 생성 및 설정
            Form orderForm = new Form();
            orderForm.Text = "Order Management System";
            orderForm.Size = new System.Drawing.Size(1000, 700);
            orderForm.StartPosition = FormStartPosition.CenterScreen;
            orderForm.BackColor = System.Drawing.Color.FromArgb(30, 30, 30); // 다크 테마 배경
            orderForm.Controls.Add(host);

            // 6. (데이터 로드) 창을 띄우기 전에 데이터 먼저 가져오기
            await LoadOrderDataAsync(wpfOrder);

            // 7. 창 띄우기
            orderForm.Show();
        }

        /// <summary>
        /// (2) API: 주문 목록 가져오기 (GET /api/orders)
        /// </summary>
        private async Task LoadOrderDataAsync(WpfOrderConfirmation wpfControl)
        {
            string apiUrl = $"http://127.0.0.1:5000/api/orders?_t={DateTime.Now.Ticks}"; // 캐시 방지

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    // JSON을 리스트로 변환 (OrderModel 클래스 필요)
                    var orders = JsonConvert.DeserializeObject<List<OrderModel>>(json);

                    // WPF 화면에 데이터 전달
                    wpfControl.SetOrderData(orders);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"주문 목록 로드 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// (3) API: 주문 상태 변경 (POST /api/order/update_status)
        /// </summary>
        private async Task UpdateOrderStatusAsync(string orderId, string newStatus)
        {
            string apiUrl = "http://127.0.0.1:5000/api/order/update_status";

            try
            {
                var data = new { id = orderId, status = newStatus };
                string json = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // 성공 시 별도 메시지 없이 갱신만 해도 됨 (혹은 로그 남기기)
                    LogManager.Add($"주문 {orderId} 상태 변경 -> {newStatus}");
                }
                else
                {
                    MessageBox.Show("상태 변경 실패 (서버 오류)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"통신 오류: {ex.Message}");
            }
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {
            // (참고) 탭 페이지 자체가 아니라, 그 안의 WebView2 컨트롤이 영상을 띄웁니다.
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await LoadProductDataAsync();
        }

        private void lblClock_Click(object sender, EventArgs e)
        {

        }

        private void UpdateClock()
        {
            var now = DateTime.Now;
            string line1 = now.ToString("yyyy -MM-dd", CultureInfo.InvariantCulture);
            string line2 = now.ToString("dddd", new CultureInfo("en-US"));
            string line3 = now.ToString("HH:mm", CultureInfo.InvariantCulture);
            lblClock.Text = $"{line1}{Environment.NewLine}{line2}{Environment.NewLine}{line3}";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        // ▼▼▼ 폼 로드 함수 (카메라 로직 추가됨) ▼▼▼
        private async void main_Load(object sender, EventArgs e)
        {
            // 시계 설정
            lblClock.AutoSize = false;
            lblClock.Dock = DockStyle.Top;
            lblClock.Height = 80;
            lblClock.TextAlign = ContentAlignment.MiddleCenter;
            lblClock.Padding = new Padding(0, 6, 0, 0);
            lblClock.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
            lblClock.BackColor = System.Drawing.Color.Black;
            timer1.Interval = 1000;
            timer1.Start();
            UpdateClock();

            // 그리드 설정
            dataGridView1.AutoGenerateColumns = false;

            if (!this.DesignMode)
            {
                // 1. 기존 데이터 로드
                await LoadProductDataAsync();

                // 2. ▼▼▼ 카메라 WebView2 초기화 (추가된 부분) ▼▼▼
                await InitializeCameraWebViewAsync();
            }
            // 1. (★★★★★) WPF 메뉴 인스턴스를 만듭니다.
            WpfButtonMenu wpfMenu = new WpfButtonMenu();

            // 2. (★★★★★) WPF가 보낸 "신호"를 main.cs의 함수와 "연결"합니다.

            // (예) WPF의 "Setting" 버튼 신호가 오면 -> 
            //      main.cs의 "btnSetting_Click" 함수를 실행해라
            wpfMenu.SettingButtonClicked += btnSetting_Click;
            wpfMenu.AppLogButtonClicked += btnAppLog_Click;

            // ▼▼▼ [수정된 부분] ▼▼▼
            // "Picture Log" 버튼(BtnLog)의 신호를 WpfMenu_LogButtonClicked 함수와 연결합니다.
            wpfMenu.LogButtonClicked += WpfMenu_LogButtonClicked;

            wpfMenu.TonggyeButtonClicked += WpfMenu_TonggyeButtonClicked;

            // (다른 버튼들도 필요시 여기에 연결)
            // wpfMenu.BaljuButtonClicked += button1_Click; 
            // wpfMenu.TonggyeButtonClicked += button2_Click; 
            // ▲▲▲ [수정 완료] ▲▲▲

            // 3. (핵심) ElementHost(그릇)에 WPF 메뉴(내용물)를 담습니다.
            // (우리가 1단계에서 코드로 만든 'elementHost1' 변수를 사용)
            elementHost1.Child = wpfMenu;

            // 1. (★★★★★) WPF 검색창 인스턴스를 만듭니다.
            WpfSearchBar wpfSearch = new WpfSearchBar();

            // 2. (★★★★★) WPF가 보낸 "신호"를 main.cs의 함수와 "연결"합니다.
            wpfSearch.SearchButtonClicked += WpfSearch_SearchButtonClicked;
            wpfSearch.RefreshButtonClicked += WpfSearch_RefreshButtonClicked;
            wpfMenu.BaljuButtonClicked += WpfMenu_BaljuButtonClicked;
            // 3. (핵심) ElementHost(그릇)에 WPF 검색창(내용물)을 담습니다.
            elementHost2.Child = wpfSearch;

            var grid = dataGridView1;

            // 1. (핵심) 그리드 테두리 없애기
            grid.BorderStyle = BorderStyle.None;

            // 2. 그리드 전체 배경색 (빈 공간)
            grid.BackgroundColor = System.Drawing.Color.Black; // 폼 배경색과 맞춤

            // 3. 헤더(제목) 스타일 설정
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None; // 헤더 테두리 없음
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(25, 25, 25); // 헤더 배경색 (진한 검정)
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White; // 헤더 글자색 (흰색)
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold); // 헤더 폰트
            grid.EnableHeadersVisualStyles = false; // (중요) 이걸 꺼야 위 스타일이 먹힘

            // 4. 셀(칸) 스타일 설정
            grid.RowHeadersVisible = false; // (맨 왼쪽) 행 선택 회색 바 숨기기
            grid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(45, 45, 48); // 셀 배경색 (어두운 회색)
            grid.DefaultCellStyle.ForeColor = System.Drawing.Color.White; // 셀 글자색 (흰색)

            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = System.Drawing.Color.Gray; // 셀 구분선 색상

            // 5. 셀 "선택" 스타일 설정
            grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.CornflowerBlue; // 선택 시 배경색 (WPF 버튼과 비슷하게)
            grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White; // 선택 시 글자색

            // 6. (옵션) 행 높이 조절
            grid.RowTemplate.Height = 30; // 행 높이를 살짝
            grid.ColumnHeadersHeight = 35; // 헤더 높이를 살짝


        }

        /// <summary>
        /// (WPF) "Picture Log" 버튼 클릭 시 실행
        /// </summary>
        // (▼▼▼ 새 폼을 띄우는 이 함수로 통째로 교체하세요 ▼▼▼)
        /// <summary>
        /// (WPF) "Picture Log" 버튼 클릭 시 실행 (새 폼으로 띄우기)
        /// </summary>
        /// <summary>
        /// (WPF) "Picture Log" 버튼 클릭 시 실행 (새 폼으로 띄우기)
        /// </summary>
        private void WpfMenu_LogButtonClicked(object sender, EventArgs e)
        {
            // 1. (그릇) ElementHost 생성
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // 2. (내용물) WpfPictureLog 컨트롤 생성
            WpfPictureLog wpfControl = new WpfPictureLog();

            // 3. (★★★★★) 내용물에 ViewModel(데이터) 연결
            wpfControl.DataContext = this.pictureLogViewModel;

            // 4. (조립) 그릇에 내용물을 담습니다.
            host.Child = wpfControl;

            // 5. (새 폼) WPF 컨트롤을 담을 새 WinForms 폼을 생성합니다.
            Form logForm = new Form();
            logForm.Text = "Picture Log Viewer"; // 폼 제목
            logForm.Size = new System.Drawing.Size(503, 713);
            logForm.StartPosition = FormStartPosition.CenterScreen; // 화면 중앙
            logForm.BackColor = System.Drawing.Color.FromArgb(45, 45, 48); // 배경색

            // 6. 새 폼에 (그릇) ElementHost를 추가합니다.
            logForm.Controls.Add(host);

            // 7. 폼을 띄웁니다.
            logForm.Show(); // (ShowDialog() 아님)

            LogManager.Add("Picture Log 폼을 열었습니다.");

            // --- ▼▼▼ [수정된 테스트 코드!] ▼▼▼ ---

            // 1. 알려주신 경로 ( \ -> \\ 로 변경)
            string myPath = "C:\\Users\\모블\\Desktop\\사진";

            // 2. (★★★★★) '사진' 폴더 안에 있는 실제 파일 4개 추가
            // (3열 그리드에 맞춰 2줄로 표시될 것입니다)

            pictureLogViewModel.AddLog(myPath + "\\거누.jpg", "거누");
            pictureLogViewModel.AddLog(myPath + "\\모블FC.jpg", "모블FC");
            pictureLogViewModel.AddLog(myPath + "\\쏭이형.png", "씅이형");
            pictureLogViewModel.AddLog(myPath + "\\주엽이형.jpg", "주엽이형");
        }
        // ▼▼▼ 새로 추가된 카메라 초기화 함수 ▼▼▼
        private async Task InitializeCameraWebViewAsync()
        {
            try
            {
                // 디자이너에서 추가한 'webViewCam1' 컨트롤을 초기화합니다.
                await webViewCam1.EnsureCoreWebView2Async(null);

                webViewCam1.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                // 초기화가 완료되면 WebRTC 주소로 접속합니다.
                webViewCam1.CoreWebView2.Navigate(WEBRTC_URL);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CAM1 WebView2 초기화 오류: {ex.Message}");
            }
        }
        // ▲▲▲ --- ▲▲▲

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer1_Panel1_DoubleClick(object sender, EventArgs e)
        {
            int step = 60;
            int min = splitContainer1.Panel1MinSize;
            int max = splitContainer1.Width - splitContainer1.Panel2MinSize;
            int next = splitContainer1.SplitterDistance + step;
            if (next > max) next = max;
            splitContainer1.SplitterDistance = next;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click_1(object sender, EventArgs e)
        {
            // (tabPage1_Click과 중복된 이벤트 핸들러로 보입니다. 그대로 둡니다.)
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            setting settingForm = new setting();
            settingForm.ShowDialog();
        }

        private void btnAppLog_Click(object sender, EventArgs e)
        {
            // 1. (로그 기록) "Log" 버튼을 눌렀다는 사실 자체를 로그로 남깁니다.
            // (이 LogManager는 이전에 만들기로 했던 LogManager.cs 클래스입니다)
            LogManager.Add("Log 버튼 클릭. 로그 폼을 엽니다.");

            // 2. (로그 폼 열기) 이전에 만들기로 했던 'LogForm'을 생성하고 엽니다.
            // (Show()를 사용해야 메인 폼과 같이 볼 수 있습니다)
            LogForm logForm = new LogForm();
            logForm.Show();
        }

        private async Task LoadProductDataAsync()
        {
            var targetGrid = dataGridView1;
            string apiUrl = $"http://127.0.0.1:5000/api/products?_t={DateTime.Now.Ticks}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("--- 서버에서 받은 JSON ---");
                    System.Diagnostics.Debug.WriteLine(jsonResponse);

                    DataTable productTable = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
                    targetGrid.DataSource = productTable;
                }
                else
                {
                    MessageBox.Show($"서버 응답 오류: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 중 오류 발생: {ex.Message}");
            }
        }


        // ▼▼▼ [추가할 함수] 통계 창 띄우기 ▼▼▼
        private void WpfMenu_TonggyeButtonClicked(object sender, EventArgs e)
        {
            // 1. (그릇) ElementHost 생성
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // 2. (내용물) 아까 만든 예쁜 통계 화면 생성
            WpfStatistics wpfStats = new WpfStatistics();

            // 3. (조립) 그릇에 내용물 담기
            host.Child = wpfStats;

            // 4. (새 창) 폼 만들어서 띄우기
            Form statsForm = new Form();
            statsForm.Text = "Statistics Dashboard"; // 창 제목
            statsForm.Size = new System.Drawing.Size(1100, 750); // 창 크기
            statsForm.StartPosition = FormStartPosition.CenterScreen; // 화면 중앙에 뜨게
            statsForm.BackColor = System.Drawing.Color.FromArgb(30, 30, 30); // 배경색 어둡게

            // 폼에 그릇 추가하고 보여주기
            statsForm.Controls.Add(host);
            statsForm.Show(); // (ShowDialog()로 하면 창 끌 때까지 메인화면 못 씀)
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                string stockStatusColumnName = "Column1";
                string stockValueColumnName = "Column7";

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[stockValueColumnName].Value != null)
                    {
                        int stock = Convert.ToInt32(row.Cells[stockValueColumnName].Value);
                        DataGridViewCell statusCell = row.Cells[stockStatusColumnName];

                        if (stock <= 2) // 2개 이하 (위험)
                        {
                            statusCell.Value = "위험";
                            statusCell.Style.ForeColor = System.Drawing.Color.Red;
                            statusCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                        }
                        else if (stock == 3) // 딱 3개 (주의)
                        {
                            statusCell.Value = "주의";
                            statusCell.Style.ForeColor = System.Drawing.Color.Orange;
                            statusCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                        }
                        else // 4개 이상 (정상)
                        {
                            statusCell.Value = "정상";
                            statusCell.Style.ForeColor = System.Drawing.Color.Green;
                            statusCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"재고 상태 업데이트 중 오류: {ex.Message}");
            }
        }

       

        private void WpfSearch_SearchButtonClicked(object sender, EventArgs e)
        {
            // 1. (★★★★★) elementHost2(그릇)에서 WpfSearchBar(내용물)를 꺼냅니다.
            WpfSearchBar wpfSearch = elementHost2.Child as WpfSearchBar;
            if (wpfSearch == null) return;

            // 2. WpfSearchBar에서 검색어를 가져옵니다.
            string searchTerm = wpfSearch.SearchTerm.Trim();

            // 3. 그리드의 DataSource를 DataTable로 변환합니다.
            DataTable table = dataGridView1.DataSource as DataTable;
            if (table == null) return;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                table.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                // 4. (핵심) DataTable 필터 실행
                string safeSearchTerm = searchTerm.Replace("'", "''");
                table.DefaultView.RowFilter = string.Format(
                    "item_code LIKE '%{0}%' OR " +
                    "product_name LIKE '%{0}%' OR " +
                    "brand LIKE '%{0}%' OR " +
                    "color LIKE '%{0}%' OR " +
                    "size LIKE '%{0}%' OR " +
                    "category LIKE '%{0}%'",
                    safeSearchTerm
                );
            }
        }

        private async void WpfSearch_RefreshButtonClicked(object sender, EventArgs e)
        {
            // 1. (★★★★★) elementHost2(그릇)에서 WpfSearchBar(내용물)를 꺼냅니다.
            WpfSearchBar wpfSearch = elementHost2.Child as WpfSearchBar;

            // 2. 검색 필터를 해제합니다.
            DataTable table = dataGridView1.DataSource as DataTable;
            if (table != null)
            {
                table.DefaultView.RowFilter = string.Empty;
            }

            // 3. 검색 텍스트박스를 비웁니다.
            if (wpfSearch != null)
            {
                wpfSearch.SearchTerm = "";
            }

            // 4. [기존 기능] 서버에서 새 데이터를 로드합니다.
            await LoadProductDataAsync();
        }


        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // 페이지 로드가 성공했을 때만 실행
            if (e.IsSuccess)
            {
                // MediaMTX의 기본 WebRTC 플레이어는 <video> 태그를 사용합니다.
                // 이 <video> 태그가 컨테이너를 꽉 채우도록 CSS를 주입합니다.

                // 1. (추천) "Cover" 모드:
                //    비율을 유지하면서 꽉 채웁니다. (영상의 상/하 또는 좌/우 일부가 잘릴 수 있음)
                string css = @"
                    video {
                        object-fit: cover !important; /* 'cover'로 설정 */
                        width: 100% !important;
                        height: 100% !important;
                        left: 0 !important;
                        top: 0 !important;
                    }
                    body {
                        overflow: hidden !important; /* 스크롤바 숨기기 */
                        background-color: black !important; /* 여백 배경 검은색 */
                    }
                ";

                // 2. (참고) "Fill" 모드:
                //    비율을 무시하고 꽉 채웁니다. (영상이 찌그러져 보일 수 있음)
                /*
                string css = @"
                    video {
                        object-fit: fill !important; // 'fill'로 설정
                        width: 100% !important;
                        height: 100% !important;
                        left: 0 !important;
                        top: 0 !important;
                    }
                    body {
                        overflow: hidden !important;
                        background-color: black !important;
                    }
                ";
                */


                // CSS를 <style> 태그로 만들어서 <body>에 주입하는 스크립트 실행
                string script = $@"
                    var style = document.createElement('style');
                    style.type = 'text/css';
                    style.innerHTML = `{css.Replace("\n", "").Replace("\r", "")}`;
                    document.body.appendChild(style);
                ";

                await webViewCam1.CoreWebView2.ExecuteScriptAsync(script);
            }
        }





    } // public partial class main 끝





    public class Product
    {
        public int id { get; set; }
        public string item_code { get; set; }
        public string product_name { get; set; }
        public string brand { get; set; }
        public string category { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public int stock { get; set; }
    }
} // namespace last_project 끝