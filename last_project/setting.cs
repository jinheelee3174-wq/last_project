using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Windows.Forms.Integration; // (필수) ElementHost

namespace last_project
{
    public partial class setting : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private Size largeFormSize = new Size(1043, 658);
        private Size smallFormSize = new Size(473, 584);

        // --- ▼▼▼ [수정!] WPF 컨트롤 3개를 '클래스 변수'로 선언 ▼▼▼ ---
        private WpfSlotEditor wpfEditor;
        private WpfSlotInfo wpfSlotInfo;
        private WpfProductAdmin wpfProductAdmin; // (★★★★★) tabPage2용 컨트롤
        // --- ▲▲▲ 'isSlotEditorLoaded' 깃발은 이제 필요 없음 ▲▲▲ ---

        public setting()
        {
            InitializeComponent();
        }

        // --- ▼▼▼ 폼이 "처음 켜질 때" 실행되는 Load 이벤트 ▼▼▼ ---
        private void setting_Load(object sender, EventArgs e)
        {
            // --- 1. 왼쪽 패널 (WPF 카메라/그리기) 설정 ---
            ElementHost wpfHostLeft = new ElementHost();
            wpfHostLeft.Dock = DockStyle.Fill;
            wpfEditor = new WpfSlotEditor();
            wpfEditor.SlotDrawn += WpfEditor_SlotDrawn;
            wpfHostLeft.Child = wpfEditor;
            splitContainer1.Panel1.Controls.Add(wpfHostLeft);

            // --- 2. 오른쪽 패널 (WPF 슬롯 정보) 설정 ---
            ElementHost wpfHostRight = new ElementHost();
            wpfHostRight.Dock = DockStyle.Fill;
            wpfSlotInfo = new WpfSlotInfo();
            // (wpfSlotInfo.SaveButtonClicked += ... 나중에 "저장" 기능 연결)
            wpfHostRight.Child = wpfSlotInfo;

            // (Panel2에 wpfHostRight를 추가)
            splitContainer1.Panel2.Controls.Add(wpfHostRight);

            wpfHostRight.Dock = DockStyle.Top;
            wpfHostRight.Height = 220;
            dataGridView1.Dock = DockStyle.Fill;
            // (★★★★★) 그리드 컨트롤 이름이 'dataGridView1'이 맞는지 확인!
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
        }

        // (기존 이벤트 핸들러 - 내용은 비어있음)
        private void label4_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        // --- ▼▼▼ [수정] 탭 변경 이벤트 (tabPage2 로드 기능 추가) ▼▼▼ ---
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

                // --- ▼▼▼ [핵심] tabPage2에 WPF 컨트롤 심기 ▼▼▼ ---

                // (WPF 컨트롤이 '처음' 로드되는지 확인하는 '깃발')
                if (wpfProductAdmin == null)
                {
                    // 1. (그릇) WPF를 담을 ElementHost 생성
                    ElementHost wpfHostProducts = new ElementHost();
                    wpfHostProducts.Dock = DockStyle.Top; // tabPage2의 '위쪽'에 붙이기
                    wpfHostProducts.Height = 300; // (XAML 디자인 높이)

                    // 2. (내용물) "WPF 제품 관리" 생성
                    wpfProductAdmin = new WpfProductAdmin(); // (클래스 변수에 할당)

                    // 3. (신호 연결) WPF 버튼의 '신호'를 C# '함수'와 연결
                    wpfProductAdmin.RegisterClicked += btnRegister_Click; // "신규등록"
                    wpfProductAdmin.UpdateClicked += button3_Click; // "수정"
                    wpfProductAdmin.DeleteClicked += btnDelete_Click; // "삭제"
                    wpfProductAdmin.RefreshClicked += btnRefresh_Click; // "새로고침"

                    // 4. (조립) 그릇에 내용물을 담습니다.
                    wpfHostProducts.Child = wpfProductAdmin;

                    // 5. (★★★★★) 
                    // tabPage2 (두 번째 탭)에 '그릇'을 추가합니다!
                    tabPage2.Controls.Add(wpfHostProducts);

                    // (옵션) dataGridView2를 '아래쪽' 꽉 채우기
                    dataGridView2.Dock = DockStyle.Fill;
                }

