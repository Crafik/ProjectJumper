using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class HyperlinkPopupManager : MonoBehaviour
{
    // Looks ready, i'll leave it at that for now
    [SerializeField] private Button copyButton;
    [SerializeField] private Button openButton;

    [SerializeField] private TextMeshProUGUI m_URL;
    private GameObject m_BG;

    private Controls m_controls;

    void Awake(){
        m_controls = new Controls();

        copyButton.Select();
    }

    void OnEnable(){
        m_controls.Enable();

        m_controls.Global.select_left.performed += FindSelLeft;
        m_controls.Global.select_right.performed += FindSelRight;
        m_controls.Global.select_escape.performed += OnEscapePressed;
        m_controls.Global.select_enter.performed += SelectedAction;
    }

    void OnDisable(){
        m_controls.Disable();

        m_controls.Global.select_left.performed -= FindSelLeft;
        m_controls.Global.select_right.performed -= FindSelRight;
        m_controls.Global.select_escape.performed -= OnEscapePressed;
        m_controls.Global.select_enter.performed -= SelectedAction;
    }

    public void PopupInit(string URL, GameObject bg){
        m_URL.text = URL;
        m_BG = bg;
    }

    public void OnCopyClick(){
        GUIUtility.systemCopyBuffer = m_URL.text;
    }

    public void OnOpenClick(){
        Application.OpenURL(m_URL.text);
    }

    public void OnCloseClick(){
        Destroy(m_BG);
        StartManager.Instance.isInFocus = true;
        Destroy(gameObject);
    }

    void FindSelLeft(InputAction.CallbackContext ctx){
        openButton.Select();
    }

    void FindSelRight(InputAction.CallbackContext ctx){
        copyButton.Select();
    }

    void OnEscapePressed(InputAction.CallbackContext ctx){
        OnCloseClick();
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
        v_button.interactable = false;  // This implies that disabled and pressed colors are same. Not really good, but works for me, i don't have disabled buttons anyways
        StartCoroutine(ButtonHold(v_button));
    }

    private IEnumerator ButtonHold(Button v_button){
        yield return new WaitForSeconds(0.1f);
        v_button.interactable = true;
        // is not working properly
        // kinda. Need it to not reselect button on main screen when url popup pops
        v_button.Select();
    }
}
