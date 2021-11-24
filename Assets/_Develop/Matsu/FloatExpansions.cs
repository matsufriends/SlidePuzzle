namespace _Develop.Matsu {
    public static class FloatExpansions {
        public static bool AsProbability(this float _base) {
            return UnityEngine.Random.Range(0, 1f) < _base;
        }
    }
}