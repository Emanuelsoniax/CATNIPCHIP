using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMask : MonoBehaviour
{
    [SerializeField] private float _baseRange = 0;
    [SerializeField] private float _targetRange = 2;
    [SerializeField] private float _time = 2;
    [SerializeField] private LeanTweenType _tweenType = LeanTweenType.easeInOutCubic;

    private float _range;

    public float Range
    {
        get
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return _range;
            else 
                return _baseRange;
#else
            return _range;
#endif
        }
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.DrawWireSphere(transform.position, _baseRange);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
#endif
}
