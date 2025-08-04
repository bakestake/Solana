using System;

namespace Gamegaard.RuntimeDebug
{
    public class Sift4Calculator : ISimilarityCalculator
    {
        private readonly int maxOffset;

        public Sift4Calculator(int maxOffset = 5)
        {
            this.maxOffset = maxOffset;
        }

        public int Calculate(string command, string query)
        {
            if (string.IsNullOrEmpty(command)) return query.Length;
            if (string.IsNullOrEmpty(query)) return command.Length;
            if (command == query) return 0;

            int l1 = command.Length;
            int l2 = query.Length;

            int c1 = 0;
            int c2 = 0;
            int lcss = 0;
            int localCs = 0;
            int transpositions = 0;

            while (c1 < l1 && c2 < l2)
            {
                if (command[c1] == query[c2])
                {
                    localCs++;
                }
                else
                {
                    lcss += localCs;
                    localCs = 0;

                    if (c1 != c2)
                    {
                        transpositions++;
                    }

                    int maxOffsetTemp = Math.Min(maxOffset, Math.Max(l1, l2));
                    bool foundMatch = false;

                    for (int i = 0; i < maxOffsetTemp; i++)
                    {
                        if ((c1 + i < l1) && (command[c1 + i] == query[c2]))
                        {
                            c1 += i;
                            foundMatch = true;
                            break;
                        }

                        if ((c2 + i < l2) && (command[c1] == query[c2 + i]))
                        {
                            c2 += i;
                            foundMatch = true;
                            break;
                        }
                    }

                    if (!foundMatch)
                    {
                        break;
                    }
                }

                c1++;
                c2++;
            }

            lcss += localCs;

            return Math.Max(l1, l2) - lcss + transpositions;
        }
    }
}