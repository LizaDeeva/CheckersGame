using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame.Models
{
    public class GameStats
    {
        public int StatID { get; set; }
        public int UserID { get; set; }
        public int TotalGames { get; set; }
        public int WonGames { get; set; }
        public int LostGames { get; set; }
        public int DrawnGames { get; set; }
        public decimal WinRate { get; set; }
        public string Username { get; set; }
    }
}