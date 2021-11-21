using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public static class Utils
    {
        public static T GetRandomElement<T>(this List<T> collection)
        {
            var size = collection.Count;
            var randomIdx = Random.Range(0, size - 1);
            return collection[randomIdx];
        }
    }
}