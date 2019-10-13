﻿using System;

namespace OCR.TextComparator
{
    class LevenshteinDistance
    {
        public static int Calculate(string firstText, string secondText)
        {
            int n = firstText.Length;
            int m = secondText.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;

            if (m == 0)
                return n;

            for (int i = 0; i <= n; d[i, 0] = i++){ }

            for (int j = 0; j <= m; d[0, j] = j++){ }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (secondText[j - 1] == firstText[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
}