                // 1번 탭을 누를 때마다 "제품 목록" 새로고침
                await LoadProductDataAsync();
            }
            // 그 외 다른 탭들 ("수동 제어" - 2번 등)
            else
            {
                this.Size = largeFormSize;
            }
            // (★★★★★) 그리드 컨트롤 이름이 'dataGridView2'가 맞는지 확인!
            var grid = dataGridView2;

            // 1. (핵심) 그리드 테두리 없애기
            grid.BorderStyle = BorderStyle.None;

            // 2. 그리드 전체 배경색 (빈 공간)
            grid.BackgroundColor = System.Drawing.Color.FromArgb(45, 45, 48); // 셀 배경과 통일

            // 3. 헤더(제목) 스타일 설정
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;

            // 4. 셀(칸) 스타일 설정
            grid.RowHeadersVisible = false;
            grid.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grid.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = System.Drawing.Color.Gray;

            // 5. 셀 "선택" 스타일 설정
            grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;

            // 6. (옵션) 행 높이 조절
            grid.RowTemplate.Height = 30;
            grid.ColumnHeadersHeight = 35;
        }

        // --- ▼▼▼ "좌표 받기" 함수 (WpfEditor_SlotDrawn) ▼▼▼ ---
        private void WpfEditor_SlotDrawn(object sender, SlotDrawnEventArgs e)
        {
            if (wpfSlotInfo == null) return;

            int x = (int)Math.Round(e.X);
            int y = (int)Math.Round(e.Y);
            int w = (int)Math.Round(e.W);
            int h = (int)Math.Round(e.H);

            // (WPF "슬롯 정보" 텍스트박스에 좌표값 채우기)
            wpfSlotInfo.SlotX = x.ToString();
            wpfSlotInfo.SlotY = y.ToString();
            wpfSlotInfo.SlotW = w.ToString();
            wpfSlotInfo.SlotH = h.ToString();
            wpfSlotInfo.SlotId = "(신규 슬롯)";
            wpfSlotInfo.IsSlotActive = true;
        }

        // --- (이하 "제품 품목 설정" 탭(tabPage2)의 모든 함수들) ---
        // (★★★★★ 코드 내용은 동일하지만, 텍스트박스 읽는 부분만 수정됨 ★★★★★)

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

      

        // (이벤트 핸들러 - 비어있음)
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }


        // --- ▼▼▼ [수정!] "수정" 버튼 클릭 이벤트 (WPF가 호출) ▼▼▼ ---
        private async void button3_Click(object sender, EventArgs e)
        {
            var targetGrid = dataGridView2;
            string stockColumnName = "Column12";
            string itemCodeColumnName = "Column13";

            if (targetGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = targetGrid.SelectedRows[0];
                string itemCode = selectedRow.Cells[itemCodeColumnName].Value.ToString();

                // (★★★★★)
                // '수정'은 팝업창을 띄우는 게 아니라, 
                // "WPF 텍스트박스"의 값을 읽어와야 합니다!
                try
                {
                    int newStock = Convert.ToInt32(wpfProductAdmin.Stock); // WPF 텍스트박스에서 읽기!
                    await UpdateStockInDatabaseAsync(itemCode, newStock);
                    await LoadProductDataAsync(); // 새로고침
                }
                catch (FormatException) { MessageBox.Show("재고는 숫자만 입력해주세요."); }
                catch (Exception ex) { MessageBox.Show($"수정 중 오류: {ex.Message}"); }
            }
            else
            {
                MessageBox.Show("먼저 그리드에서 수정할 행을 '선택'해주세요.");
            }
        }

        // --- ▼▼▼ [수정!] "새로고침" 버튼 클릭 이벤트 (WPF가 호출) ▼▼▼ ---
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            // (WPF 텍스트박스 비우기)
            if (wpfProductAdmin != null) wpfProductAdmin.ClearTextBoxes();

            await LoadProductDataAsync();
        }

        // --- "신규 등록" 전송 함수 (수정 없음) ---
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

        // --- ▼▼▼ [수정!] "신규등록" 버튼 클릭 이벤트 (WPF가 호출) ▼▼▼ ---
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            // (방어 코드)
            if (wpfProductAdmin == null) return;

            // (★★★★★ 1. '삭제된' WinForms 텍스트박스 대신, 'WPF' 속성에서 값을 읽어옵니다!)
            string itemCode = wpfProductAdmin.ItemCode;
            string brand = wpfProductAdmin.Brand;
            string color = wpfProductAdmin.Color;
            string size = wpfProductAdmin.ProductSize;
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
                await LoadProductDataAsync(); // 새로고침
                wpfProductAdmin.ClearTextBoxes(); // 텍스트박스 비우기
            }
            catch (Exception ex)
            {
                MessageBox.Show($"신규 등록 중 오류 발생: {ex.Message}");
            }
        }

        // --- "삭제" 전송 함수 (수정 없음) ---
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

        // --- "삭제" 버튼 클릭 이벤트 (WPF가 호출) ---
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
                        await LoadProductDataAsync(); // 새로고침
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
