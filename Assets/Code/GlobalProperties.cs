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

        public int KeyCountPerLevel;
        public int LevelsToCompleteCount;
        public AnimationCurve CameraMovementCurve;
        public AnimationCurve BulletFillCurve;
        public float CameraMovementDuration;
        public float BulletLaunchForceBase;
        public float BulletLaunchForceAdditional;
        public float BulletLifeDurationInSeconds;
        public float BulletPressToMaxPowerDurationInSeconds;
        public float EnemyToRoomRatio;
        public float TrapToRoomRatio;
    }
}