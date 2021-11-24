using UnityEngine.EventSystems;

namespace _Develop.Matsu {
    public static class PointEventDataExpansions {
        public static bool IsLeftClick(this PointerEventData _base) {
            return _base.pointerId == -1;
        }
    }
}