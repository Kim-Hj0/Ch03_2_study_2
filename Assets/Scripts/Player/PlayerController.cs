using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Movement")]    //이동과 관련된 것.
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;

    private Vector2 mouseDelta; //민감도 설정.

    [HideInInspector]
    public bool canLook = true; //커서 안씀.

    private Rigidbody _rigidbody;

    public static PlayerController instance;    //싱글톤화.
    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;         //FPS 같은 경우, 커서가 있으면 안되겠죠. 그래서 락을 걸겠다.
    }

    private void FixedUpdate()  //물리적인 처리를 위해 사용함.
    {
        Move();
    }

    private void LateUpdate()   //모든 처리가 끝나고 동작함. 주로 카메라 작업에 많이 사용함.
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context)    //화면(시선)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)    //이동
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;    //캔슬이 났을 때는 움직이면 안되니까.
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)    //점프
    {
        if (context.phase == InputActionPhase.Started)  
        {
            if (IsGrounded())
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse); //질량을 가지고서 처리하는 것.

        }
    }

    private bool IsGrounded()   //땅을 밟고 있을 때만 점프할 수 있게 한다.
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f) , Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),   //충돌을 하는지 체크
        };

        for (int i = 0; i < rays.Length; i++)   //선형으로 이어져있는 아이들은Length, List처럼 뜨문뜨문인 얘들이 count
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))    //어디서 어느 방향으로
            {
                return true;    //4개 중에 하나라도 땅에 닿았으면 true, 점프해라.
            }
        }
        return false;
    }

    private void OnDrawGizmos() //선태했을 때만, wGizmos 볼 수 있게.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

}
