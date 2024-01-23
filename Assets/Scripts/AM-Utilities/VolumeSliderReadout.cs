using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderReadOut : MonoBehaviour {
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _readout;
    [SerializeField] private string _readoutPrefix;
    

    private void Awake() {
        _slider = GetComponent<Slider>();
        _readout = GetComponentInChildren<TMP_Text>();
        _slider.onValueChanged.AddListener((float value) => _readout.text = $"{_readoutPrefix} {value:0%}");
        _readout.text = $"{_readoutPrefix} {_slider.value:0%}";
    }
}