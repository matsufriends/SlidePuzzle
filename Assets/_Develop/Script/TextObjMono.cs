using UnityEngine;

namespace _Develop.Script {
    public class TextObjMono : MonoBehaviour {
        [SerializeField]
        private SpriteRenderer mBackSprite;

        [SerializeField]
        private Color mInCollectColor;

        [SerializeField]
        private Color mCollectColor;

        [SerializeField]
        private TextMesh mText;

        public int Value { get; private set; }

        public void Init(int _value) {
            Value          = _value;
            mText.text     = _value.ToString();
            mText.fontSize = _value >= 10 ? 88 : 122;
        }

        public void SetColor(bool _isCollect) {
            mBackSprite.color = _isCollect ? mCollectColor : mInCollectColor;
        }
    }
}