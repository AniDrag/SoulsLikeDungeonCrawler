using UnityEngine;
using UnityEngine.InputSystem;

namespace AniDrag.Player
{
    /// <summary>
    /// PlayerCamera handles first-person camera control for the player.
    /// 
    /// Responsibilities:
    /// - Reads mouse/gamepad input for camera rotation (look direction).
    /// - Applies vertical rotation (pitch) to the camera and horizontal rotation (yaw) to the player body.
    /// - Clamps vertical rotation to prevent camera from flipping.
    /// - Optionally applies smooth Field of View (FOV) changes from CameraSettings.
    /// - Locks and hides the cursor for FPS-style control.
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        [Header("========================\n" +
       "    References      \n" +
       "========================")]
        [Tooltip("The Camera component used for rendering the player's view.")]
        [SerializeField] private Camera playerCam;
        [Tooltip("The PlayerInput component for reading mouse/gamepad inputs.")]
        [SerializeField] private PlayerInput inputs;
        [Tooltip("Camera settings containing FOV and sensitivity.")]
        [SerializeField] private CameraSettings settings;
        [Tooltip("Transform representing the player's orientation (used for yaw rotation).")]
        [SerializeField] private Transform playerOrientation;
        [Tooltip("Transform that the camera should follow for position tracking.")]
        [SerializeField] private Transform camTrackPosition;

        [Header("========================\n" +
      "    Sensitivity      \n" +
      "========================")]
        [Tooltip("Base horizontal sensitivity multiplier.")]
        [SerializeField] private float baseHorizontalSensitivity = 1f;
        [Tooltip("Base vertical sensitivity multiplier.")]
        [SerializeField] private float baseVerticalSensitivity = 1f;

        [Header("========================\n" +
     "    Clamp      \n" +
     "========================")]
        [Tooltip("Maximum angle the camera can look up/down from horizontal (degrees).")]
        [SerializeField, Range(0, 90f)] private float verticalClamp = 85f;

        private float xRotation; // Current vertical rotation (pitch)
        private float yRotation; // Current horizontal rotation (yaw)
        private Vector2 lookDirection; // Input from mouse/gamepad

        /// <summary>
        /// Initialize camera references, field of view, and lock cursor for FPS control.
        /// </summary>
        private void Start()
        {
            // Assign main camera if none specified
            if (playerCam == null)
                playerCam = Camera.main;

            // Set initial FOV from settings
            if (settings != null)
                playerCam.fieldOfView = settings.FOV;

            // Lock and hide the cursor for FPS-style camera control
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        /// <summary>
        /// Updates camera rotation and position each frame.
        /// Handles input, clamping, and smooth FOV updates.
        /// </summary>
        private void Update()
        {
            if (inputs.actions["EnableMouse"].IsPressed())
            { 
                Debug.Log("Mouse enabled");
                settings.EnableCursor();
            }
            else if (inputs.actions["EnableMouse"].WasReleasedThisFrame())
            {
                Debug.Log("Mouse Dissable");
                settings.DisableCursor();
            }

            // Match camera position to tracking transform
            playerCam.transform.position = camTrackPosition.position;
            if(settings.isInMenu) return; // Skip camera control if in menu

            // Read look input from PlayerInput
            lookDirection = inputs.actions["Look"].ReadValue<Vector2>();
            if (lookDirection == Vector2.zero) return; // Skip if no input

            // Apply input to rotations with sensitivity scaling
            yRotation += lookDirection.x * baseHorizontalSensitivity * Time.deltaTime * settings.SensitivityHorizontal;
            xRotation -= lookDirection.y * baseVerticalSensitivity * Time.deltaTime * settings.SensitivityVertical;

            // Clamp vertical rotation to prevent flipping
            xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

            // Apply vertical rotation (pitch) to the camera only
            playerCam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

            // Apply horizontal rotation (yaw) to the player orientation
            if (playerOrientation != null)
                playerOrientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

            // Smoothly interpolate FOV if changed in settings
            if (Mathf.Abs(playerCam.fieldOfView - settings.FOV) > 0.01f)
                playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, settings.FOV, Time.deltaTime * 5f);
        }
    }
}