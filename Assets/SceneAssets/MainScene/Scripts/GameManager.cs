using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    // GameManager:
    // 1. Start of the level
    // 2. Saving game
    // 3. Loading of saved game
    // << Maybe i'll do a checkpoint system without permanent saves, if game won't become big enough >>
    // 4. Reload after death
    //
    // 5. Management of GUI

    // This whole script is changing into singleton, cause imdumb and should've done that in the first place

    // ToDo: Key and unlockable doors(would be best if implemented with tileset, but dunno)

    public delegate void GameRestartEvent();
    public static event GameRestartEvent OnGameRestart;

    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private GameObject PausedScreen;
    public GameObject FadeInScreen;
    [SerializeField] private GameObject Camera;

    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject Startpoint;

    [SerializeField] private GameObject MainCamera;

    public GameObject backgroundButtonPrefab;
    public GameObject exitPopupPrefab;
    public GameObject m_canvas;

    private Controls controls;

    public GameObject Player;

    #region Global variables

    bool isGameActive;
    public bool isPlayerActive;
    public bool isPlayerGunActive;

    public GameObject currentSpawnpoint;
    private GameObject savedRoomObject;
    private string currentSavedRoom;

    public string currentActiveRoom;
    public GameObject currentRoomObject;
    private bool spawnPlayerGun;

    #endregion

    public GameObject smallExplosionPrefab;
    public GameObject mediumExplosionPrefab;

    void Awake(){
        controls = new Controls();
        // Should not be a problem at all, but internet says it's a right thing to do
        if (Instance != null && Instance != this){
            Destroy(this);
        }
        else{
            Instance = this;
        }
    }

    void Start(){
        // Looks horrible

        // This is a silly attempt to "optimazashun"
        // probably fine, not like it'll lead to problems
        GameObject[] cannons = GameObject.FindGameObjectsWithTag("Cannon");
        foreach (GameObject cannon in cannons){
            cannon.SetActive(false);
        }

        GameOverScreen.SetActive(false);
        PausedScreen.SetActive(false);
        FadeInScreen.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0);
        FadeInScreen.SetActive(false);
        isPlayerActive = true;
        currentSpawnpoint = Startpoint;
        savedRoomObject = Startpoint.transform.parent.gameObject;
        currentRoomObject = savedRoomObject;
        currentActiveRoom = "startroom";
        currentSavedRoom = currentActiveRoom;
        Player = Instantiate(PlayerPrefab, Startpoint.transform.GetChild(0).transform.position, Quaternion.identity);
        GunSpawnpointBehaviour.Instance.SpawnGunPickup();
        currentRoomObject.GetComponent<RoomManager>().SetRoomActive(true);
        OnGameRestart();

        CharacterController2D.OnPlayerDeath += GameOver;
        SpawnpointBehaviour.OnCheckpointEnter += CheckpointEntered;
    }

    private void OnEnable(){
        controls.Enable();

        controls.Player.Restart.performed += GameRestart;
        controls.Global.Pause.performed += GamePause;
        controls.Global.select_escape.performed += OnEscapePressed;
    }

    private void OnDisable(){
        controls.Disable();
        
        controls.Player.Restart.performed -= GameRestart;
        controls.Global.Pause.performed -= GamePause;
        controls.Global.select_escape.performed += OnEscapePressed;
    }

    void CheckpointEntered(GameObject CP){
        currentSpawnpoint.GetComponent<SpawnpointBehaviour>().SetSpawnpointInactive();
        currentSavedRoom = currentActiveRoom;
        currentSpawnpoint = CP;
        if (savedRoomObject != CP.transform.parent.gameObject){
            savedRoomObject = CP.transform.parent.gameObject;
        }
        spawnPlayerGun = isPlayerGunActive;
    }

    public bool isInFocus = true;
    public bool isPaused = false;
    public void GamePause(InputAction.CallbackContext ctx){
        if (isInFocus){
            if (isPaused){
                isPaused = false;
                Time.timeScale = 1f;
                PausedScreen.SetActive(false);
            }
            else{
                isPaused = true;
                Time.timeScale = 0f;
                PausedScreen.SetActive(true);
            }
        }
    }

    GameObject backgroundButton;
    GameObject currentPopup;
    void OnEscapePressed(InputAction.CallbackContext ctx){
        if (isInFocus){
            if (!isPaused){
                GamePause(new InputAction.CallbackContext());
            }
            isInFocus = false;
            backgroundButton = Instantiate(backgroundButtonPrefab, m_canvas.transform);
            currentPopup = Instantiate(exitPopupPrefab, m_canvas.transform);
            backgroundButton.GetComponent<Button>().onClick.AddListener(currentPopup.GetComponent<ExitPopupBehaviour>().OnCloseClick);
            currentPopup.GetComponent<ExitPopupBehaviour>().PopupInit(backgroundButton);
            // troubles, fix here
        }
    }
    
    void GameRestart(InputAction.CallbackContext ctx){
        if (!isPlayerActive){
            // Here be respawn
            Destroy(Player);
            Player = Instantiate(PlayerPrefab, currentSpawnpoint.transform.GetChild(0).transform.position, Quaternion.identity);
            
            MainCamera.GetComponent<CinemachineBrain>().enabled = true;
            MainCameraBehaviour.Instance.switcher.Play(currentSavedRoom);
            currentActiveRoom = currentSavedRoom;

            GameOverScreen.SetActive(false);
            CharacterController2D plrContr = Player.GetComponent<CharacterController2D>();
            plrContr.SetGunActive(spawnPlayerGun);
            isPlayerActive = true;

            if (!spawnPlayerGun && !GunSpawnpointBehaviour.Instance.PickUpExists){
                GunSpawnpointBehaviour.Instance.SpawnGunPickup();
            }

            savedRoomObject.GetComponent<RoomManager>().SetRoomActive(true);

            GameObject[] cannonballs = GameObject.FindGameObjectsWithTag("EnemyProjectile");
            foreach (GameObject obj in cannonballs){
                Destroy(obj);
            }

            OnGameRestart();
        }
    }

    void GameOver(){
        GameOverScreen.SetActive(true);
        isPlayerActive = false;
        // Super hacky(IMO) way to implement camera freeze, but works for me.
        DisableFollow();
    }

    public void DisableFollow(){
        MainCamera.GetComponent<CinemachineBrain>().enabled = false;
    }
}
