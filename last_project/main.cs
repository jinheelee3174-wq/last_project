using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.Windows.Forms.Integration;
using System.Text;

namespace last_project
{
    public partial class main : Form
    {
        private static readonly HttpClient client = new HttpClient();

        // =================================================================================
        // ★ [설정] 라즈베리파이 4대의 IP 주소와 스트리밍 포트(8000)를 여기에 입력하세요.
        // =================================================================================
        private const string CAM1_URL = "http://192.168.0.79:8000/stream.mjpg"; // 라파 1번 IP
        private const string CAM2_URL = "http://192.168.0.112:8000/stream.mjpg"; // 라파 2번 IP
        private const string CAM3_URL = "http://192.168.0.34:8000/stream.mjpg"; // 라파 3번 IP
        private const string CAM4_URL = "http://192.168.0.97:8000/stream.mjpg"; // 라파 4번 IP 

        // ★ 데이터 등을 가져올 Flask 서버 주소 (윈폼 PC 또는 별도 서버 IP)
        private const string FLASK_SERVER_URL = "http://127.0.0.1:5000";

        private PictureLogViewModel pictureLogViewModel = new PictureLogViewModel();

        public main()
        {
            InitializeComponent();
            // 디자인 초기 설정
            splitContainer1.Panel1.BackColor = System.Drawing.Color.Gray;
            splitContainer1.BackColor = System.Drawing.Color.Black;
        }

        private void groupBox1_Enter(object sender, EventArgs e) { }

        // =============================================================
        //  [1] 폼 로드 & 초기화 (카메라 연결 포함)
        // =============================================================
        private async void main_Load(object sender, EventArgs e)
        {
            // 1. 시계 UI 설정
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

            // 2. 그리드 설정
            dataGridView1.AutoGenerateColumns = false;
            StyleDataGridView(dataGridView1); // 그리드 스타일 함수 호출

            if (!this.DesignMode)
            {
                // 제품 데이터 로드
                await LoadProductDataAsync();

                // ★ 카메라(WebView2) 초기화 및 연결 함수 호출
                await InitializeCameraWebViewAsync();
            }

            // 3. WPF 메뉴(왼쪽) 설정
            WpfButtonMenu wpfMenu = new WpfButtonMenu();
            wpfMenu.SettingButtonClicked += btnSetting_Click;
            wpfMenu.AppLogButtonClicked += btnAppLog_Click;
            wpfMenu.LogButtonClicked += WpfMenu_LogButtonClicked;
            wpfMenu.TonggyeButtonClicked += WpfMenu_TonggyeButtonClicked;
            wpfMenu.BaljuButtonClicked += WpfMenu_BaljuButtonClicked;
            elementHost1.Child = wpfMenu;

            // 4. WPF 검색창(상단) 설정
            WpfSearchBar wpfSearch = new WpfSearchBar();
            wpfSearch.SearchButtonClicked += WpfSearch_SearchButtonClicked;
            wpfSearch.RefreshButtonClicked += WpfSearch_RefreshButtonClicked;
            elementHost2.Child = wpfSearch;
        }

