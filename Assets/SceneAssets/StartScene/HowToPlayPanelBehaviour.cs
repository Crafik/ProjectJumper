using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HowToPlayPanelBehaviour : MonoBehaviour
{
    public Animator m_anim;
    public GameObject m_BG;

    bool isClosing = false;

    private Controls m_controls;

    void Awake(){
        m_controls = new Controls();
    }

    void OnEnable(){
        m_controls.Enable();
        m_controls.Global.select_escape.performed += OnEscapePressed;
    }

    void OnDisable(){
        m_controls.Disable();
        m_controls.Global.select_escape.performed -= OnEscapePressed;
    }

    public void PopupInit(GameObject bg){
        m_BG = bg;
    }

    public void OnCloseClick(){
        if (!isClosing){
            m_anim.Play("PopUpReverse");
            isClosing = true;
            StartCoroutine(ClosePopUp());
        }
    }

    private IEnumerator ClosePopUp(){
        yield return new WaitForSeconds(0.34f);
        Destroy(m_BG);
        StartManager.Instance.isInFocus = true;
        Destroy(gameObject);
    }
    
    void OnEscapePressed(InputAction.CallbackContext ctx){
        OnCloseClick();
    }
}
