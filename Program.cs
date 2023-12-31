using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IplAnalysis
{
    class Program
    {
        public const int MATCH_ID = 0;
        public const int MATCH_YEAR = 1;
        public const int MATCH_TEAM1 = 4;
        public const int MATCH_TEAM2 = 5;
        public const int MATCH_CITY = 2;
        public const int MATCH_PLAYER_OF_MATCH = 13;
        public const int MATCH_WINNER_TEAM = 10;
        public const int DELIVERY_MATCH_ID = 0;
        public const int DELIVERY_BATTING_TEAM = 2;
        public const int DELIVERY_BOWLING_TEAM = 3;
        public const int DELIVERY_BATSMEN = 6;
        public const int DELIVERY_BATSMAN_RUN = 15;
        public const int DELIVERY_BOWLER = 8;
        public const int DELIVERY_BALL = 5;
        public const int DELIVERY_TOTAL_RUNS = 17;
        public const int DELIVERY_EXTRA_RUN = 16;
        public const int DELIVERY_BYES_RUN = 11;
        public const int DELIVERY_LEGBYE_RUNS = 12;
        public const int DELIVERY_PENALTY_RUNS = 14;
        public const int DELIVERY_WIDE_RUNS = 10;
        public const int DELIVERY_NOBALL_RUNS = 13;
        public const string DATA_DELIVERIES_CSV = "data/deliveries.csv";
        public const string DATA_MATCHES_CSV = "data/matches.csv";

        static void Main(string[] args)
        {
            List<Match> matches = GetMatchData();
            List<Delivery> deliveries = GetDeliveryData();

            FindNumberOfMatchesPlayedPerYear(matches);
            FindNumberOfMatchWonByTeamAllYear(matches);
            FindExtraRunByEachTeamInYear2016(matches, deliveries);
            FindTopEconomicalBowlerIn2015(matches, deliveries);
            FindMaximumNumberOfManOfTheMatchEachYear(matches);
        }

        private static void FindMaximumNumberOfManOfTheMatchEachYear(List<Match> matches)
        {
            Dictionary<string, Dictionary<string, int>> manOfTheMatchPlayerEachYear = new Dictionary<string, Dictionary<string, int>>();

            try
            {
                foreach (Match match in matches)
                {
                    if (manOfTheMatchPlayerEachYear.ContainsKey(match.Year))
                    {
                        Dictionary<string, int> manOfMatchByPlayer = manOfTheMatchPlayerEachYear[match.Year];

                        int totalManOfTheMatchByPlayer = manOfMatchByPlayer.GetValueOrDefault(match.PlayerOfMatch, 0) + 1;
                        manOfMatchByPlayer[match.PlayerOfMatch] = totalManOfTheMatchByPlayer;

                        manOfTheMatchPlayerEachYear[match.Year] = manOfMatchByPlayer;
                    }
                    else
                    {
                        Dictionary<string, int> manOfMatchByPlayer = new Dictionary<string, int>();
                        manOfMatchByPlayer[match.PlayerOfMatch] = 1;
                        manOfTheMatchPlayerEachYear[match.Year] = manOfMatchByPlayer;
                    }
                }

                foreach (string year in manOfTheMatchPlayerEachYear.Keys)
                {
                    Console.Write(year + " :");

                    string maxManOfTheMatchPlayer = " ";
                    int maxManOfTheMatch = 0;

                    Dictionary<string, int> map = manOfTheMatchPlayerEachYear[year];

                    foreach (string batsman in map.Keys)
                    {
                        if (map[batsman] > maxManOfTheMatch)
                        {
                            maxManOfTheMatch = map[batsman];
                            maxManOfTheMatchPlayer = batsman;
                        }
                    }

                    Console.WriteLine(maxManOfTheMatchPlayer + "  total " + maxManOfTheMatch);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void FindTopEconomicalBowlerIn2015(List<Match> matches, List<Delivery> deliveries)
        {
            HashSet<string> matchId = GetMatchIdByYear(matches, "2015");
            Dictionary<string, int> runGivenByBowler = new Dictionary<string, int>();
            Dictionary<string, int> ballBowledByBowler = new Dictionary<string, int>();
            Dictionary<string, double> bowlerEconomy = new Dictionary<string, double>();

            try
            {
                foreach (Delivery delivery in deliveries)
                {
                    if (matchId.Contains(delivery.MatchId))
                    {
                        int totalRunsGivenByBowler = delivery.TotalRuns - delivery.LegByeRuns
                                - delivery.ByesRuns - delivery.PenaltyRuns;
                        int totalRunByBowlerGiven = runGivenByBowler.GetValueOrDefault(delivery.Bowler, 0)
                                + totalRunsGivenByBowler;
                        runGivenByBowler[delivery.Bowler] = totalRunByBowlerGiven;

                        int totalBallBowledByBowler = ballBowledByBowler.GetValueOrDefault(delivery.Bowler, 0) + 1;
                        totalBallBowledByBowler -= delivery.NoBall + delivery.Wide;
                        ballBowledByBowler[delivery.Bowler] = totalBallBowledByBowler;

                        foreach (KeyValuePair<string, int> entry in runGivenByBowler)
                        {
                            string bowler = entry.Key;
                            int run = entry.Value;
                            int ball = ballBowledByBowler[bowler];
                            double economyRate = (double)run / ((double)ball / 6);
                            bowlerEconomy[bowler] = economyRate;
                        }
                    }
                }

                List<KeyValuePair<string, double>> sortBowlerByEconomy = bowlerEconomy.ToList();
                sortBowlerByEconomy.Sort(new EntryValueComparer());

                KeyValuePair<string, double> lowestEconomyBowler = sortBowlerByEconomy[0];

                string bowlerName = lowestEconomyBowler.Key;
                double economyData = lowestEconomyBowler.Value;

                Console.WriteLine("Bowler Name: " + bowlerName + "  " + "Economy Data: " + economyData);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void FindExtraRunByEachTeamInYear2016(List<Match> matches, List<Delivery> deliveries)
        {
            HashSet<string> matchId = GetMatchIdByYear(matches, "2016");
            Dictionary<string, int> extraRunByTeam = new Dictionary<string, int>();

            try
            {
                foreach (Delivery delivery in deliveries)
                {
                    if (matchId.Contains(delivery.MatchId))
                    {
                        int extraRun = extraRunByTeam.GetValueOrDefault(delivery.BattingTeam, 0)
                                + delivery.ExtraRuns;
                        extraRunByTeam[delivery.BattingTeam] = extraRun;
                    }
                }

                foreach (KeyValuePair<string, int> entry in extraRunByTeam)
                {
                    Console.WriteLine("Team- " + entry.Key + " " + "extra run :" + entry.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private static HashSet<string> GetMatchIdByYear(List<Match> matches, string year)
        {
            HashSet<string> matchId = new HashSet<string>();

            try
            {
                foreach (Match match in matches)
                {
                    if (match.Year.Equals(year))
                    {
                        matchId.Add(match.Id);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return matchId;
        }

        private static void FindNumberOfMatchWonByTeamAllYear(List<Match> matches)
        {
            Dictionary<string, int> totalMatchWonByTeam = new Dictionary<string, int>();

            try
            {
                foreach (Match match in matches)
                {
                    int matchWonByTeam = totalMatchWonByTeam.GetValueOrDefault(match.WinnerTeam, 0) + 1;
                    totalMatchWonByTeam[match.WinnerTeam] = matchWonByTeam;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            foreach (KeyValuePair<string, int> entry in totalMatchWonByTeam)
            {
                Console.WriteLine("Team " + entry.Key + " " + "Match won :" + entry.Value);
            }
        }

        private static void FindNumberOfMatchesPlayedPerYear(List<Match> matches)
        {
            Dictionary<string, int> matchPerYear = new Dictionary<string, int>();

            try
            {
                foreach (Match match in matches)
                {
                    int noOfMatch = matchPerYear.GetValueOrDefault(match.Year, 0) + 1;
                    matchPerYear[match.Year] = noOfMatch;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            foreach (KeyValuePair<string, int> entry in matchPerYear)
            {
                Console.WriteLine("Year " + entry.Key + " " + "Match played :" + entry.Value);
            }
        }

        private static List<Delivery> GetDeliveryData()
        {
            List<Delivery> deliveries = new List<Delivery>();

            StreamReader reader = null;
            string line = "";

            try
            {
                reader = new StreamReader(DATA_DELIVERIES_CSV);
                reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                {
                    string[] data = line.Split(",");

                    Delivery delivery = new Delivery
                    {
                        MatchId = data[DELIVERY_MATCH_ID],
                        BattingTeam = data[DELIVERY_BATTING_TEAM],
                        BowlingTeam = data[DELIVERY_BOWLING_TEAM],
                        Batsmen = data[DELIVERY_BATSMEN],
                        BatsmanRuns = int.Parse(data[DELIVERY_BATSMAN_RUN]),
                        Bowler = data[DELIVERY_BOWLER],
                        Ball = int.Parse(data[DELIVERY_BALL]),
                        TotalRuns = int.Parse(data[DELIVERY_TOTAL_RUNS]),
                        ExtraRuns = int.Parse(data[DELIVERY_EXTRA_RUN]),
                        ByesRuns = int.Parse(data[DELIVERY_BYES_RUN]),
                        LegByeRuns = int.Parse(data[DELIVERY_LEGBYE_RUNS]),
                        PenaltyRuns = int.Parse(data[DELIVERY_PENALTY_RUNS]),
                        Wide = int.Parse(data[DELIVERY_WIDE_RUNS]),
                        NoBall = int.Parse(data[DELIVERY_NOBALL_RUNS])
                    };

                    deliveries.Add(delivery);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return deliveries;
        }

        private static List<Match> GetMatchData()
        {
            List<Match> matches = new List<Match>();

            StreamReader reader = null;
            string line = "";

            try
            {
                reader = new StreamReader(DATA_MATCHES_CSV);
                reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                {
                    string[] data = line.Split(",");

                    Match match = new Match
                    {
                        Id = data[MATCH_ID],
                        Year = data[MATCH_YEAR],
                        Team1 = data[MATCH_TEAM1],
                        Team2 = data[MATCH_TEAM2],
                        City = data[MATCH_CITY],
                        PlayerOfMatch = data[MATCH_PLAYER_OF_MATCH],
                        WinnerTeam = data[MATCH_WINNER_TEAM]
                    };

                    matches.Add(match);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return matches;
        }
    }

    class EntryValueComparer : IComparer<KeyValuePair<string, double>>
    {
        public int Compare(KeyValuePair<string, double> entry1, KeyValuePair<string, double> entry2)
        {
            return entry1.Value.CompareTo(entry2.Value);
        }
    }
}