        // =============================================================
        //  [핵심] 카메라 초기화 및 연결 (WebView2 -> 라즈베리파이 직접 접속)
        // =============================================================
        private async Task InitializeCameraWebViewAsync()
        {
            try
            {
                // 1. 모든 WebView2 컨트롤 초기화 (비동기 대기)
                // (Designer.cs에 webViewCam1~4, webViewAll이 선언되어 있어야 합니다)
                if (webViewCam1 != null) await webViewCam1.EnsureCoreWebView2Async(null);
                if (webViewCam2 != null) await webViewCam2.EnsureCoreWebView2Async(null);
                if (webViewCam3 != null) await webViewCam3.EnsureCoreWebView2Async(null);
                if (webViewCam4 != null) await webViewCam4.EnsureCoreWebView2Async(null);
                if (webViewAll != null) await webViewAll.EnsureCoreWebView2Async(null);

                // 2. 각 탭별로 라즈베리파이 스트리밍 주소에 접속
                if (webViewCam1 != null) webViewCam1.CoreWebView2.Navigate(CAM1_URL);
                if (webViewCam2 != null) webViewCam2.CoreWebView2.Navigate(CAM2_URL);
                if (webViewCam3 != null) webViewCam3.CoreWebView2.Navigate(CAM3_URL);
                if (webViewCam4 != null) webViewCam4.CoreWebView2.Navigate(CAM4_URL);

                // 3. [핵심] All CAM 탭 (4분할 화면) HTML 생성 및 로드
                if (webViewAll != null)
                {
                    // 4개의 카메라 주소를 img 태그로 4분할 그리드에 배치하는 HTML
                    string fourSplitHtml = $@"
                        <html>
                        <head>
                            <style>
                                body {{ 
                                    margin: 0; 
                                    background-color: #111; 
                                    display: grid; 
                                    grid-template-columns: 50% 50%; 
                                    grid-template-rows: 50% 50%; 
                                    height: 100vh; 
                                    overflow: hidden; 
                                }}
                                .cam-box {{ 
                                    position: relative; 
                                    width: 100%; 
                                    height: 100%; 
                                    border: 1px solid #333; 
                                    box-sizing: border-box; 
                                }}
                                img {{ 
                                    width: 100%; 
                                    height: 100%; 
                                    object-fit: fill; /* 비율 무시하고 꽉 채움 */
                                    display: block; 
                                }}
                                .label {{ 
                                    position: absolute; 
                                    top: 5px; left: 5px; 
                                    color: #00FF00; 
                                    font-weight: bold; 
                                    background: rgba(0, 0, 0, 0.5); 
                                    padding: 2px 6px; 
                                    font-family: sans-serif; 
                                    font-size: 14px; 
                                    border-radius: 4px;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='cam-box'>
                                <div class='label'>CAM 1 (고정)</div>
                                <img src='{CAM1_URL}' onerror=""this.style.display='none'"">
                            </div>
                            <div class='cam-box'>
                                <div class='label'>CAM 2 (고정)</div>
                                <img src='{CAM2_URL}' onerror=""this.style.display='none'"">
                            </div>
                            <div class='cam-box'>
                                <div class='label'>CAM 3 (고정)</div>
                                <img src='{CAM3_URL}' onerror=""this.style.display='none'"">
                            </div>
                            <div class='cam-box'>
                                <div class='label'>CAM 4 (이동형)</div>
                                <img src='{CAM4_URL}' onerror=""this.style.display='none'"">
                            </div>
                        </body>
                        </html>";

                    // HTML 문자열을 브라우저에 바로 렌더링
                    webViewAll.CoreWebView2.NavigateToString(fourSplitHtml);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"카메라 초기화 오류: {ex.Message}");
            }
        }

        // =============================================================
        //  [2] 발주 관리 (Order Confirmation)
        // =============================================================
        private async void WpfMenu_BaljuButtonClicked(object sender, EventArgs e)
        {
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            WpfOrderConfirmation wpfOrder = new WpfOrderConfirmation();

            // 이벤트 연결
            wpfOrder.RefreshClicked += async (s, ev) => await LoadOrderDataAsync(wpfOrder);
            wpfOrder.ApproveClicked += async (s, orderId) =>
            {
                await UpdateOrderStatusAsync(orderId, "승인됨");
                await LoadOrderDataAsync(wpfOrder);
            };
            wpfOrder.CancelOrderClicked += async (s, orderId) =>
            {
                await UpdateOrderStatusAsync(orderId, "취소");
                await LoadOrderDataAsync(wpfOrder);
            };

            host.Child = wpfOrder;

            // 새 창 띄우기
            Form orderForm = new Form();
            orderForm.Text = "Order Management System";
            orderForm.Size = new System.Drawing.Size(1000, 700);
            orderForm.StartPosition = FormStartPosition.CenterScreen;
            orderForm.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            orderForm.Controls.Add(host);

            await LoadOrderDataAsync(wpfOrder);
            orderForm.ShowDialog();
        }

        private async Task LoadOrderDataAsync(WpfOrderConfirmation wpfControl)
        {
            string apiUrl = $"{FLASK_SERVER_URL}/api/orders?_t={DateTime.Now.Ticks}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var orders = JsonConvert.DeserializeObject<List<OrderModel>>(json);
                    wpfControl.SetOrderData(orders);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"주문 목록 로드 실패: {ex.Message}");
            }
        }

