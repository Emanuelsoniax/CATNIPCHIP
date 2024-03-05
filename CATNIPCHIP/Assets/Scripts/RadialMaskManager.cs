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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Shader.SetGlobalVector("_RadialMask", new Vector4(_transform.position.x, _transform.position.y, _transform.position.z, _range));
        }
    }
}
