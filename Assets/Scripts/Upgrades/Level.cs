using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Upgrades;

using Utilities;

public class Level : MonoBehaviour {
    [Header("XP Variables")]
    [SerializeField] private int _level = 0;
    [SerializeField] private int _xp = 0;

    [Header("Level Config")]
    [SerializeField] private AnimationCurve _experienceCurve;
    [SerializeField] private int _minXP = 10;
    [SerializeField] private int _levelXP = 10;

    [Header("UI References - Must be assigned in editor!")]
    [SerializeField] private Slider _xpBar;
    [SerializeField] private UpgradeManager _upgradeManager;
    [SerializeField] private TMP_Text _xpReadout;
    [SerializeField] private TMP_Text _xpTickText;

    [Header("UI Variables")]
    [SerializeField] private float _animationSpeed = 5f;
    [SerializeField] private float _currentValue;
    [SerializeField] private int _tickRate = 5;

    public float LevelProgress => _level == MAX_LEVEL ? 1f : (float) _xp / NextLevelXP;
    public int NextLevelXP => _minXP + Mathf.CeilToInt(_experienceCurve.Evaluate((float) _level / MAX_LEVEL) * _levelXP);
    public int RequiredLevelXP => NextLevelXP - _xp;

    public const int MAX_LEVEL = 10;

    private void Start() {
        _experienceCurve ??= AnimationCurve.Linear(0f, 0f, 1f, 1f);
        _currentValue = LevelProgress;
        Debug.Log("Level progress: " + LevelProgress + " Required Level XP: " + RequiredLevelXP);
        _upgradeManager = transform.parent.GetComponentInChildren<UpgradeManager>();
        _xpBar = GetComponent<Slider>();
        _xpBar.value = _currentValue;
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in texts) {
            if (text.name == "Readout") {
                _xpReadout = text;
            } else if (text.name == "XP Tick") {
                _xpTickText = text;
            }
        }
    }

    private void FixedUpdate() {
        //UpdateXPBar();
    }

    private void UpdateXPBar() {
        _currentValue = Mathf.MoveTowards(_currentValue, LevelProgress, _animationSpeed * Time.fixedDeltaTime);
        _xpBar.value = _currentValue;
        _xpReadout.text = $"{_xp} / {NextLevelXP} ({LevelProgress:0%})";
    }

    public void AddXP(int amount) {
        StartCoroutine(IncreaseXP(amount));
    }

    private IEnumerator IncreaseXP(int amount) {
        _xpTickText.text = amount.ToString();
        while (amount > 0) {
            if (_level == MAX_LEVEL) {
                _xp = 0;
                UpdateXPBar();
                _xpTickText.text = string.Empty;
                Debug.Log("Player Reached Max Level!");
                yield break;
            } else if (amount >= RequiredLevelXP && RequiredLevelXP != 0) { //Guard against infinite loop
                Debug.Log("Player Levelled Up!");
                _level++;
                amount -= RequiredLevelXP;
                _upgradeManager.AllowUpgrade();
                _xpTickText.text = amount.ToString();
                UpdateXPBar();
            } else {
                _xp += Mathf.Min(_tickRate, amount);
                amount -= Mathf.Min(_tickRate, amount);
                _xpTickText.text = amount.ToString();
                Debug.Log("Player gained " + amount + " xp!");
                UpdateXPBar();
                yield break;
            }
            yield return Yielders.WaitForFixedUpdate;
        }
    }
}