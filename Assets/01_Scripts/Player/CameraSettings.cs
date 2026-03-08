using AniDrag.Utility;
using UnityEngine;

namespace AniDrag.Core
{
    [CreateAssetMenu(fileName = "CameraSettings", menuName = "AniDrag/Settings/CameraSettings")]
    public class CameraSettings : ScriptableObject
    {
        [Header("========================\n" +
     "    Camera Settings      \n" +
     "========================")]
        public float SensitivityHorizontal = 1;
        public float SensitivityVertical = 1;
        public float FOV = 60;

        public bool isInMenu = false;
        public int menuesActive = 0;
        [Button]
        public void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isInMenu = false;
        }
        [Button]
        public void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            isInMenu = true;
        }
        public void EnableMenuPanel()
        {
            menuesActive++;
            Debug.Log("Menues active: " + menuesActive);
            if (menuesActive > 1) return;
            EnableCursor();
        }
        public void DisableMenuPanel()
        {
            menuesActive--;
            Debug.Log("Menues active: " + menuesActive);
            if (menuesActive > 0) return;
            DisableCursor();


        }
    }
}