﻿using System;
using System.Linq;
using System.Collections.Generic;

static class CombinationGenerator
{
    private static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        return k == 0 ?
            new[] { new T[0] } :
            elements.SelectMany((e, i) => elements.Skip(i + 1)
                .Combinations(k - 1)
                .Select(c =>
                    (new[] { e }).Concat(c)
                )
            );
    }

    // { 1, 2 } => { }, { 1 }, { 2 }, { 1, 2 }
    public static ICollection<IEnumerable<T>> Generate<T>(ICollection<T> elements)
    {
        var results = new List<IEnumerable<T>>();

        for (int i = 0; i <= elements.Count; i++)
            results.AddRange(elements.Combinations(i));

        return results;
    }
}
