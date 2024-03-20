using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CATNIP
{
    [Serializable]
    public class RadialMaskManager
    {
        private const int MAX_RADIAL_MASKS = 8;

        [SerializeField, Tooltip("Limited to 8 masks")] private RadialMask[] _masks = new RadialMask[8];

        private Vector4[] _maskData = new Vector4[MAX_RADIAL_MASKS];

        public event Action onMaskEngulved;

        public void AnimatePrimaryMask(float target, float time, Action onAnimated) => AnimateMask(0, target, time, onAnimated);

        public void AnimateMask(int index, float target, float time, Action onAnimated)
        {
            if(!TryGetMask(index, out RadialMask mask))
            {
                Debug.LogWarning($"No mask with index {index} found");
                return;
            }

            mask.Animate(target, time);
        }

        public bool TryGetPrimaryMask(out RadialMask mask) => TryGetMask(0, out mask);

        public bool TryGetMask(int index, out RadialMask mask)
        {
            if (index < 0 || index >= _masks.Length)
            {
                mask = null;
                return false;
            }

            mask = _masks[index];
            if (mask == null)
                return false;

            return true;
        }


        public void UpdateShaderProperties()
        {
            if (_masks.Length == 0)
                return;

            int maskCount = 0;

            for (int i = 0; i < _masks.Length; i++)
            {
                if (i >= MAX_RADIAL_MASKS)
                    break;

                if (_masks[i] != null && _masks[i].Range > 0)
                {
                    Vector3 pos = _masks[i].transform.position;
                    _maskData[maskCount] = new Vector4(pos.x, pos.y, pos.z, _masks[i].Range);
                    maskCount++;
                }
            }

            Shader.SetGlobalVectorArray("_RadialMasks", _maskData);
            Shader.SetGlobalInt("_RadialMasksAmount", maskCount);
        }
    }
}
