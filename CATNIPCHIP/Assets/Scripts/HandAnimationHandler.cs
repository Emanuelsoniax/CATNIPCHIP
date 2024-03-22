using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimationHandler : MonoBehaviour
{
    [SerializeField] private InputActionProperty _grabAction;
    [SerializeField] private InputActionProperty _pinchAction;
    [SerializeField] private Animator _animator;

    private void Update()
    {
        float grabValue = _grabAction.action.ReadValue<float>();
        _animator.SetFloat("Grip", grabValue);

        float pinchValue = _pinchAction.action.ReadValue<float>();
        _animator.SetFloat("Trigger", pinchValue);
    }
}
