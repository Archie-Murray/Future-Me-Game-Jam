using System.Collections;


using Enemy;

using UnityEngine;
using UnityEngine.SceneManagement;

using Utilities;

using Cinemachine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System;

public class GameManager : Singleton<GameManager> {
    public bool BossDead = false;
    public bool PlayerAlive = true;
    public bool InMenu = false;
    [SerializeField] private CanvasGroup _winScreen;
    [SerializeField] private CanvasGroup _loseScreen;
    [SerializeField] private TickSystem _tickSystem;
    [SerializeField] private EnemyManager[] _enemyManagers = null;
    [SerializeField] private CountDownTimer _combatTimer = new CountDownTimer(0f);
    [SerializeField] private CinemachineBasicMultiChannelPerlin _cameraNoise;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private BGMEmitter _bgmEmitter;
    [SerializeField] private Volume _postProcessVolume;
    [SerializeField] private ChromaticAberration _chromaticAberration;
    [SerializeField] private GameObject _tutorialManager;
    private float _initialCameraSize;
    private Coroutine _endGameState = null;

    private void Start() {
        _enemyManagers = FindObjectsOfType<EnemyManager>();
        _tickSystem = gameObject.GetOrAddComponent<TickSystem>();
        _tickSystem.TickLoop += HandleEndGame;
        _tickSystem.TickLoop += (float deltaTime) => _combatTimer.Update(deltaTime);
        _bgmEmitter = GetComponent<BGMEmitter>();
        _combatTimer.OnTimerStart += () => _bgmEmitter.PlayBGM(BGMType.COMBAT);
        _combatTimer.OnTimerStop += () => _bgmEmitter.PlayBGM(BGMType.PASSIVE);
        _virtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        _initialCameraSize = _virtualCamera.m_Lens.OrthographicSize;
        _cameraNoise = _virtualCamera.OrNull()?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>() ?? null;
        _postProcessVolume = FindFirstObjectByType<Volume>();
        _postProcessVolume.profile.TryGet(out _chromaticAberration);
        if (!_chromaticAberration) {
            Debug.Log("IDK, AHHHHHHHHHHHHHHHHHHHHH");
        }
    }

    public void CameraAberration(float amount, float time = 2f) {
        StartCoroutine(AbberationCamera(amount, time));
    }

    public IEnumerator AbberationCamera(float amount, float time) {
        amount = Mathf.Clamp01(amount);
        CountDownTimer timer = new CountDownTimer(time * 0.5f);
        timer.Start();
        while (timer.IsRunning) {
            timer.Update(Time.fixedDeltaTime);
            _chromaticAberration.intensity.Interp(0f, amount, 1f - timer.Progress);
            yield return Yielders.WaitForFixedUpdate;
        }
        timer.Reset();
        timer.Start();
        while (timer.IsRunning) {
            timer.Update(Time.fixedDeltaTime);
            _chromaticAberration.intensity.Interp(amount, 0f, 1f - timer.Progress);
            yield return Yielders.WaitForFixedUpdate;
        }
    }

    public void CameraPan(float amount, float time = 2f) {
        StartCoroutine(PanCamera(amount, time));
    }

    private IEnumerator PanCamera(float amount, float time) {
        float timer = time * 0.5f;
        float deltaMax = amount / time;
        float newSize = _initialCameraSize + amount;
        while (timer > 0f) {
            timer -= Time.fixedDeltaTime;
            _virtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_virtualCamera.m_Lens.OrthographicSize, newSize, deltaMax * Time.fixedDeltaTime);
            yield return Yielders.WaitForFixedUpdate;
        }
        _virtualCamera.m_Lens.OrthographicSize = newSize;
        timer = time * 0.5f;
        while (timer > 0f) {
            timer -= Time.fixedDeltaTime;
            _virtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_virtualCamera.m_Lens.OrthographicSize, _initialCameraSize, deltaMax * Time.fixedDeltaTime);
            yield return Yielders.WaitForFixedUpdate;
        }
        _virtualCamera.m_Lens.OrthographicSize = _initialCameraSize;
    }

    public void CameraShake(float intensity = 1f, float time = 0.1f) {
        StartCoroutine(ShakeCamera(intensity, time));
    }

    private IEnumerator ShakeCamera(float intensity, float time) {
        if (_cameraNoise == null) {
            Debug.LogError("Perlin Noise Not initialised");
            yield break;
        }
        _cameraNoise.m_AmplitudeGain = intensity;
        yield return Yielders.WaitForSeconds(time);
        _cameraNoise.m_AmplitudeGain = 0f;
        yield return Yielders.WaitForEndOfFrame;
        _virtualCamera.transform.rotation = Quaternion.identity;
    }

    public bool GameEnd => BossDead && SpawnersFinished;
    private bool SpawnersFinished {
        get {
            foreach (EnemyManager enemyManager in _enemyManagers) {
                if (!enemyManager.FinishedSpawning) {
                    return false;
                }
            }
            return true;
        }
    }
    private void HandleEndGame(float dt) {
        if (_endGameState != null) {
            return;
        }
        if (GameEnd) {
            _endGameState = StartCoroutine(WinState());
        } else if (!PlayerAlive) {
            _endGameState = StartCoroutine(LoseState());
        }
    }

    private IEnumerator WinState() {
        ToggleWinScreen(true);
        yield return Yielders.WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }

    private void ToggleWinScreen(bool toggle) {
        _winScreen.interactable = toggle;
        _winScreen.alpha = toggle ? 1f : 0f;
    }

    private IEnumerator LoseState() {
        ToggleLoseScreen(true);
        yield return Yielders.WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }

    private void ToggleLoseScreen(bool toggle) {
        _loseScreen.interactable = toggle;
        _loseScreen.alpha = toggle ? 1f : 0f;
    }

    public void MainMenu() {
        SceneManager.LoadScene(0);
    }

    public void ResetCombatTimer() {
        _combatTimer.Reset(5f);
        _combatTimer.Start();
    }

    public void StartTutorial() {
        Instantiate(_tutorialManager, Vector3.zero, Quaternion.identity);
    }
}