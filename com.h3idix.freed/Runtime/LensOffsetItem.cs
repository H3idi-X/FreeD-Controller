using System;
using UnityEngine;

namespace h3idiX
{
    [Serializable]
    internal struct LensOffsetItem 
    {
        [SerializeField] internal bool isActive;
        [SerializeField] internal int input;
        [SerializeField] internal Vector3 offset;
    }
}