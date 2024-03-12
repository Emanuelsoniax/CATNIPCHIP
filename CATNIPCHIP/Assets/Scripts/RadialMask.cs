using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMask : MonoBehaviour
{
    [SerializeField] private float _baseRange = 0;
    [SerializeField] private float _targetRange = 2;
    [SerializeField] private float _time = 2;
    [SerializeField] private LeanTweenType _tweenType;

    private float _range;

    public float Range
    {
        get => _range;
        set { _range = value; }
    }

    private void Awake()
    {
        _range = _baseRange;
    }

    [ContextMenu("AnimateIn")]
    public void AnimateIn()
    {
        LeanTween.value(_range, _targetRange, _time).setEase(_tweenType).setOnUpdate(val => _range = val);
    }

    [ContextMenu("AnimateOut")]
    public void AnimateOut()
    {
        LeanTween.value(_range, _baseRange, _time).setEase(_tweenType).setOnUpdate(val => _range = val);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}
