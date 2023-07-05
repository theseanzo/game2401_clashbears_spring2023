using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInput))]
public class RTSCameraControlloer : MonoBehaviour
{
    public UnityEvent<Vector2> OnCameraMove;
    public UnityEvent<float> OnCameraRotate;
    public UnityEvent<float> OnCameraZoom;

    private PlayerInput mPlayerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        mPlayerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableInput(bool enable)
    {
        if(enable)
        {
            mPlayerInput.ActivateInput();
        } else
        {
            mPlayerInput.DeactivateInput();
        }
    }

    public void Zoom(InputAction.CallbackContext ctx)
    {
        OnCameraZoom?.Invoke(ctx.ReadValue<float>());
    }

    public void Rotate(InputAction.CallbackContext ctx)
    {
        OnCameraRotate?.Invoke(ctx.ReadValue<float>());
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        OnCameraMove?.Invoke(ctx.ReadValue<Vector2>());
    }
}
