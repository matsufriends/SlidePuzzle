using UnityEngine;

namespace _Develop.Script {
    public class BlockObjMono : MonoBehaviour {
        [SerializeField]
        private SpriteRenderer mSpriteRenderer;
        
        [SerializeField]
        private Color mNormalColor;

        [SerializeField]
        private Color mDragColor;

        public int   IndexH { get; private set; }
        public int   IndexW { get; private set; }
        public float PosX   => transform.position.x;
        public float ScaleX { get; private set; }

        public void Init(int _indexH, int _indexW, Vector3 _scale) {
            IndexH               = _indexH;
            IndexW               = _indexW;
            ScaleX               = _scale.x;
            transform.localScale = _scale;
        }

        public void SetX(float _x) {
            var pos = transform.position;
            pos.x              = _x;
            transform.position = pos;
        }

        public void SetDrag(bool _isDragged) {
            mSpriteRenderer.color = _isDragged ? mDragColor : mNormalColor;
        }
    }
}