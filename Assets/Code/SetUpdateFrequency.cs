using UnityEngine;

namespace Code
{
    public class SetUpdateFrequency : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}