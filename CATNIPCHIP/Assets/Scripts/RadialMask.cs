using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMask : MonoBehaviour
{
    [SerializeField] private float _range;

    public float Range
    {
        get => _range;
        set { _range = value; }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}
