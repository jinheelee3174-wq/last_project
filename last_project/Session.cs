using System;

namespace last_project
{
    public static class Session
    {
        // 기본 정보
        public static string UserId { get; set; } = ""; // ★ 여기가 UserId 여야 합니다
        public static string UserName { get; set; } = "";
        public static string Nickname { get; set; } = "";
        public static string Role { get; set; } = "STAFF";

        // 상세 정보
        public static string Email { get; set; } = "";
        public static string Phone { get; set; } = "";
        public static string Birthdate { get; set; } = "";

        // 시스템 정보
        public static DateTime LoginTime { get; set; } = DateTime.Now;
        public static string ProfileImagePath { get; set; } = "";

        // 데이터 세팅 함수
        public static void SetUser(string id, string name, string nickname, string role, string email, string phone, string birthdate)
        {
            UserId = id;
            UserName = name;
            Nickname = nickname;
            Role = role;
            Email = email;
            Phone = phone;
            Birthdate = birthdate;
            LoginTime = DateTime.Now;
        }
    }
}