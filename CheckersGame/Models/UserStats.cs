using System;

namespace CheckersGame.Models
{
    public class UserStats
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public int TotalGames { get; set; }
        public int WonGames { get; set; }
        public int LostGames { get; set; }
        public int DrawnGames { get; set; }
        public decimal WinRate { get; set; }
    }
}