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

        public main()
        {
            InitializeComponent();
            splitContainer1.Panel1.BackColor = System.Drawing.Color.Gray;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

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
        }

        // ▼▼▼ 새로 추가된 카메라 초기화 함수 ▼▼▼
        private async Task InitializeCameraWebViewAsync()
        {
            try
            {
                // 디자이너에서 추가한 'webViewCam1' 컨트롤을 초기화합니다.
                await webViewCam1.EnsureCoreWebView2Async(null);

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

        private async void button6_Click(object sender, EventArgs e)
        {
            await LoadProductDataAsync();
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