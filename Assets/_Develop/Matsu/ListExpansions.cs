using System;
using System.Collections.Generic;
using System.Linq;

namespace _Develop.Matsu {
    public static class ListExpansions {
        public static T RandomValue<T>(this IEnumerable<T> _base) {
            var enumerable = _base as T[] ?? _base.ToArray();
            return enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Count()));
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> _base) {
            return _base.OrderBy(_ => Guid.NewGuid());
        }
    }
}