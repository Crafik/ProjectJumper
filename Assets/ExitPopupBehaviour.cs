using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitPopupBehaviour : MonoBehaviour
{
    public Animator m_anim;
    public GameObject m_BG;

    public Button cancelButton;
    public Button exitButton;

    bool isClosing = false;
    int sceneID; // 0 - start; 1 - main; 2 - ending

    private Controls m_controls;

    void Awake(){
        m_controls = new Controls();
        sceneID = SceneManager.GetActiveScene().buildIndex;
        if (sceneID == 1){
            m_anim.Play("ExitPanelPopUp", 0, 1f);
        }
        cancelButton.Select();
    }

    void OnEnable(){
        m_controls.Enable();
        m_controls.Global.select_escape.performed += OnEscapePressed;
        m_controls.Global.select_left.performed += FindSelLeft;
        m_controls.Global.select_right.performed += FindSelRight;
        m_controls.Global.select_enter.performed += SelectedAction;
    }

    void OnDisable(){
        m_controls.Disable();
        m_controls.Global.select_escape.performed -= OnEscapePressed;
        m_controls.Global.select_left.performed -= FindSelLeft;
        m_controls.Global.select_right.performed -= FindSelRight;
        m_controls.Global.select_enter.performed -= SelectedAction;
    }

    public void PopupInit(GameObject bg){
        m_BG = bg;
    }
    
    void FindSelLeft(InputAction.CallbackContext ctx){
        cancelButton.Select();
    }

    void FindSelRight(InputAction.CallbackContext ctx){
        exitButton.Select();
    }

    public void SelectedAction(InputAction.CallbackContext ctx){
        // routine to press selected button by keyboard "Enter"
        // looks and feels super hacky
        // hope it won't explode on me someday

        if (EventSystem.current.currentSelectedGameObject == null){
            return;
        }
        var v_button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        v_button.onClick.Invoke();
    }

    bool isQuitting = false;
    public void OnExitClick(){
        if (!isQuitting){
            if (sceneID == 2){
                isQuitting = true;
                EndingManager.Instance.SetFadeInActive();
                m_anim.Play("ExitPanelPopUpReverse");
                StartCoroutine(ExitDelayed());
            }
            else{
                Application.Quit();
            }

            #if UNITY_EDITOR
            Debug.Log("Quitting game!");
            #endif
        }
    }

    private IEnumerator ExitDelayed(){
        yield return new WaitForSeconds(2.7f);
        Application.Quit();

        #if UNITY_EDITOR
        Debug.Log("Quitting game!");
        #endif
    }

    public void OnCloseClick(){
        if (sceneID == 1){
            Destroy(m_BG);
            GameManager.Instance.isInFocus = true;
            GameManager.Instance.GamePause(new InputAction.CallbackContext());
            Destroy(gameObject);
        }
        else{
            if (!isClosing){
                m_anim.Play("ExitPanelPopUpReverse");
                isClosing = true;
                StartCoroutine(ClosePopUp());
            }
        }
    }

    private IEnumerator ClosePopUp(){
        yield return new WaitForSeconds(0.34f);
        Destroy(m_BG);
        if (sceneID == 0){
            StartManager.Instance.isInFocus = true;
        }
        if (sceneID == 2){
            EndingManager.Instance.isInFocus = true;
        }
        Destroy(gameObject);
    }
    
    void OnEscapePressed(InputAction.CallbackContext ctx){
        OnCloseClick();
    }
}
