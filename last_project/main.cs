using System.Globalization;
// Flask 서버의 JSON 데이터와 1:1로 대응되는 C# 클래스
// (기존 using 문들...)
using System.Net.Http;          // <- HTTP 통신용
using Newtonsoft.Json;          // <- JSON 변환용
using System.Collections.Generic; // <- List<T> 사용용
using System.Threading.Tasks;   // <- 비동기(async/await) Task 사용용


namespace last_project
{

    public partial class main : Form
    {
        private static readonly HttpClient client = new HttpClient();
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

        }

        // ▼▼▼ 수정 1: "새로고침" 버튼 기능 추가 ▼▼▼
        // (이 버튼이 'button2'가 맞다고 가정합니다)
        private async void button2_Click(object sender, EventArgs e)
        {
            // 데이터 로드 함수 호출
            await LoadProductDataAsync();
        }

        private void lblClock_Click(object sender, EventArgs e)
        {

        }


        private void UpdateClock()
        {
            var now = DateTime.Now;

            // 1줄: "yyyy -MM-dd"
            string line1 = now.ToString("yyyy -MM-dd", CultureInfo.InvariantCulture);

            // 2줄: 영어 요일
            string line2 = now.ToString("dddd", new CultureInfo("en-US"));

            // 3줄: 시간(24시간제). 12시간제로 원하면 "tt h:mm"으로 바꿔.
            string line3 = now.ToString("HH:mm", CultureInfo.InvariantCulture);

            lblClock.Text = $"{line1}{Environment.NewLine}{line2}{Environment.NewLine}{line3}";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        // ▼▼▼ 수정 2: 'Form1_Load' -> 'main_Load'로 이름 변경 및 폼 로드 시 데이터 호출 ▼▼▼
        private async void main_Load(object sender, EventArgs e)
        {
            // 라벨이 왼쪽 패널 전체 너비를 차지하도록
            lblClock.AutoSize = false;
            lblClock.Dock = DockStyle.Top;          // ← 패널의 상단에 가로로 꽉 차게
            lblClock.Height = 80;                      // 필요하면 70~100 사이로 조정
            lblClock.TextAlign = ContentAlignment.MiddleCenter; // ← 가로/세로 가운데 정렬
            lblClock.Padding = new Padding(0, 6, 0, 0);     // 살짝 내려오게(옵션)
            lblClock.Font = new Font("Segoe UI", 11f, FontStyle.Regular);

            timer1.Interval = 1000;
            timer1.Start();
            UpdateClock();

            dataGridView1.AutoGenerateColumns = false;

            // ▼▼▼ 수정 3: 디자이너가 멈추지 않도록 DesignMode 체크 추가 ▼▼▼
            // (폼이 켜질 때 딱 한 번 데이터를 불러옵니다)
            if (!this.DesignMode)
            {
                await LoadProductDataAsync();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer1_Panel1_DoubleClick(object sender, EventArgs e)
        {
            int step = 60; // 아주 조금 (10~16 픽셀 정도 추천)
            int min = splitContainer1.Panel1MinSize;
            int max = splitContainer1.Width - splitContainer1.Panel2MinSize;

            int next = splitContainer1.SplitterDistance + step; // ← ‘조금 늘리기’
            if (next > max) next = max;                       // 범위 보정

            splitContainer1.SplitterDistance = next;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage1_Click_1(object sender, EventArgs e)
        {

        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            // 1. setting 폼의 새 인턴스(객체)를 만듭니다.
            setting settingForm = new setting();

            // 2. 폼을 'Modal'로 띄웁니다.
            // (이 창이 닫히기 전까지 main 폼을 클릭할 수 없습니다.)
            settingForm.ShowDialog();
        }
        private async Task LoadProductDataAsync()
        {
            // targetGrid의 이름이 'dataGridView1'이 맞는 것 같네요!
            var targetGrid = dataGridView1;

            string apiUrl = "http://127.0.0.1:5000/api/products";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("--- 서버에서 받은 JSON ---");
                    System.Diagnostics.Debug.WriteLine(jsonResponse);
                    List<Product> productList = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);

                    targetGrid.DataSource = productList;

                    // (데이터 로드가 성공하면 메시지 박스는 굳이 안 띄우는 게 더 좋습니다)
                    // MessageBox.Show("데이터를 성공적으로 새로고침했습니다.");
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
                // (디자이너 확인) "재고상태" 컬럼의 (Name) 속성이 "Column1"이 맞는지 확인
                string stockStatusColumnName = "Column1";

                // (디자이너 확인) "재고" 컬럼의 (Name) 속성이 "Column7"이 맞는지 확인
                string stockValueColumnName = "Column7";

                // 그리드의 모든 행(Row)을 하나씩 반복합니다.
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // 재고 셀(Column7)의 값을 가져옵니다.
                    if (row.Cells[stockValueColumnName].Value != null)
                    {
                        // 값을 정수(int)로 변환
                        int stock = Convert.ToInt32(row.Cells[stockValueColumnName].Value);

                        // 재고상태 셀(Column1)을 가져옵니다.
                        DataGridViewCell statusCell = row.Cells[stockStatusColumnName];

                        // --- 요청하신 새 기준으로 "재고상태" 컬럼 채우기 ---
                        if (stock <= 2) // 2개 이하 (위험)
                        {
                            statusCell.Value = "위험";
                            statusCell.Style.ForeColor = System.Drawing.Color.Red;
                            // (옵션) 위험 상태만 굵게 표시
                            statusCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                        }
                        else if (stock == 3) // 딱 3개 (주의)
                        {
                            statusCell.Value = "주의";
                            statusCell.Style.ForeColor = System.Drawing.Color.Orange;
                            // (옵션) 폰트 스타일을 원래대로 (굵게 아님)
                            statusCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                        }
                        else // 4개 이상 (정상)
                        {
                            statusCell.Value = "정상";
                            statusCell.Style.ForeColor = System.Drawing.Color.Green;
                            // (옵션) 폰트 스타일을 원래대로
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
    }
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