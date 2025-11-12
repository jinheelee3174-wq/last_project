using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http; // 서버 통신용
using Newtonsoft.Json; // JSON 변환용
using Microsoft.VisualBasic; // InputBox 용
using System.Windows.Forms.Integration; // <-- 1. ElementHost를 위해 추가!

namespace last_project
{
    public partial class setting : Form
    {
        // (★★★★★) 서버와 통신하기 위한 필수 객체입니다.
        private static readonly HttpClient client = new HttpClient();

        // --- ▼▼▼ [수정!] 클래스 변수 5줄 (오류 방지) ▼▼▼ ---
        private Size largeFormSize = new Size(1043, 658);
        private Size smallFormSize = new Size(473, 584);

        // (WPF 컨트롤 2개를 클래스 변수로 선언)
        // (WpfEditor_SlotDrawn 함수가 wpfSlotInfo를 참조해야 하기 때문)
        private WpfSlotEditor wpfEditor;
        private WpfSlotInfo wpfSlotInfo;
        // --- ▲▲▲ 'isSlotEditorLoaded' 깃발은 이제 필요 없음 ▲▲▲ ---

        public setting()
        {
            InitializeComponent();
        }

        // --- ▼▼▼ [추가!] 폼이 "처음 켜질 때" 실행되는 Load 이벤트 ▼▼▼ ---
        // (디자이너 [속성] > [⚡] > 'Load'를 더블 클릭해서 연결해야 합니다!)
        private void setting_Load(object sender, EventArgs e)
        {
            // (★★★★★) 
            // 폼이 켜질 때 0번 탭("슬롯 상세")의 WPF 컨트롤을 '미리' 로드합니다.

            // --- 1. 왼쪽 패널 (WPF 카메라/그리기) 설정 ---
            ElementHost wpfHostLeft = new ElementHost();
            wpfHostLeft.Dock = DockStyle.Fill;
            wpfEditor = new WpfSlotEditor(); // (클래스 변수에 할당)
            wpfEditor.SlotDrawn += WpfEditor_SlotDrawn; // (신호 연결)
            wpfHostLeft.Child = wpfEditor;
            splitContainer1.Panel1.Controls.Add(wpfHostLeft); // (Panel1에 추가)

            // --- 2. 오른쪽 패널 (WPF 슬롯 정보) 설정 ---
            ElementHost wpfHostRight = new ElementHost();
            wpfHostRight.Dock = DockStyle.Fill;
            wpfSlotInfo = new WpfSlotInfo(); // (클래스 변수에 할당)
            // (wpfSlotInfo.SaveButtonClicked += ... 나중에 "저장" 기능 연결)
            wpfHostRight.Child = wpfSlotInfo;

            // (★★★★★) 
            // Panel2(오른쪽 패널)에 'groupBox1' 대신 'wpfHostRight'를 추가!
            // (주의! dataGridView1(슬롯 목록)도 Panel2에 있어야 합니다)

            // (Panel2에서 기존 groupBox1을 찾아서 제거)
            if (this.Controls.Find("groupBox1", true).Length > 0)
            {
                Control groupBox = this.Controls.Find("groupBox1", true)[0];
                splitContainer1.Panel2.Controls.Remove(groupBox);
            }

            // (Panel2에 wpfHostRight를 추가)
            splitContainer1.Panel2.Controls.Add(wpfHostRight);

            // (Panel2의 컨트롤 순서 정리: WPF 정보창이 맨 위, 그리드가 그 아래)
            wpfHostRight.Dock = DockStyle.Top; // "슬롯 정보"는 위쪽에 붙이기
            wpfHostRight.Height = 220; // (WPF XAML에서 정한 디자인 높이)

            dataGridView1.Dock = DockStyle.Fill; // "슬롯 목록"이 남은 공간 꽉 채우기

            var grid = dataGridView1;

            // 1. (핵심) 그리드 테두리 없애기
            grid.BorderStyle = BorderStyle.None;

            // 2. 그리드 전체 배경색 (빈 공간)
            grid.BackgroundColor = System.Drawing.Color.FromArgb(45, 45, 48); // 셀 배경과 통일

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
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // 가로줄만 보이기
            grid.GridColor = System.Drawing.Color.Gray; // 셀 구분선 색상

            // 5. 셀 "선택" 스타일 설정
            grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.CornflowerBlue; // 선택 시 배경색
            grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White; // 선택 시 글자색

            // 6. (옵션) 행 높이 조절
            grid.RowTemplate.Height = 30; // 행 높이를 살짝
            grid.ColumnHeadersHeight = 35; // 헤더 높이를 살짝

            splitContainer1.BackColor = System.Drawing.Color.Black;
        }

