using UnityEngine;

namespace AniDrag.Core
{
    [CreateAssetMenu(fileName = "CameraSettings", menuName = "Settings/CameraSettings")]
    public class CameraSettings : ScriptableObject
    {
        [Header("========================\n" +
                "    Camera Settings      \n" +
                "========================")]
        public float SensitivityHorizontal = 1;
        public float SensitivityVertical = 1;
        public float FOV = 60;
        public bool InvertVertical { get; set; }

        
    }
}