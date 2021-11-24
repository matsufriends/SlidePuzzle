using UnityEngine;
using UnityEngine.UI;

namespace _Develop.Script {
    public class SliderMono : MonoBehaviour {
        [SerializeField]
        private Slider mSlider;

        [SerializeField]
        private string mBaseText;
        
        [SerializeField]
        private Text mText;

        private void Awake() {
            mSlider.onValueChanged.AddListener(
                _value => {
                    mText.text = $"{mBaseText}:{mSlider.value}";
                });
        }
    }
}