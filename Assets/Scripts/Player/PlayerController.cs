using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Movement")]    //�̵��� ���õ� ��.
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

    private Vector2 mouseDelta; //�ΰ��� ����.

    [HideInInspector]
    public bool canLook = true; //Ŀ�� �Ⱦ�.

    private Rigidbody _rigidbody;

    public static PlayerController instance;    //�̱���ȭ.
    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;         //FPS ���� ���, Ŀ���� ������ �ȵǰ���. �׷��� ���� �ɰڴ�.
    }

    private void FixedUpdate()  //�������� ó���� ���� �����.
    {
        Move();
    }

    private void LateUpdate()   //��� ó���� ������ ������. �ַ� ī�޶� �۾��� ���� �����.
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

    public void OnLookInput(InputAction.CallbackContext context)    //ȭ��(�ü�)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)    //�̵�
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;    //ĵ���� ���� ���� �����̸� �ȵǴϱ�.
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)    //����
    {
        if (context.phase == InputActionPhase.Started)  
        {
            if (IsGrounded())
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse); //������ ������ ó���ϴ� ��.

        }
    }

    private bool IsGrounded()   //���� ��� ���� ���� ������ �� �ְ� �Ѵ�.
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f) , Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),   //�浹�� �ϴ��� üũ
        };

        for (int i = 0; i < rays.Length; i++)   //�������� �̾����ִ� ���̵���Length, Listó�� �߹��߹��� ����� count
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))    //��� ��� ��������
            {
                return true;    //4�� �߿� �ϳ��� ���� ������� true, �����ض�.
            }
        }
        return false;
    }

    private void OnDrawGizmos() //�������� ����, wGizmos �� �� �ְ�.
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
