using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract();
}


public class InteractionManager : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    private GameObject curInteractGameobject;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;
    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;   //main를 가지고 있는 카메라 하나만 자동적으로 잡히는 것(싱글톤 사용하는 것처럼)
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));    //카메라 정중앙에서 발사하겠다.
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask)) //hit에 충돌에 대한 정보가 넘어왔을 것이다.
            {
                if (hit.collider.gameObject != curInteractGameobject)    //우리가 저장했던 것과 다른 것이냐.
                {
                    curInteractGameobject = hit.collider.gameObject;    //다르다면 collider한다.
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();

                }
            }
            else //충돌한 게 아무것도 없다면.
            {
                curInteractGameobject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true); //prompt 알고있다는 전제하에.
        promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt());
    }

    public void OnInteractInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameobject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false); //우리가 알고 있는 정보들을 초기화.
        }
    }
}
