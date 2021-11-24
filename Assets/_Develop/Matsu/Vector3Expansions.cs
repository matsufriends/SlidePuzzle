using UnityEngine;

namespace _Develop.Matsu {
    public static class Vector3Expansions {
        public static Vector2 XZ(this Vector3 _base) {
            return new Vector2(_base.x, _base.z);
        }
    }
}