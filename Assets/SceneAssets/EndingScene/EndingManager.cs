using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    public GameObject FadeOut;
    public GameObject FinText;

    public GameObject backgroundButtonPrefab;
    public GameObject exitPopupPrefab;
    public GameObject m_canvas;

    private float fadeOutAlpha = 1f;
    private bool isFadeOutActive = true;
    private bool isFadeInActive = false;

    private bool isFinActive = false;
    private float finFadeInAlpha = 0f;

    private GameObject backgroundButton;
    private GameObject currentPopup;

    private Controls m_controls;

    void Awake(){
        m_controls = new Controls();
        if (Instance != null && Instance != this){
            Destroy(this);
        }
        else{
            Instance = this;
        }

        FinText.SetActive(false);
    }

    private void OnEnable(){
        m_controls.Enable();

        m_controls.Global.select_escape.performed += OnEscapePressed;
    }

    private void OnDisable(){
        m_controls.Disable();

        m_controls.Global.select_escape.performed += OnEscapePressed;
    }

    void Update(){
        // FadeOut sequence seems to work fine
        if (isFadeOutActive){
            if (fadeOutAlpha > 0f){
                FadeOut.GetComponent<Image>().color = new Color(1f, 1f, 1f, fadeOutAlpha);
                fadeOutAlpha -= 0.4f * Time.deltaTime;
            }
            else{
                FadeOut.gameObject.SetActive(false);
                isFadeOutActive = false;
                isInFocus = true;
            }
        }
        if (isFinActive){
            if (finFadeInAlpha < 1f){
                FinText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, finFadeInAlpha);
                finFadeInAlpha += 0.4f * Time.deltaTime;
            }
            else{
                isFinActive = false;
            }
        }
        if (isFadeInActive){
            if (fadeOutAlpha < 1f){
                FadeOut.GetComponent<Image>().color = new Color(1f, 1f, 1f, fadeOutAlpha);
                fadeOutAlpha += 0.4f * Time.deltaTime;
            }
            else{
                isFadeInActive = false;
            }
        }
    }

    public void SetFinTextActive(){
        FinText.SetActive(true);
        isFinActive = true;
    }

    public void SetFadeInActive(){
        FadeOut.gameObject.SetActive(true);
        isFadeInActive = true;
    }

    public bool isInFocus = false;
    void OnEscapePressed(InputAction.CallbackContext ctx){
        if (isInFocus){
            isInFocus = false;
            backgroundButton = Instantiate(backgroundButtonPrefab, m_canvas.transform);
            currentPopup = Instantiate(exitPopupPrefab, m_canvas.transform);
            backgroundButton.GetComponent<Button>().onClick.AddListener(currentPopup.GetComponent<ExitPopupBehaviour>().OnCloseClick);
            currentPopup.GetComponent<ExitPopupBehaviour>().PopupInit(backgroundButton);
        }
    }
}
