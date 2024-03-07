using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CATNIP
{
    [ExecuteAlways]
    public class RadialMaskManager : MonoBehaviour
    {
        private const int MAX_RADIAL_MASKS = 8;

        [SerializeField] private RadialMask[] _masks;

        private Vector4[] _maskData = new Vector4[MAX_RADIAL_MASKS];

        private void Update()
        {
            if (_masks.Length == 0)
                return;

            int maskCount = 0;

            for (int i = 0; i < _masks.Length; i++)
            {
                if (i >= MAX_RADIAL_MASKS)
                    break;

                if (_masks[i] != null)
                {
                    Vector3 pos = _masks[i].transform.position;
                    _maskData[maskCount] = new Vector4(pos.x, pos.y, pos.z, _masks[i].Range);
                    maskCount++;
                }
            }

            Shader.SetGlobalVectorArray("_RadialMasks", _maskData);
            Shader.SetGlobalInt("_RadialMasksAmount", maskCount);

            //Shader.SetGlobalVector("_RadialMask", new Vector4(_transform.position.x, _transform.position.y, _transform.position.z, _range));
        }
    }
}
