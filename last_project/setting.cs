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
// (참고: System.Collections.Generic, System.Threading.Tasks, System.Text는 이미 포함되어 있습니다)
using Microsoft.VisualBasic; // InputBox 용



namespace last_project
{
    public partial class setting : Form
    {
        // (★★★★★) 서버와 통신하기 위한 필수 객체입니다.
        private static readonly HttpClient client = new HttpClient();

        public setting()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private Size largeFormSize = new Size(1043, 658);
        private Size smallFormSize = new Size(473, 584);

        // ▼▼▼ [수정] 'async'를 추가해서 'await LoadProductDataAsync()'를 호출할 수 있게 합니다 ▼▼▼
        private async void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1번 인덱스("제품 품목 설정")가 선택되었는지 확인
            if (tabControl1.SelectedIndex == 1)
            {
                // 폼의 크기를 '작은' 크기로 변경합니다.
                this.Size = smallFormSize;

                // ▼▼▼ [추가] 탭이 선택될 때마다 데이터를 "새로고침"합니다 ▼▼▼
                await LoadProductDataAsync();
            }
            // 그 외 다른 탭들 ("슬롯 상세 설정" - 0번, "수동 제어" - 2번 등)
            else
            {
                // 폼의 크기를 '큰' 크기로 다시 되돌립니다.
                this.Size = largeFormSize;
            }
        }

