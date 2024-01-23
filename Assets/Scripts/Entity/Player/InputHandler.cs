using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class InputHandler : MonoBehaviour {
    public Vector2 MoveInput;
    public Vector2 MousePosition;
    public float DashInput;
    public float SprintInput;
    public float BrakeInput;
    public bool FireInput;
    public bool HeavyFireInput;
    public bool EliteFireInput;

    private Controls _inputActions;
    public Controls InputActions => _inputActions;

    public void Awake() {
        _inputActions = new Controls();
        _inputActions.PlayerControls.Move.started += MoveHandle;
        _inputActions.PlayerControls.Move.performed += MoveHandle;
        _inputActions.PlayerControls.Move.canceled += MoveHandle;
        _inputActions.PlayerControls.Dash.started += DashHandle;
        _inputActions.PlayerControls.Dash.performed += DashHandle;
        _inputActions.PlayerControls.Dash.canceled += DashHandle;
        _inputActions.PlayerControls.Brake.started += BrakeHandle;
        _inputActions.PlayerControls.Brake.performed += BrakeHandle;
        _inputActions.PlayerControls.Brake.canceled += BrakeHandle;
        _inputActions.PlayerControls.Sprint.started += SprintHandle;
        _inputActions.PlayerControls.Sprint.performed += SprintHandle;
        _inputActions.PlayerControls.Sprint.canceled += SprintHandle;
        _inputActions.PlayerControls.Look.started += LookHandle;
        _inputActions.PlayerControls.Look.performed += LookHandle;
        _inputActions.PlayerControls.Look.canceled += LookHandle;
        _inputActions.PlayerControls.Fire.started += FireHandle;
        _inputActions.PlayerControls.Fire.canceled += FireHandle;
        _inputActions.PlayerControls.HeavyFire.started += HeavyFireHandle;
        _inputActions.PlayerControls.HeavyFire.canceled += HeavyFireHandle;
        _inputActions.PlayerControls.SpecialFire.started += MagicFireHandle;
        _inputActions.PlayerControls.SpecialFire.canceled += MagicFireHandle;
        //_inputActions.Enable();
    }

    private void LookHandle(InputAction.CallbackContext context) => MousePosition = context.ReadValue<Vector2>();
    private void MoveHandle(InputAction.CallbackContext context) => MoveInput = context.ReadValue<Vector2>();
    private void DashHandle(InputAction.CallbackContext context) => DashInput = context.ReadValue<float>();
    private void SprintHandle(InputAction.CallbackContext context) => SprintInput = context.ReadValue<float>();
    private void BrakeHandle(InputAction.CallbackContext context) => BrakeInput = context.ReadValue<float>();
    private void FireHandle(InputAction.CallbackContext context) => FireInput = context.ReadValueAsButton();
    private void HeavyFireHandle(InputAction.CallbackContext context) => HeavyFireInput = context.ReadValueAsButton();
    private void MagicFireHandle(InputAction.CallbackContext context) => EliteFireInput = context.ReadValueAsButton();

    private void OnEnable() => _inputActions.Enable();

    private void OnDisable() => _inputActions.Disable();

    public bool IsMovePressed => MoveInput.sqrMagnitude > 0;
    public bool IsDashPressed => !Mathf.Approximately(DashInput, 0f);
    public bool IsSprintPressed => !Mathf.Approximately(SprintInput, 0f);
    public bool BrakePressed => !Mathf.Approximately(BrakeInput, 0f);
}