        private async Task UpdateOrderStatusAsync(string orderId, string newStatus)
        {
            string apiUrl = $"{FLASK_SERVER_URL}/api/order/update_status";

            try
            {
                var data = new { id = orderId, status = newStatus };
                string json = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
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

        // =============================================================
        //  [3] Picture Log (사진 로그)
        // =============================================================
        private void WpfMenu_LogButtonClicked(object sender, EventArgs e)
        {
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            WpfPictureLog wpfControl = new WpfPictureLog();
            wpfControl.DataContext = this.pictureLogViewModel;
            host.Child = wpfControl;

            Form logForm = new Form();
            logForm.Text = "Picture Log Viewer";
            logForm.Size = new System.Drawing.Size(503, 713);
            logForm.StartPosition = FormStartPosition.CenterScreen;
            logForm.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            logForm.Controls.Add(host);
            logForm.ShowDialog();

            LogManager.Add("Picture Log 폼을 열었습니다.");

            // (테스트용 샘플 데이터 추가)
            string myPath = "C:\\Users\\모블\\Desktop\\사진";
            try
            {
                pictureLogViewModel.AddLog(myPath + "\\거누.jpg", "거누");
                pictureLogViewModel.AddLog(myPath + "\\모블FC.jpg", "모블FC");
                pictureLogViewModel.AddLog(myPath + "\\쏭이형.png", "씅이형");
                pictureLogViewModel.AddLog(myPath + "\\주엽이형.jpg", "주엽이형");
            }
            catch { }
        }

        // =============================================================
        //  [4] 통계 (Statistics)
        // =============================================================
        private void WpfMenu_TonggyeButtonClicked(object sender, EventArgs e)
        {
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            WpfStatistics wpfStats = new WpfStatistics();
            host.Child = wpfStats;

            Form statsForm = new Form();
            statsForm.Text = "Statistics Dashboard";
            statsForm.Size = new System.Drawing.Size(1100, 750);
            statsForm.StartPosition = FormStartPosition.CenterScreen;
            statsForm.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            statsForm.Controls.Add(host);
            statsForm.ShowDialog();
        }

        // =============================================================
        //  [5] 제품 데이터 관리 (메인 그리드)
        // =============================================================
        private async Task LoadProductDataAsync()
        {
            var targetGrid = dataGridView1;
            string apiUrl = $"{FLASK_SERVER_URL}/api/products?_t={DateTime.Now.Ticks}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
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

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                // 컬럼 인덱스 또는 이름으로 접근
                string stockStatusColumnName = "Column1";
                string stockValueColumnName = "Column7";

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[stockValueColumnName].Value != null)
                    {
                        int stock = Convert.ToInt32(row.Cells[stockValueColumnName].Value);
                        DataGridViewCell statusCell = row.Cells[stockStatusColumnName];

                        if (stock <= 2)
                        {
                            statusCell.Value = "위험";
                            statusCell.Style.ForeColor = System.Drawing.Color.Red;
                            statusCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                        }
                        else if (stock == 3)
                        {
                            statusCell.Value = "주의";
                            statusCell.Style.ForeColor = System.Drawing.Color.Orange;
                            statusCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                        }
                        else
                        {
                            statusCell.Value = "정상";
                            statusCell.Style.ForeColor = System.Drawing.Color.Green;
                            statusCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                        }
                    }
                }
            }
            catch {


            
            }
        }

        // =============================================================
        //  [6] 검색바 및 기타 이벤트
        // =============================================================
        private void WpfSearch_SearchButtonClicked(object sender, EventArgs e)
        {
            WpfSearchBar wpfSearch = elementHost2.Child as WpfSearchBar;
            if (wpfSearch == null) return;

            string searchTerm = wpfSearch.SearchTerm.Trim();
            DataTable table = dataGridView1.DataSource as DataTable;
            if (table == null) return;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                table.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                string safeSearchTerm = searchTerm.Replace("'", "''");
                table.DefaultView.RowFilter = string.Format(
                    "item_code LIKE '%{0}%' OR product_name LIKE '%{0}%' OR brand LIKE '%{0}%' OR color LIKE '%{0}%' OR size LIKE '%{0}%' OR category LIKE '%{0}%'",
                    safeSearchTerm
                );
            }
        }

        private async void WpfSearch_RefreshButtonClicked(object sender, EventArgs e)
        {
            WpfSearchBar wpfSearch = elementHost2.Child as WpfSearchBar;
            DataTable table = dataGridView1.DataSource as DataTable;
            if (table != null) table.DefaultView.RowFilter = string.Empty;
            if (wpfSearch != null) wpfSearch.SearchTerm = "";
            await LoadProductDataAsync();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            setting settingForm = new setting();
            settingForm.ShowDialog();
        }

        private void btnAppLog_Click(object sender, EventArgs e)
        {
            LogManager.Add("Log 버튼 클릭.");
            LogForm logForm = new LogForm();
            logForm.ShowDialog();
        }

        // =============================================================
        //  [보조] 시계 및 UI 스타일링
        // =============================================================
        private void UpdateClock()
        {
            var now = DateTime.Now;
            string line1 = now.ToString("yyyy -MM-dd", CultureInfo.InvariantCulture);
            string line2 = now.ToString("dddd", new CultureInfo("en-US"));
            string line3 = now.ToString("HH:mm", CultureInfo.InvariantCulture);
            lblClock.Text = $"{line1}{Environment.NewLine}{line2}{Environment.NewLine}{line3}";
        }

        private void lblClock_Click(object? sender, EventArgs e)
        {
            // Clock label click handler kept for future use.
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        private void StyleDataGridView(DataGridView grid)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = System.Drawing.Color.Black;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grid.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = System.Drawing.Color.Gray;
            grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            grid.RowTemplate.Height = 30;
            grid.ColumnHeadersHeight = 35;
        }

        // 빈 이벤트 핸들러 (디자이너 연결용)
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e) { }
        private void splitContainer1_Panel1_DoubleClick(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void tabPage1_Click(object sender, EventArgs e) { }
        private void tabPage1_Click_1(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }

    } // class main 끝

    // 데이터 모델
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
}
