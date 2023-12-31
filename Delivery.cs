using System;
namespace IplAnalysis{
    class Delivery
    {
        public string MatchId { get; set; }
        public string BattingTeam { get; set; }
        public string BowlingTeam { get; set; }
        public string Batsmen { get; set; }
        public int BatsmanRuns { get; set; }
        public string Bowler { get; set; }
        public int Ball { get; set; }
        public int TotalRuns { get; set; }
        public int ExtraRuns { get; set; }
        public int ByesRuns { get; set; }
        public int LegByeRuns { get; set; }
        public int PenaltyRuns { get; set; }
        public int Wide { get; set; }
        public int NoBall { get; set; }
    }
}