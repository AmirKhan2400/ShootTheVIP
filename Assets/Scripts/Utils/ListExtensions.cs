using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// Shuffles the list in place using Fisher–Yates algorithm.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // inclusive upper bound
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
