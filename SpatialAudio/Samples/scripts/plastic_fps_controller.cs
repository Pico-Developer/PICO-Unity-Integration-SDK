using System;
using UnityEngine;

public class plastic_fps_controller : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;

    [SerializeField] private float mouseSensitivity = 3.5f;
    
    private float _cameraPitch = 0.0f;
    private float _cameraYaw = 0.0f;

    private bool _isCursorLocked = false;
    
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetCursorLock(true);
            _isCursorLocked = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isCursorLocked)
            {
                SetCursorLock(false);
                _isCursorLocked = false;
            }
            else
                Application.Quit(0);
        }

        if (_isCursorLocked)
        {
            UpdateRotation();
            UpdatePosition();
        }
    }

    void UpdateRotation()
    {
        Vector2 targetMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
        _cameraPitch = -targetMousePos.y * mouseSensitivity;
        _cameraYaw = targetMousePos.x * mouseSensitivity;

        transform.eulerAngles += new Vector3(_cameraPitch, _cameraYaw, 0.0f);
    }
    
    void UpdatePosition()
    {
        //  Calculate movement distance
        float movementDistance = speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementDistance *= 2.0f;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            movementDistance /= 5.0f;
        }
            
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            movement.x = -movementDistance;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x = movementDistance;
        }
        if (Input.GetKey(KeyCode.W))
        {
            movement.z = movementDistance;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.z = -movementDistance;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            movement.y = movementDistance;
        }
        if (Input.GetKey(KeyCode.E))
        {
            movement.y = -movementDistance;
        }

        transform.position += transform.rotation * movement;
    }

    private void OnDestroy()
    {
        SetCursorLock(false);
    }

    private void SetCursorLock(bool lockCursor) {
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
