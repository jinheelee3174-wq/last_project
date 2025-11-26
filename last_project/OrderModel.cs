using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace last_project
{
    // 데이터 변경 시 UI에 즉시 반영하기 위해 INotifyPropertyChanged 구현
    public class OrderModel : INotifyPropertyChanged
    {
        private string _status;

        public int Id { get; set; }             // DB의 고유 ID
        public string Company { get; set; }     // 발주처 (기업명)
        public string ItemName { get; set; }    // 품목명
        public int Quantity { get; set; }       // 수량
        public string OrderDate { get; set; }   // 발주 일자 (String으로 관리하는 게 편함)
        public string DueDate { get; set; }     // 납기일
        public string Contact { get; set; }     // 담당자 연락처
        public int Price { get; set; }          // 단가
        public string Note { get; set; }        // 비고

        // 총액 (단가 * 수량) - 읽기 전용 속성
        public int TotalPrice => Price * Quantity;

        // 진행 상태 (값이 바뀌면 UI 색상이 변하게 하려고 알림 설정)
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}