        // (기존 이벤트 핸들러 - 내용은 비어있음)
        private void label4_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        // --- ▼▼▼ [수정] 탭 변경 이벤트 (폼 크기 조절만 남김) ▼▼▼ ---
        private async void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 0번 인덱스 ("슬롯 상세 설정")
            if (tabControl1.SelectedIndex == 0)
            {
                this.Size = largeFormSize;
            }
            // 1번 인덱스 ("제품 품목 설정")
            else if (tabControl1.SelectedIndex == 1)
            {
                this.Size = smallFormSize;
                await LoadProductDataAsync();
            }
            // 그 외 다른 탭들 ("수동 제어" - 2번 등)
            else
            {
                this.Size = largeFormSize;
            }
        }

        // --- ▼▼▼ [수정!] "좌표 받기" 함수 (WpfEditor_SlotDrawn) ▼▼▼ ---
        // (textBox2 대신, wpfSlotInfo의 속성을 채우도록 수정!)
        private void WpfEditor_SlotDrawn(object sender, SlotDrawnEventArgs e)
        {
            // (방어 코드) wpfSlotInfo(오른쪽 패널)가 로드되기 전이면 종료
            if (wpfSlotInfo == null) return;

            // 1. (WPF 좌표는 double) WinForms 텍스트박스에 맞게 정수(int)로 변환
            int x = (int)Math.Round(e.X);
            int y = (int)Math.Round(e.Y);
            int w = (int)Math.Round(e.W);
            int h = (int)Math.Round(e.H);

            // 2. (★★★★★ 핵심 수정 ★★★★★)
            // WPF "슬롯 정보" 텍스트박스에 좌표값 채우기
            wpfSlotInfo.SlotX = x.ToString(); // '좌표 X' 텍스트박스
            wpfSlotInfo.SlotY = y.ToString(); // '좌표 Y' 텍스트박스
            wpfSlotInfo.SlotW = w.ToString(); // '너비 W' 텍스트박스
            wpfSlotInfo.SlotH = h.ToString(); // '높이 H' 텍스트박스

            wpfSlotInfo.SlotId = "(신규 슬롯)";
            wpfSlotInfo.IsSlotActive = true; // '활성화'에 자동 체크
        }

        // --- (이하 "제품 품목 설정" 탭(tabPage2)의 모든 함수들) ---
        // (LoadProductDataAsync, UpdateStockInDatabaseAsync, AddNewProductAsync,
        //  DeleteProductAsync, btnRegister_Click, button3_Click, btnDelete_Click,
        //  btnRefresh_Click, dataGridView2_DataBindingComplete, dataGridView2_CellClick 등...)

