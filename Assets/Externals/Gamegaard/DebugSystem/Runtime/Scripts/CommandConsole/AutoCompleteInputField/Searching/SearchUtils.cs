using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamegaard.RuntimeDebug
{
    public static class SearchUtils
    {
        private static readonly Dictionary<(string, string), int> _similarityCache = new Dictionary<(string, string), int>();

        public static IEnumerable<LogCommandBase> ApplyLevenshteinSearch(IEnumerable<LogCommandBase> suggestions, string value, int minScore = 5)
        {
            return ApplySearch(suggestions, value, new LevenshteinCalculator(), SearchSettings.Default, minScore);
        }

        public static IEnumerable<LogCommandBase> ApplyDamerauLevenshteinSearch(IEnumerable<LogCommandBase> suggestions, string value, int minScore = 5)
        {
            return ApplySearch(suggestions, value, new DamerauLevenshteinCalculator(), SearchSettings.Default, minScore);
        }

        public static IEnumerable<LogCommandBase> ApplySift4CalculatorSearch(IEnumerable<LogCommandBase> suggestions, string value, int minScore = 5)
        {
            return ApplySearch(suggestions, value, new Sift4Calculator(), SearchSettings.Default, minScore);
        }

        public static IEnumerable<LogCommandBase> ApplySearch(IEnumerable<LogCommandBase> suggestions, string value, ISimilarityCalculator calculator, SearchSettings settings, int minScore = 5)
        {
            var sortedCommands = suggestions
                .Select(cmd => new
                {
                    Command = cmd,
                    Score = GetSimilarityScore(cmd.Format, value, calculator, settings)
                })
                .Where(cmd => cmd.Score >= minScore)
                .OrderByDescending(cmd => cmd.Score)
                .Select(cmd => cmd.Command)
                .ToArray();

            return sortedCommands;
        }

        private static int GetSimilarityScore(string command, string query, ISimilarityCalculator calculator, SearchSettings settings)
        {
            int score = 0;

            if (_similarityCache.TryGetValue((command, query), out int cachedScore))
            {
                return cachedScore;
            }

            if (command.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            {
                score += settings.StartsWithScore;
            }

            if (command.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                score += settings.ContainsScore;
            }

            score += GetCharacterMatchScore(command, query, settings);

            int commandLength = command.Length;
            int queryLength = query.Length;

            if (queryLength > commandLength)
            {
                int lengthPenalty = Math.Max(0, commandLength - queryLength) * settings.MaxPenaltyFactor;
                score -= lengthPenalty;
                score -= Math.Max(0, (commandLength - queryLength) / settings.LengthPenaltyFactor);
                score = Math.Max(score, 0);
            }

            if (calculator != null)
            {
                score -= calculator.Calculate(command, query) * settings.MaxPenaltyFactor;
            }

            _similarityCache[(command, query)] = score;
            return score;
        }

        private static int GetCharacterMatchScore(string command, string query, SearchSettings settings)
        {
            int score = 0;
            int commandIndex = 0;

            string commandLower = command.ToLower();
            string queryLower = query.ToLower();

            foreach (char c in queryLower)
            {
                commandIndex = commandLower.IndexOf(c, commandIndex);

                if (commandIndex != -1)
                {
                    score += settings.CharacterMatchScore;
                    commandIndex++;
                }
                else
                {
                    break;
                }
            }

            if (commandLower.Contains(queryLower))
            {
                score += settings.CharacterMatchScore;
            }

            return score;
        }
    }
}