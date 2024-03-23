using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimationHandler : MonoBehaviour
{
    [SerializeField] private Transform _base;
    [SerializeField] private Transform _controller;
    [SerializeField] private InputActionProperty _grabAction;
    [SerializeField] private InputActionProperty _pinchAction;
    [SerializeField] private Animator _animator;

    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private Rigidbody _rigidbody;


    [SerializeField] private float _startPhysicsTrackingRadius = .2f;
    [SerializeField] private Vector3 _trackingOffset;
    [SerializeField, Range(0, 1)] private float _velocityDamp = .8f;
    [SerializeField, Range(0, 1)] private float _angularVelocityDamp = .8f;
    [SerializeField, Range(0, 300)] private float _maxPositionChange = 75f;
    [SerializeField, Range(0, 300)] private float _maxRotationChange = 75f;


    private void OnValidate()
    {
        if (!_rigidbody) _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody)
        {
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.useGravity = false;
        }
    }

    private void Update()
    {
        float grabValue = _grabAction.action.ReadValue<float>();
        _animator.SetFloat("Grip", grabValue);

        float pinchValue = _pinchAction.action.ReadValue<float>();
        _animator.SetFloat("Trigger", pinchValue);

        // Check if the hand is near any physics objects
        if (!Physics.CheckSphere(_base.position + _trackingOffset, _startPhysicsTrackingRadius, _collisionMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = _controller.position;
            transform.rotation = _controller.rotation;
        }

        Vector3 newVelocity = FindNewVelocity();
        if (IsValidVelocity(newVelocity.x))
        {
            float maxChange = _maxPositionChange * Time.deltaTime;
            _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, newVelocity, maxChange);
        }

        Quaternion delta = _controller.rotation * Quaternion.Inverse(transform.rotation);
        delta.ToAngleAxis(out float angle, out Vector3 axis);
        Vector3 newAngularVelocity = angle * axis * Mathf.Deg2Rad / Time.deltaTime;

        if (IsValidVelocity(newAngularVelocity.x))
        {
            float maxChange = _maxRotationChange * Time.deltaTime;
            _rigidbody.angularVelocity = Vector3.MoveTowards(_rigidbody.angularVelocity, newAngularVelocity, maxChange);
        }
    }


    private Vector3 FindNewVelocity()
    {
        return (_controller.position - transform.position) / Time.deltaTime;
    }

    private bool IsValidVelocity(float velocity)
    {
        return !float.IsNaN(velocity) && !float.IsInfinity(velocity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_base.position + _trackingOffset, _startPhysicsTrackingRadius);
    }
}