        // ( ... [이전 대화]의 모든 함수 코드가 여기에 있다고 가정합니다 ... )
        private async Task LoadProductDataAsync()
        { /* ... (이전 코드와 동일) ... */
            var targetGrid = dataGridView2;
            targetGrid.AutoGenerateColumns = false;
            string apiUrl = $"http://127.0.0.1:5000/api/products?_t={DateTime.Now.Ticks}";
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
        private async Task UpdateStockInDatabaseAsync(string itemCode, int newStock)
        { /* ... (이전 코드와 동일) ... */
            string apiUrl = "http://127.0.0.1:5000/api/product/update_stock";
            try
            {
                var updateData = new { item_code = itemCode, new_stock = newStock };
                string jsonData = JsonConvert.SerializeObject(updateData);
                StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"'{itemCode}'의 재고가 {newStock}개로 수정되었습니다.");
                }
                else
                {
                    string errorJson = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"서버 수정 오류: {errorJson}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"재고 수정 중 예외 발생: {ex.Message}");
            }
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e) { /* ... (이전 코드와 동일) ... */ }
        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        { /* ... (이전 코드와 동일) ... */
            try
            {
                string stockStatusColumnName = "Column1";
                string stockValueColumnName = "Column12";
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells[stockValueColumnName].Value != null)
                    {
                        int stock = Convert.ToInt32(row.Cells[stockValueColumnName].Value);
                        DataGridViewCell statusCell = row.Cells[stockStatusColumnName];
                        if (stock <= 2)
                        {
                            statusCell.Value = "위험";
                            statusCell.Style.ForeColor = System.Drawing.Color.Red;
                            statusCell.Style.Font = new Font(dataGridView2.Font, FontStyle.Bold);
                        }
                        else if (stock == 3)
                        {
                            statusCell.Value = "주의";
                            statusCell.Style.ForeColor = System.Drawing.Color.Orange;
                            statusCell.Style.Font = new Font(dataGridView2.Font, FontStyle.Regular);
                        }
                        else
                        {
                            statusCell.Value = "정상";
                            statusCell.Style.ForeColor = System.Drawing.Color.Green;
                            statusCell.Style.Font = new Font(dataGridView2.Font, FontStyle.Regular);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"재고 상태 업데이트 중 오류: {ex.Message}"); }
        }
        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView2_DataBindingComplete_1(object sender, DataGridViewBindingCompleteEventArgs e) { }
        private async void button3_Click(object sender, EventArgs e)
        { /* ... (이전 코드와 동일) ... */
            var targetGrid = dataGridView2;
            string stockColumnName = "Column12";
            string itemCodeColumnName = "Column13";
            if (targetGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = targetGrid.SelectedRows[0];
                string itemCode = selectedRow.Cells[itemCodeColumnName].Value.ToString();
                string currentStock = selectedRow.Cells[stockColumnName].Value.ToString();
                string prompt = $"'{itemCode}'의 재고를 수정합니다.\n\n현재 재고: {currentStock}";
                string title = "재고 수정";
                string newStockString = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, currentStock);
                if (!string.IsNullOrWhiteSpace(newStockString))
                {
                    try
                    {
                        int newStock = Convert.ToInt32(newStockString);
                        await UpdateStockInDatabaseAsync(itemCode, newStock);
                        await LoadProductDataAsync();
                    }
                    catch (FormatException) { MessageBox.Show("숫자만 입력해주세요."); }
                    catch (Exception ex) { MessageBox.Show($"수정 중 오류: {ex.Message}"); }
                }
            }
            else
            {
                MessageBox.Show("먼저 그리드에서 수정할 행을 '선택'해주세요.\n(행의 맨 앞 회색 칸을 클릭하세요)");
            }
        }
        private async void btnRefresh_Click(object sender, EventArgs e)
        { /* ... (이전 코드와 동일) ... */
            await LoadProductDataAsync();
        }
        private async Task AddNewProductAsync(string itemCode, string brand, string color, string size, string category, int stock)
        { /* ... (이전 코드와 동일) ... */
            string apiUrl = "http://127.0.0.1:5000/api/product/add";
            try
            {
                var newProductData = new { item_code = itemCode, brand = brand, color = color, size = size, category = category, stock = stock };
                string jsonData = JsonConvert.SerializeObject(newProductData);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("신규 제품이 성공적으로 등록되었습니다.");
                }
                else
                {
                    MessageBox.Show($"등록 오류: {responseString}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"신규 등록 중 예외 발생: {ex.Message}");
            }
        }
        private async void btnRegister_Click(object sender, EventArgs e)
        { /* ... (이전 코드와 동일) ... */
            string itemCode = txtItemCode.Text;
            string brand = txtBrand.Text;
            string color = txtColor.Text;
            string size = txtSize.Text;
            string category = txtCategory.Text;
            string stockText = txtStock.Text;
            if (string.IsNullOrWhiteSpace(itemCode))
            {
                MessageBox.Show("품목번호는 필수 입력 항목입니다.");
                return;
            }
            int stock = 0;
            if (!string.IsNullOrWhiteSpace(stockText) && !int.TryParse(stockText, out stock))
            {
                MessageBox.Show("재고는 숫자만 입력해주세요.");
                return;
            }
            try
            {
                await AddNewProductAsync(itemCode, brand, color, size, category, stock);
                await LoadProductDataAsync();
                txtItemCode.Text = "";
                txtBrand.Text = "";
                txtColor.Text = "";
                txtSize.Text = "";
                txtCategory.Text = "";
                txtStock.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"신규 등록 중 오류 발생: {ex.Message}");
            }
        }
        private async Task DeleteProductAsync(string itemCode)
        { /* ... (이전 코드와 동일) ... */
            string apiUrl = "http://127.0.0.1:5000/api/product/delete";
            try
            {
                var deleteData = new { item_code = itemCode };
                string jsonData = JsonConvert.SerializeObject(deleteData);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("제품이 성공적으로 삭제되었습니다.");
                }
                else
                {
                    MessageBox.Show($"삭제 오류: {responseString}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"삭제 중 예외 발생: {ex.Message}");
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private async void btnDelete_Click(object sender, EventArgs e)
        { /* ... (이전 코드와 동일) ... */
            var targetGrid = dataGridView2;
            string itemCodeColumnName = "Column13";
            if (targetGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = targetGrid.SelectedRows[0];
                string itemCode = selectedRow.Cells[itemCodeColumnName].Value.ToString();
                DialogResult result = MessageBox.Show(
                    $"정말 '{itemCode}' 제품을 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다.",
                    "삭제 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await DeleteProductAsync(itemCode);
                        await LoadProductDataAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"삭제 중 오류 발생: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("먼저 그리드에서 삭제할 행을 '선택'해주세요.\n(행의 맨 앞 회색 칸을 클릭하세요)");
            }
        }
    }
}