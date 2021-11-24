using UnityEngine;
using UnityEngine.UI;

namespace _Develop.Script {
    public class UIManagerMono : MonoBehaviour {
        [Header("Random")]
        [SerializeField]
        private GameObject mRandomParent;

        [SerializeField]
        private Button mRandomButton;

        [SerializeField]
        private Slider mRandomSliderW;

        [SerializeField]
        private Slider mRandomSliderH;

        [SerializeField]
        private Slider mRandomSliderP;

        [SerializeField]
        private Button mRandomInitButton;

        [SerializeField]
        private Button mRandomCloseButton;

        private void Awake() {
            //Random
            mRandomButton.onClick.AddListener(
                () => {
                    mRandomParent.SetActive(true);
                }
            );
            mRandomInitButton.onClick.AddListener(
                () => {
                    PuzzleManagerMono.InitFromRandom(
                        Mathf.RoundToInt(mRandomSliderW.value)
                      , Mathf.RoundToInt(mRandomSliderH.value)
                      , mRandomSliderP.value
                    );
                    mRandomParent.SetActive(false);
                }
            );
            mRandomCloseButton.onClick.AddListener(
                () => {
                    mRandomParent.SetActive(false);
                }
            );
        }
    }
}