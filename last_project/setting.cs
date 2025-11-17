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

        // --- ▼▼▼ [수정] 클래스 변수 5줄 (오류 방지) ▼▼▼ ---
        private Size largeFormSize = new Size(1043, 658);
        private Size smallFormSize = new Size(520, 584);
        private Size productTabSize = new Size(456, 581);

        // (WPF 컨트롤 3개를 클래스 변수로 선언)
        private WpfSlotEditor wpfEditor;
        private WpfSlotInfo wpfSlotInfo;
        private WpfProductAdmin wpfProductAdmin;
        private bool isManualControlLoaded = false; // (tabPage3용 '깃발')
        // --- ▲▲▲ 'isSlotEditorLoaded' 깃발은 이제 필요 없음 ▲▲▲ ---
        private WpfLogoutControl wpfLogoutControl;
        private bool isLogoutLoaded = false;
        public setting()
        {
            InitializeComponent();
        }

        private void WpfLogoutControl_LogoutClicked(object sender, EventArgs e)
        {
            LogManager.Add("로그아웃 버튼 클릭됨. 확인창 표시.");

            DialogResult result = MessageBox.Show(
                "정말 로그아웃하시겠습니까?\n프로그램이 재시작되어 로그인 화면으로 돌아갑니다.",
                "로그아웃 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                LogManager.Add("사용자가 '예'를 선택. 애플리케이션을 다시 시작합니다.");
                Application.Restart(); // 로그인 폼(second.cs)으로 돌아갑니다.
            }
            else
            {
                LogManager.Add("사용자가 '아니요'를 선택. 로그아웃 취소.");
            }
        }

        // --- ▼▼▼ [추가!] 폼이 "처음 켜질 때" 실행되는 Load 이벤트 ▼▼▼ ---
        private async void setting_Load(object sender, EventArgs e)
        {
            // --- 1. 왼쪽 패널 (WPF 카메라/그리기) 설정 ---
            ElementHost wpfHostLeft = new ElementHost();
            wpfHostLeft.Dock = DockStyle.Fill;
            wpfEditor = new WpfSlotEditor(); // (클래스 변수에 할당)
            wpfEditor.SlotDrawn += WpfEditor_SlotDrawn; // (신호 연결)
            wpfHostLeft.Child = wpfEditor;
            splitContainer1.Panel1.Controls.Add(wpfHostLeft);

            // --- 2. 오른쪽 패널 (WPF 슬롯 정보) 설정 ---
            ElementHost wpfHostRight = new ElementHost();
            wpfHostRight.Dock = DockStyle.Fill;
            wpfSlotInfo = new WpfSlotInfo(); // (클래스 변수에 할당)

            // --- ▼▼▼ [추가!] "저장" 버튼 신호 연결! ▼▼▼ ---
            wpfSlotInfo.SaveButtonClicked += async (s, ev) => await SaveSlotDataAsync();

            wpfHostRight.Child = wpfSlotInfo;
            splitContainer1.Panel2.Controls.Add(wpfHostRight);

            wpfHostRight.Dock = DockStyle.Top;
            wpfHostRight.Height = 220;
            dataGridView1.Dock = DockStyle.Fill;

            // --- (dataGridView1 스타일링 코드) ---
            var grid = dataGridView1;
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = System.Drawing.Color.FromArgb(45, 45, 48);
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
            grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(63, 63, 70); // (파란색 -> 회색)
            grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            grid.RowTemplate.Height = 30;
            grid.ColumnHeadersHeight = 35;
            splitContainer1.BackColor = System.Drawing.Color.Black;

            // --- ▼▼▼ [추가!] 폼이 켜질 때 "슬롯 목록" 로드! ▼▼▼ ---
            await LoadSlotDataAsync();
        }


        // (기존 이벤트 핸들러 - 내용은 비어있음)
        private void label4_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        // --- ▼▼▼ [수정] 탭 변경 이벤트 (WPF 로드 기능 추가) ▼▼▼ ---
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
                this.Size = productTabSize;

                // --- ▼▼▼ [핵심] tabPage2에 WPF 컨트롤 심기 ▼▼▼ ---
                if (wpfProductAdmin == null)
                {
                    ElementHost wpfHostProducts = new ElementHost();
                    wpfHostProducts.Dock = DockStyle.Top;
                    wpfHostProducts.Height = 300;

                    wpfProductAdmin = new WpfProductAdmin();

                    // (신호 연결) WPF 버튼의 '신호'를 C# '함수'와 연결
                    wpfProductAdmin.RegisterClicked += btnRegister_Click; // "신규등록"
                    wpfProductAdmin.UpdateClicked += button3_Click; // "수정"
                    wpfProductAdmin.DeleteClicked += btnDelete_Click; // "삭제"
                    wpfProductAdmin.RefreshClicked += btnRefresh_Click; // "새로고침"

                    wpfHostProducts.Child = wpfProductAdmin;
                    tabPage2.Controls.Add(wpfHostProducts);

                    // --- ▼▼▼ [추가!] dataGridView2 스타일링 코드 ▼▼▼ ---
                    var grid = dataGridView2;
                    grid.BorderStyle = BorderStyle.None;
                    grid.BackgroundColor = System.Drawing.Color.FromArgb(45, 45, 48);
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
                    grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(63, 63, 70); // (파란색 -> 회색)
                    grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
                    grid.RowTemplate.Height = 30;
                    grid.ColumnHeadersHeight = 35;
                    // --- ▲▲▲ [추가!] 여기까지 ▲▲▲ ---

                    dataGridView2.Dock = DockStyle.Fill;
                }

                await LoadProductDataAsync();
            }
            // 2번 인덱스 ("수동 제어")
            else if (tabControl1.SelectedIndex == 2)
            {
                this.Size = largeFormSize;

                // (WPF 컨트롤이 '처음' 로드되는지 확인)
                if (isManualControlLoaded == false)
                {
                    // 1. (그릇) ElementHost 생성
                    ElementHost wpfHostManual = new ElementHost();
                    wpfHostManual.Dock = DockStyle.Fill; // tabPage3 꽉 채우기

                    // 2. (내용물) "WPF 수동 제어판" 생성
                    WpfManualControl wpfControl = new WpfManualControl();

                    // 3. (조립) 그릇에 내용물을 담습니다.
                    wpfHostManual.Child = wpfControl;

                    // 4. (★★★★★) tabPage3 (세 번째 탭)에 '그릇'을 추가!
                    tabPage3.Controls.Add(wpfHostManual);

                    isManualControlLoaded = true; // "띄우기 완료" 깃발
                }
            }
            // --- ▼▼▼ [3. 추가!] 3번 인덱스 ("로그아웃") 처리 ▼▼▼ ---
            else if (tabControl1.SelectedIndex == 3)
            {
                // 로그아웃 탭은 창 크기를 작게 조절
                this.Size = smallFormSize;

                // 로그아웃 컨트롤이 아직 로드되지 않았다면
                if (isLogoutLoaded == false)
                {
                    // 1. (그릇) ElementHost 생성
                    ElementHost wpfHostLogout = new ElementHost();
                    wpfHostLogout.Dock = DockStyle.Fill; // tabPage4 꽉 채우기

                    // 2. (내용물) "WPF 로그아웃 컨트롤" 생성
                    // (WpfLogoutControl.xaml과 .cs 파일이 프로젝트에 있어야 합니다)
                    wpfLogoutControl = new WpfLogoutControl();

                    // 3. (★★★★★) "신호" 연결!
                    // WPF 컨트롤의 'LogoutClicked' 신호가 오면
                    // 'WpfLogoutControl_LogoutClicked' 함수를 실행
                    wpfLogoutControl.LogoutClicked += WpfLogoutControl_LogoutClicked;

                    // 4. (조립) 그릇에 내용물을 담습니다.
                    wpfHostLogout.Child = wpfLogoutControl;

                    // 5. tabPage4 (네 번째 탭)에 '그릇'을 추가!
                    // (디자이너에서 tabPage4가 있는지 확인하세요)
                    tabPage4.Controls.Add(wpfHostLogout);

                    isLogoutLoaded = true; // "띄우기 완료" 깃발
                }
            }
            // --- ▲▲▲ [수정!] 여기까지 ▲▲▲ ---
        }

        // --- "좌표 받기" 함수 (WpfEditor_SlotDrawn) ---
        private void WpfEditor_SlotDrawn(object sender, SlotDrawnEventArgs e)
        {
            if (wpfSlotInfo == null) return;

            int x = (int)Math.Round(e.X);
            int y = (int)Math.Round(e.Y);
            int w = (int)Math.Round(e.W);
            int h = (int)Math.Round(e.H);

            wpfSlotInfo.SlotX = x.ToString();
            wpfSlotInfo.SlotY = y.ToString();
            wpfSlotInfo.SlotW = w.ToString();
            wpfSlotInfo.SlotH = h.ToString();
            wpfSlotInfo.SlotId = "(신규 슬롯)";
            wpfSlotInfo.IsSlotActive = true;
        }


        // --- ▼▼▼ [추가!] "슬롯 목록 읽기" C# 함수 (dataGridView1용) ▼▼▼ ---
        private async Task LoadSlotDataAsync()
        {
            var targetGrid = dataGridView1; // (★★★★★) 0번 탭의 슬롯 그리드
            targetGrid.AutoGenerateColumns = false;

            string apiUrl = $"http://127.0.0.1:5000/api/slots?_t={DateTime.Now.Ticks}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // (정렬을 위해 'DataTable'로 변환)
                    DataTable slotTable = JsonConvert.DeserializeObject<DataTable>(jsonResponse);
                    targetGrid.DataSource = slotTable;
                }
                else
                {
                    MessageBox.Show($"슬롯 목록 로드 오류: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"슬롯 목록 로드 중 예외 발생: {ex.Message}");
            }
        }

        // --- ▼▼▼ [추가!] "슬롯 저장" C# 함수 (WPF '저장' 버튼용) ▼▼▼ ---
        private async Task SaveSlotDataAsync()
        {
            if (wpfSlotInfo == null) return;

            string apiUrl = "http://127.0.0.1:5000/api/slots/save";
            try
            {
                // 1. (WPF 텍스트박스에서 값 읽기)
                var slotData = new
                {
                    slot_id = wpfSlotInfo.SlotId,
                    x = Convert.ToInt32(wpfSlotInfo.SlotX),
                    y = Convert.ToInt32(wpfSlotInfo.SlotY),
                    w = Convert.ToInt32(wpfSlotInfo.SlotW),
                    h = Convert.ToInt32(wpfSlotInfo.SlotH),
                    is_active = wpfSlotInfo.IsSlotActive
                };

                if (string.IsNullOrWhiteSpace(slotData.slot_id) || slotData.slot_id == "(신규 슬롯)")
                {
                    MessageBox.Show("슬롯 아이디를 입력해주세요.\n(예: 'A-3')", "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. (JSON 변환 및 전송)
                string jsonData = JsonConvert.SerializeObject(slotData);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("슬롯이 성공적으로 저장되었습니다.");
                    // 3. (마무리) 저장이 끝나면 그리드 "새로고침"
                    await LoadSlotDataAsync();
                }
                else
                {
                    MessageBox.Show($"슬롯 저장 오류: {responseString}");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("좌표(X,Y,W,H)는 숫자만 입력해야 합니다.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"슬롯 저장 중 예외 발생: {ex.Message}");
            }
        }


        // --- (이하 "제품 품목 설정" 탭(tabPage2)의 모든 함수들) ---

        // --- 1. 데이터 "읽기" 함수 (DataTable 버전) ---
        private async Task LoadProductDataAsync()
        {
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

        // --- 2. 데이터 "수정" 함수 ---
        private async Task UpdateStockInDatabaseAsync(string itemCode, int newStock)
        {
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

        // --- 3. "재고상태" 계산 이벤트 핸들러 (삭제됨) ---
        // (private void dataGridView2_DataBindingComplete(...) 함수는 삭제했습니다)

        // (이벤트 핸들러 - 비어있음)
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView2_DataBindingComplete_1(object sender, DataGridViewBindingCompleteEventArgs e) { }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        // --- "수정" 버튼 클릭 이벤트 (button3) ---
        private async void button3_Click(object sender, EventArgs e)
        {
            var targetGrid = dataGridView2;
            string stockColumnName = "Column12";
            string itemCodeColumnName = "Column13";

            if (targetGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = targetGrid.SelectedRows[0];
                string itemCode = selectedRow.Cells[itemCodeColumnName].Value.ToString();

                try
                {
                    int newStock = Convert.ToInt32(wpfProductAdmin.Stock);
                    await UpdateStockInDatabaseAsync(itemCode, newStock);
                    await LoadProductDataAsync();
                }
                catch (FormatException) { MessageBox.Show("재고는 숫자만 입력해주세요."); }
                catch (Exception ex) { MessageBox.Show($"수정 중 오류: {ex.Message}"); }
            }
            else
            {
                MessageBox.Show("먼저 그리드에서 수정할 행을 '선택'해주세요.");
            }
        }

        // --- "새로고침" 버튼 클릭 이벤트 (btnRefresh) ---
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            if (wpfProductAdmin != null) wpfProductAdmin.ClearTextBoxes();
            await LoadProductDataAsync();
        }

        // --- "신규 등록" 전송 함수 ---
        private async Task AddNewProductAsync(string itemCode, string brand, string color, string size, string category, int stock)
        {
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

        // --- "신규등록" 버튼 클릭 이벤트 (btnRegister) ---
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (wpfProductAdmin == null) return;

            string itemCode = wpfProductAdmin.ItemCode;
            string brand = wpfProductAdmin.Brand;
            string color = wpfProductAdmin.Color;
            string size = wpfProductAdmin.ProductSize; // ('Size'가 아닌 'ProductSize')
            string category = wpfProductAdmin.Category;
            string stockText = wpfProductAdmin.Stock;

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
                wpfProductAdmin.ClearTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"신규 등록 중 오류 발생: {ex.Message}");
            }
        }

        // --- "삭제" 전송 함수 ---
        private async Task DeleteProductAsync(string itemCode)
        {
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

        // --- "삭제" 버튼 클릭 이벤트 (btnDelete) ---
        private async void btnDelete_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show("먼저 그리드에서 삭제할 행을 '선택'해주세요.");
            }
        }
    }
}