using R3;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class OptionView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider seVolumeSlider;
        [SerializeField] private Slider sensibilityXSlider;
        [SerializeField] private Slider sensibilityYSlider;
        [SerializeField] private Toggle vibrationToggle;

        public Observable<Unit> OnBackButtonPressed => backButton.OnClickAsObservable();

        public Observable<float> OnBgmVolumeChanged => bgmVolumeSlider
            .OnValueChangedAsObservable()
            .Select(value => value / bgmVolumeSlider.maxValue);

        public Observable<float> OnSeVolumeChanged => seVolumeSlider
            .OnValueChangedAsObservable()
            .Select(value => value / seVolumeSlider.maxValue);

        public Observable<Vector2> OnControllerSensibilityChanged =>
            Observable.Merge(sensibilityXSlider.OnValueChangedAsObservable(), sensibilityYSlider.OnValueChangedAsObservable())
                .Select(_ => new Vector2(sensibilityXSlider.value / sensibilityXSlider.maxValue, sensibilityYSlider.value / sensibilityYSlider.maxValue));

        public Observable<bool> OnVibrationToggleChanged => vibrationToggle.OnValueChangedAsObservable();

        public void SetBgmVolume(float value)
        {
            bgmVolumeSlider.value = value * bgmVolumeSlider.maxValue;
        }

        public void SetSeVolume(float value)
        {
            seVolumeSlider.value = value * seVolumeSlider.maxValue;
        }

        public void SetControllerSensibility(Vector2 value)
        {
            sensibilityXSlider.value = value.x * sensibilityXSlider.maxValue;
            sensibilityYSlider.value = value.y * sensibilityYSlider.maxValue;
        }

        public void SetVibrationToggle(bool value)
        {
            vibrationToggle.isOn = value;
        }
    }
}