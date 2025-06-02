using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame.Models
{
    public class Game
    {
        public int GameID { get; set; }
        public int Player1ID { get; set; }
        public int? Player2ID { get; set; }
        public int DifficultyLevel { get; set; }
        public string GameMode { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string GameStatus { get; set; }
        public int? WinnerID { get; set; }
        public string GameResult { get; set; }
    }
}
