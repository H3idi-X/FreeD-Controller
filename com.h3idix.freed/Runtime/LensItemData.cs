using System;
using UnityEngine;

namespace h3idiX
{
    [Serializable]
    internal struct LensItemData 
    {
        [SerializeField] internal bool isActive;
        [SerializeField] internal int input;
        [SerializeField] internal float length;
    }
}