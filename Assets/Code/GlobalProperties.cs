using System;
using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "Global Properties", menuName = "Scriptable Objects/Global Properties")]
    public class GlobalProperties : ScriptableObject
    {
        public static GlobalProperties Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GlobalProperties>("Global Properties");
                }
                return _instance;
            }
        }
 
        private static GlobalProperties _instance;

        public int KeyCount;
        public AnimationCurve CameraMovementCurve;
        public float CameraMovementDuration;
    }
}