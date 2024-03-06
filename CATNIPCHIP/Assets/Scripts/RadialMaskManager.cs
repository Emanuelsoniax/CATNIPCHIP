using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CATNIP
{
    [ExecuteAlways]
    public class RadialMaskManager : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _range;

        private void Update()
        {
            Shader.SetGlobalVector("_RadialMask", new Vector4(_transform.position.x, _transform.position.y, _transform.position.z, _range));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(_transform.position, _range);
        }
    }
}
