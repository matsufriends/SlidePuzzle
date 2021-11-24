using UnityEngine;

namespace _Develop.Matsu {
    public static class TransformExpansions {
        public static void DestroyChildren(this Transform _base) {
            for (var i = _base.childCount; i > 0; i--) Object.Destroy(_base.GetChild(i - 1).gameObject);
        }
        
        public static void DestroyImmediateChildren(this Transform _base) {
            for (var i = _base.childCount; i > 0; i--) Object.DestroyImmediate(_base.GetChild(i - 1).gameObject);
        }

        public static Vector3 Dif(this Transform _base, float _x, float _y, float _z) {
            return  _base.right * _x +_base.up *_y +_base.forward *_z;
        }
    }
}