        // --- ▼▼▼ [추가] 1. 데이터 "읽기" 함수 (DataTable 버전) ▼▼▼ ---
        private async Task LoadProductDataAsync()
        {
            // (★★★★★) setting.cs의 DataGridView 이름
            var targetGrid = dataGridView2;

            // 컬럼 자동 생성을 끕니다.
            targetGrid.AutoGenerateColumns = false;

            // (캐시 문제를 피하기 위해 매번 다른 주소로 요청)
            string apiUrl = $"http://127.0.0.1:5000/api/products?_t={DateTime.Now.Ticks}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // ▼▼▼ [핵심 수정!] ▼▼▼
                    // List/BindingList 대신, 정렬이 완벽하게 작동하는 'DataTable'로 변환합니다.
                    DataTable productTable = JsonConvert.DeserializeObject<DataTable>(jsonResponse);

                    // (참고: DataTable을 쓸 때는 'default'가 필요 없습니다)
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
        // --- ▼▼▼ [추가] 2. 데이터 "수정" 함수 ▼▼▼ ---
        private async Task UpdateStockInDatabaseAsync(string itemCode, int newStock)
        {
            // app.py에 만든 수정 API 주소
            string apiUrl = "http://127.0.0.1:5000/api/product/update_stock";
            try
            {
                // 1. 서버(Flask)로 보낼 데이터를 C# 객체로 만듭니다.
                var updateData = new
                {
                    item_code = itemCode,
                    new_stock = newStock
                };

                // 2. C# 객체를 JSON 문자열로 변환합니다.
                string jsonData = JsonConvert.SerializeObject(updateData);

                // 3. JSON 데이터를 HTTP 요청 본문(Payload)으로 만듭니다.
                StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                // 4. (중요) 서버에 "POST" 방식으로 JSON 데이터를 전송합니다.
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // 5. 서버의 응답 확인
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

        // --- ▼▼▼ [추가] 3. "재고" 셀 클릭 이벤트 핸들러 ▼▼▼ ---
        // (★★★★★ 이 함수를 디자이너의 'CellClick' 이벤트와 연결해야 합니다!)
        private async void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // --- ▼▼▼ [추가] 4. "재고상태" 계산 이벤트 핸들러 ▼▼▼ ---
        // (★★★★★ 이 함수를 디자이너의 'DataBindingComplete' 이벤트와 연결해야 합니다!)
        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                // (★★★★★) "재고상태" 컬럼의 (Name) 속성
                string stockStatusColumnName = "Column1"; // <-- 디자이너에서 확인한 실제 이름
                // (★★★★★) "재고" 컬럼의 (Name) 속성
                string stockValueColumnName = "Column7"; // <-- 디자이너에서 확인한 실제 이름

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells[stockValueColumnName].Value != null)
                    {
                        int stock = Convert.ToInt32(row.Cells[stockValueColumnName].Value);
                        DataGridViewCell statusCell = row.Cells[stockStatusColumnName];

                        // 요청하신 새 기준으로 "재고상태" 컬럼 채우기
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
                        else // 4개 이상
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

        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_DataBindingComplete_1(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            // (★★★★★) setting.cs의 DataGridView 이름
            var targetGrid = dataGridView2;

            // ▼▼▼ 1. "재고" 컬럼 (Name)을 알려주신 'Column12'로 수정 ▼▼▼
            string stockColumnName = "Column12";

            // ▼▼▼ 2. "품목번호" 컬럼 (Name)을 알려주신 'Column8'로 수정 ▼▼▼
            string itemCodeColumnName = "Column13";

            // 1. 현재 "선택된 행"이 있는지 확인합니다.
            if (targetGrid.SelectedRows.Count > 0)
            {
                // 2. 선택된 '첫 번째' 행을 가져옵니다.
                DataGridViewRow selectedRow = targetGrid.SelectedRows[0];

                // 3. 현재 값들 가져오기
                string itemCode = selectedRow.Cells[itemCodeColumnName].Value.ToString();
                string currentStock = selectedRow.Cells[stockColumnName].Value.ToString();

                // 4. (요청하신) 팝업창 띄우기
                string prompt = $"'{itemCode}'의 재고를 수정합니다.\n\n현재 재고: {currentStock}";
                string title = "재고 수정";

                string newStockString = Microsoft.VisualBasic.Interaction.InputBox(prompt, title, currentStock);

                // 5. 사용자가 "확인"을 눌렀고, 값을 입력했는지 확인
                if (!string.IsNullOrWhiteSpace(newStockString))
                {
                    try
                    {
                        // 6. 입력받은 값을 숫자로 변환
                        int newStock = Convert.ToInt32(newStockString);

                        // 7. (핵심) DB 업데이트 함수 호출! (이미 만들어 둔 함수)
                        await UpdateStockInDatabaseAsync(itemCode, newStock);

                        // 8. (마무리) 그리드 새로고침
                        await LoadProductDataAsync();
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("숫자만 입력해주세요.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"수정 중 오류: {ex.Message}");
                    }
                }
            }
            else
            {
                // 9. 아무 행도 선택하지 않았을 때
                MessageBox.Show("먼저 그리드에서 수정할 행을 '선택'해주세요.\n(행의 맨 앞 회색 칸을 클릭하세요)");
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadProductDataAsync();
        }
        private async Task AddNewProductAsync(string itemCode, string brand, string color, string size, string category, int stock)
        {
            // 1. 방금 app.py에 만든 새 API 주소
            string apiUrl = "http://127.0.0.1:5000/api/product/add";
            try
            {
                // 2. 서버(Flask)로 보낼 데이터를 C# 객체로 만듭니다.
                // (product_name을 보내지 않습니다. 서버가 알아서 만듭니다.)
                var newProductData = new
                {
                    item_code = itemCode,
                    brand = brand,
                    color = color,
                    size = size,
                    category = category,
                    stock = stock
                };

                // 3. C# 객체를 JSON 문자열로 변환합니다.
                string jsonData = JsonConvert.SerializeObject(newProductData);

                // 4. JSON 데이터를 HTTP 요청 본문(Payload)으로 만듭니다.
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // 5. (중요) 서버에 "POST" 방식으로 JSON 데이터를 전송합니다.
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // 6. 서버가 보낸 응답 메시지를 문자열로 읽습니다.
                string responseString = await response.Content.ReadAsStringAsync();

                // 7. 응답이 성공(Success)인지, 아니면 (중복 오류 같은) 실패인지 확인
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("신규 제품이 성공적으로 등록되었습니다.");
                }
                else
                {
                    // 서버가 보낸 JSON 오류 메시지 (예: "품목번호 중복")
                    MessageBox.Show($"등록 오류: {responseString}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"신규 등록 중 예외 발생: {ex.Message}");
            }
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            // (★★★★★ 1. 텍스트박스의 (Name) 속성을 확인하고 여기를 고쳐주세요!)
            // (예: txtItemCode, txtBrand, txtColor, txtSize, txtCategory, txtStock)
            string itemCode = txtItemCode.Text;     // '품목번호' 텍스트박스 (Name)
            string brand = txtBrand.Text;         // '브랜드' 텍스트박스 (Name)
            string color = txtColor.Text;         // '색상' 텍스트박스 (Name)
            string size = txtSize.Text;           // '사이즈' 텍스트박스 (Name)
            string category = txtCategory.Text;     // '카테고리' 텍스트박스 (Name)
            string stockText = txtStock.Text;       // '재고' 텍스트박스 (Name)

            // 2. (필수) 빈 칸이 있는지 확인 (품목번호는 필수!)
            if (string.IsNullOrWhiteSpace(itemCode))
            {
                MessageBox.Show("품목번호는 필수 입력 항목입니다.");
                return;
            }

            // 3. (필수) 재고를 숫자로 변환 (비어있으면 0)
            int stock = 0;
            if (!string.IsNullOrWhiteSpace(stockText) && !int.TryParse(stockText, out stock))
            {
                MessageBox.Show("재고는 숫자만 입력해주세요.");
                return; // 숫자 변환 실패 시 중단
            }

            try
            {
                // 4. [2단계]에서 만든 '전송 함수' 호출
                await AddNewProductAsync(itemCode, brand, color, size, category, stock);

                // 5. [마무리] 그리드 새로고침
                await LoadProductDataAsync();

                // 6. (옵션) 텍스트박스 비우기
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
        {
            // 1. 방금 app.py에 만든 새 API 주소
            string apiUrl = "http://127.0.0.1:5000/api/product/delete";
            try
            {
                // 2. 서버(Flask)로 보낼 데이터를 C# 객체로 만듭니다.
                var deleteData = new
                {
                    item_code = itemCode
                };

                // 3. C# 객체를 JSON 문자열로 변환합니다.
                string jsonData = JsonConvert.SerializeObject(deleteData);

                // 4. JSON 데이터를 HTTP 요청 본문(Payload)으로 만듭니다.
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // 5. (중요) 서버에 "POST" 방식으로 JSON 데이터를 전송합니다.
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // 6. 서버가 보낸 응답 메시지를 문자열로 읽습니다.
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

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var targetGrid = dataGridView2;

            // (★★★★★)
            // 디자이너 "열 편집"에서 확인한 "품목번호" 컬럼의 (Name) 속성
            string itemCodeColumnName = "Column13"; // (예: "Column8" 또는 "Column2" 등)

            // 1. 현재 "선택된 행"이 있는지 확인합니다.
            if (targetGrid.SelectedRows.Count > 0)
            {
                // 2. 선택된 '첫 번째' 행을 가져옵니다.
                DataGridViewRow selectedRow = targetGrid.SelectedRows[0];
                string itemCode = selectedRow.Cells[itemCodeColumnName].Value.ToString();

                // 3. (중요!) 사용자에게 정말 삭제할 것인지 "확인"받기
                DialogResult result = MessageBox.Show(
                    $"정말 '{itemCode}' 제품을 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다.",
                    "삭제 확인",
                    MessageBoxButtons.YesNo, // "예", "아니오" 버튼
                    MessageBoxIcon.Warning  // 경고 아이콘
                );

                // 4. 사용자가 "예(Yes)"를 눌렀을 때만 삭제 실행
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // 5. [STEP 2]에서 만든 '전송 함수' 호출
                        await DeleteProductAsync(itemCode);

                        // 6. [마무리] 그리드 새로고침
                        await LoadProductDataAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"삭제 중 오류 발생: {ex.Message}");
                    }
                }
                // ( "아니오(No)"를 누르면 아무것도 하지 않고 조용히 종료 )
            }
            else
            {
                // 7. 아무 행도 선택하지 않았을 때
                MessageBox.Show("먼저 그리드에서 삭제할 행을 '선택'해주세요.\n(행의 맨 앞 회색 칸을 클릭하세요)");
            }
        }
    }



}