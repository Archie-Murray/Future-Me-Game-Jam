using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
public class StatsManager : MonoBehaviour {
    private CanvasGroup _statCanvas;
    private TMP_Text _statText;
    private PlayerController _playerController;
    [SerializeField] private bool _panelOpen = false;
    private void Start() {
        Globals.Instance.Controls.UI.OpenStats.started += ToggleStats;
        _statCanvas = GetComponent<CanvasGroup>();
        _statText = GetComponentsInChildren<TMP_Text>().FirstOrDefault((TMP_Text text) => text.name == "Stat Text");
        _panelOpen = _statCanvas.alpha == 1f;
        _playerController = FindFirstObjectByType<PlayerController>();
        _playerController.GetComponent<Stats>().OnStatChange += (_, _) => UpdateStats();
    }

    private void ToggleStats(InputAction.CallbackContext context) {
        _panelOpen = !_panelOpen;
        UpdateStats();
        _statCanvas.FadeCanvas(0.15f, !_panelOpen, this);
    }

    private void UpdateStats() {
        _statText.text = _playerController.GetStatus();
    }
}