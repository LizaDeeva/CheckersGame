using System;

namespace CheckersGame.Models
{
    public class GameMove
    {
        public int MoveID { get; set; }
        public int GameID { get; set; }
        public int PlayerID { get; set; }
        public int MoveNumber { get; set; }
        public string FromPosition { get; set; }
        public string ToPosition { get; set; }
        public DateTime MoveTime { get; set; }
        public bool IsCapture { get; set; }
        public string CapturedPiece { get; set; }
    }
}