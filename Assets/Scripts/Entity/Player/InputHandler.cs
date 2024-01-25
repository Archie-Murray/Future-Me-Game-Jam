using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class InputHandler : MonoBehaviour {
    public Vector2 MoveInput;
    public Vector2 MousePosition;
    public Vector3 WorldMousePosition;
    public float DashInput;
    public float SprintInput;
    public float BrakeInput;
    public bool FireInput;
    public bool HeavyFireInput;
    public bool EliteFireInput;

    private Plane _plane = new Plane(Vector3.up, 0f);
    private Ray _ray;

    public void Start() {
        Globals.Instance.Controls.PlayerControls.Move.started += MoveHandle;
        Globals.Instance.Controls.PlayerControls.Move.performed += MoveHandle;
        Globals.Instance.Controls.PlayerControls.Move.canceled += MoveHandle;
        Globals.Instance.Controls.PlayerControls.Dash.started += DashHandle;
        Globals.Instance.Controls.PlayerControls.Dash.performed += DashHandle;
        Globals.Instance.Controls.PlayerControls.Dash.canceled += DashHandle;
        Globals.Instance.Controls.PlayerControls.Brake.started += BrakeHandle;
        Globals.Instance.Controls.PlayerControls.Brake.performed += BrakeHandle;
        Globals.Instance.Controls.PlayerControls.Brake.canceled += BrakeHandle;
        Globals.Instance.Controls.PlayerControls.Sprint.started += SprintHandle;
        Globals.Instance.Controls.PlayerControls.Sprint.performed += SprintHandle;
        Globals.Instance.Controls.PlayerControls.Sprint.canceled += SprintHandle;
        Globals.Instance.Controls.PlayerControls.Look.started += LookHandle;
        Globals.Instance.Controls.PlayerControls.Look.performed += LookHandle;
        Globals.Instance.Controls.PlayerControls.Look.canceled += LookHandle;
        Globals.Instance.Controls.PlayerControls.Fire.started += FireHandle;
        Globals.Instance.Controls.PlayerControls.Fire.canceled += FireHandle;
        Globals.Instance.Controls.PlayerControls.HeavyFire.started += HeavyFireHandle;
        Globals.Instance.Controls.PlayerControls.HeavyFire.canceled += HeavyFireHandle;
        Globals.Instance.Controls.PlayerControls.SpecialFire.started += MagicFireHandle;
        Globals.Instance.Controls.PlayerControls.SpecialFire.canceled += MagicFireHandle;
    }
    private void LookHandle(InputAction.CallbackContext context) { 
        MousePosition = context.ReadValue<Vector2>();
        _ray = Globals.Instance.MainCamera.ScreenPointToRay(MousePosition);
        if (_plane.Raycast(_ray, out float distance)) {
            WorldMousePosition = _ray.GetPoint(distance);
        } 
    }
    private void MoveHandle(InputAction.CallbackContext context) => MoveInput = context.ReadValue<Vector2>();
    private void DashHandle(InputAction.CallbackContext context) => DashInput = context.ReadValue<float>();
    private void SprintHandle(InputAction.CallbackContext context) => SprintInput = context.ReadValue<float>();
    private void BrakeHandle(InputAction.CallbackContext context) => BrakeInput = context.ReadValue<float>();
    private void FireHandle(InputAction.CallbackContext context) => FireInput = context.ReadValueAsButton();
    private void HeavyFireHandle(InputAction.CallbackContext context) => HeavyFireInput = context.ReadValueAsButton();
    private void MagicFireHandle(InputAction.CallbackContext context) => EliteFireInput = context.ReadValueAsButton();

    public bool IsMovePressed => MoveInput.sqrMagnitude > 0;
    public bool IsDashPressed => !Mathf.Approximately(DashInput, 0f);
    public bool IsSprintPressed => !Mathf.Approximately(SprintInput, 0f);
    public bool BrakePressed => !Mathf.Approximately(BrakeInput, 0f);
    
}