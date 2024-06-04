using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public static StartManager Instance;

    // double prefab for background click functionality works just fine
    // even tho it's not a nicest look
    public GameObject backgroundButtonPrefab;
    public GameObject hyperlinkPrefab;
    public GameObject howToPlayPrefab;
    public GameObject exitPopupPrefab;
    public GameObject m_canvas;

    public GameObject fadeIn;
    bool isFadeInStarted;

    // Here be buttons
    public Button left1;
    public Button left2;
    public Button left3;
    public Button left4;
    public Button left5;
    public Button right1;
    public Button right2;
    public Button right3;
    Button[] leftButtons = new Button[5];
    Button[] rightButtons = new Button[3];
    int currentLeftButton;
    int currentRightButton;
    bool currentSide; // true for right

    private GameObject backgroundButton;
    private GameObject currentPopup;

    private Controls m_controls;

    public bool isInFocus;

    void Awake(){
        m_controls = new Controls();
        // Should not be a problem at all, but internet says it's a right thing to do
        if (Instance != null && Instance != this){
            Destroy(this);
        }
        else{
            Instance = this;
        }
        fadeIn.SetActive(false);
        isFadeInStarted = false;
        isInFocus = true;
    }

    void Start(){
        leftButtons[0] = left1;
        leftButtons[1] = left2;
        leftButtons[2] = left3;
        leftButtons[3] = left4;
        leftButtons[4] = left5;
        rightButtons[0] = right1;
        rightButtons[1] = right2;
        rightButtons[2] = right3;
        currentLeftButton = 0;
        currentRightButton = 0;
        currentSide = true;
        #if DEVELOPMENT_BUILD
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
        Debug.LogError("Dev console is active!");
        #endif
    }

    float fadeInAlpha = 0f;
    void Update(){
        if (isFadeInStarted){
            fadeIn.GetComponent<Image>().color = new Color(1f, 1f, 1f, fadeInAlpha);
            fadeInAlpha += 0.4f * Time.deltaTime;
        }
        if (fadeInAlpha > 1f){
            SceneManager.LoadScene("MainScene");
        }
    }

    void OnEnable(){
        m_controls.Enable();

        m_controls.Global.select_left.performed += FindSelLeft;
        m_controls.Global.select_right.performed += FindSelRight;
        m_controls.Global.select_up.performed += FindSelUp;
        m_controls.Global.select_down.performed += FindSelDown;
        m_controls.Global.select_enter.performed += SelectedAction;
    }

    void OnDisable(){
        m_controls.Disable();

        m_controls.Global.select_left.performed -= FindSelLeft;
        m_controls.Global.select_right.performed -= FindSelRight;
        m_controls.Global.select_up.performed -= FindSelUp;
        m_controls.Global.select_down.performed -= FindSelDown;
        m_controls.Global.select_enter.performed -= SelectedAction;
    }

    public void OnHyperlinkClick(string URL){
        backgroundButton = Instantiate(backgroundButtonPrefab, m_canvas.transform);
        currentPopup = Instantiate(hyperlinkPrefab, m_canvas.transform);
        backgroundButton.GetComponent<Button>().onClick.AddListener(currentPopup.GetComponent<HyperlinkPopupManager>().OnCloseClick);
        currentPopup.GetComponent<HyperlinkPopupManager>().PopupInit(URL, backgroundButton);
        isInFocus = false;
    }

    public void OnExitClick(){
        backgroundButton = Instantiate(backgroundButtonPrefab, m_canvas.transform);
        currentPopup = Instantiate(exitPopupPrefab, m_canvas.transform);
        backgroundButton.GetComponent<Button>().onClick.AddListener(currentPopup.GetComponent<ExitPopupBehaviour>().OnCloseClick);
        currentPopup.GetComponent<ExitPopupBehaviour>().PopupInit(backgroundButton);
        isInFocus = false;
    }

    public void OnHowToPlayClick(){
        backgroundButton = Instantiate(backgroundButtonPrefab, m_canvas.transform);
        currentPopup = Instantiate(howToPlayPrefab, m_canvas.transform);
        backgroundButton.GetComponent<Button>().onClick.AddListener(currentPopup.GetComponent<HowToPlayPanelBehaviour>().OnCloseClick);
        currentPopup.GetComponent<HowToPlayPanelBehaviour>().PopupInit(backgroundButton);
        isInFocus = false;
    }

    public void OnStartClick(){
        isInFocus = false;
        isFadeInStarted = true;
        fadeIn.SetActive(true);
    }

    // here be navigation
    // could have made this into pad, but too lazy to redo
    void FindSelLeft(InputAction.CallbackContext ctx){
        if (isInFocus){
            currentSide = false;
            VerticalSel();
        }
    }

    void FindSelRight(InputAction.CallbackContext ctx){
        if (isInFocus){
            currentSide = true;
            VerticalSel();
        }
    }

    void FindSelUp(InputAction.CallbackContext ctx){
        if (isInFocus){
            if (currentSide){
                if (currentRightButton != 0){
                    currentRightButton -= 1;
                }
            }
            else{
                if (currentLeftButton != 0){
                    currentLeftButton -= 1;
                }
            }
            VerticalSel();
        }
    }

    void FindSelDown(InputAction.CallbackContext ctx){
        if (isInFocus){
            if (currentSide){
                if (currentRightButton != 2){
                    currentRightButton += 1;
                }
            }
            else{
                if (currentLeftButton != 4){
                    currentLeftButton += 1;
                }
            }
            VerticalSel();
        }
    }

    void VerticalSel(){
        if (currentSide){
            rightButtons[currentRightButton].Select();
        }
        else{
            leftButtons[currentLeftButton].Select();
        }
    }

    public void SelectedAction(InputAction.CallbackContext ctx){
        // routine to press selected button by keyboard "Enter"
        // looks and feels super hacky
        // hope it won't explode on me someday
        if (isInFocus){
            if (EventSystem.current.currentSelectedGameObject == null){
                return;
            }
            var v_button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            v_button.onClick.Invoke();
            v_button.interactable = false;  // This implies that disabled and pressed colors are same. Not really good, but works for me, i don't have disabled buttons anyways
            StartCoroutine(ButtonHold(v_button));
        }
    }

    private IEnumerator ButtonHold(Button v_button){
        yield return new WaitForSeconds(0.1f);
        v_button.interactable = true;
        // is not working properly
        // kinda. Need it to not reselect button on main screen when url popup pops
        // Copypasted this into popup. Uber band-aid, but works :(
        if (currentPopup == null){
            v_button.Select();
        }
    }
}
