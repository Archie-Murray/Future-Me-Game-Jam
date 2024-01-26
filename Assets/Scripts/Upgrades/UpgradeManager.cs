using System;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Upgrades {
    public class UpgradeManager : MonoBehaviour {

        private const int UPGRADE_SLOTS = 3;

        [Header("Debug References (should not need to change!)")]
        [SerializeField] private Stats _playerStats;
        [SerializeField] private CanvasGroup _uiCanvas;
        [SerializeField] private UpgradeUI[] _upgradeSlots;
        [SerializeField] private int _unappliedUpgrades = 0;

        [Header("Must be set in editor")]
        [SerializeField] private Upgrade[] _upgrades;
        [SerializeField] private GameObject _upgradePrefab;
        [SerializeField] private Sprite _defaultIcon;

        private void Awake() {
            _playerStats = FindFirstObjectByType<PlayerController>().GetComponent<Stats>();
            _uiCanvas = FindFirstObjectByType<UIUpgrade>().GetComponent<CanvasGroup>();
            _uiCanvas.blocksRaycasts = false;
            _upgradeSlots = new UpgradeUI[UPGRADE_SLOTS];
            for (int i = 0; i < UPGRADE_SLOTS; i++) {
                GameObject slot = Instantiate(_upgradePrefab, _uiCanvas.transform);
                _upgradeSlots[i] = new UpgradeUI(
                    slot.GetComponentInChildren<Button>(),
                    slot.GetComponentsInChildren<Image>().First((Image image) => image.name == "Icon"),
                    slot.GetComponentInChildren<TMP_Text>(),
                    this
                );
                _upgradeSlots[i].UpgradeImage.sprite = _defaultIcon;
            }
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.U)) {
                ShowUpgrades();
            }
            if (_unappliedUpgrades > 0 && _uiCanvas.alpha == 0f) {
                ShowUpgrades();
            }
        }

        public void AllowUpgrade() {
            _unappliedUpgrades++;
        }

        // Want to call this when the player gets enough xp to level up
        private void ShowUpgrades() {
            GameManager.Instance.InMenu = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _uiCanvas.FadeCanvas(0.25f, false, this);
            foreach (UpgradeUI upgradeUI in _upgradeSlots) {
                upgradeUI.SetUpgrade(GetRandomUpgrade());
            }
        }

        private void HideUpgrades() {
            _uiCanvas.FadeCanvas(0.25f, true, this);
            foreach (UpgradeUI upgradeUI in _upgradeSlots) {
                upgradeUI.UnsetUpgrade();
            }
            GameManager.Instance.InMenu = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private Upgrade GetRandomUpgrade() {
            return _upgrades[UnityEngine.Random.Range(0, _upgrades.Length)];
        }

        [Serializable] private class UpgradeUI {
            public Button UpgradeButton;
            public Image UpgradeImage;
            public TMP_Text Text;
            public UpgradeManager UpgradeManager;

            public UpgradeUI(Button upgradeButton, Image upgradeImage, TMP_Text text, UpgradeManager upgradeManager) {
                UpgradeButton = upgradeButton;
                UpgradeImage = upgradeImage;
                Text = text;
                UpgradeManager = upgradeManager;
            }

            public void SetUpgrade(Upgrade upgrade) {
                UpgradeImage.sprite = upgrade.Icon;
                UpgradeImage.color = upgrade.IconColor;
                Text.text = upgrade.Description;
                UpgradeButton.onClick.RemoveAllListeners();
                UpgradeButton.onClick.AddListener(() => ApplyUpgrade(upgrade));
            }

            public void UnsetUpgrade() {
                UpgradeImage.sprite = UpgradeManager._defaultIcon;
                Text.text = string.Empty;
                UpgradeButton.onClick.RemoveAllListeners();
            }

            private void ApplyUpgrade(Upgrade upgrade) {
                upgrade.ApplyUpgrade(UpgradeManager._playerStats);
                UpgradeManager.HideUpgrades();
                UpgradeManager._unappliedUpgrades--;
            }
        }
    }

}