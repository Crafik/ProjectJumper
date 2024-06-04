using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FinalBossBehaviour : MonoBehaviour
{
    public FinalBossCoreBehaviour core1;
    public FinalBossCoreBehaviour core2;
    public FinalBossCoreBehaviour core3;
    public FinalBossCoreBehaviour core4;
    FinalBossCoreBehaviour[] coreArray;
    List<FinalBossCoreBehaviour> coreList;

    public CinemachineImpulseSource m_impulseSource;
    public AudioSource m_audioSource;

    public FinalGateLock finalGate;
    public FinalPortalBehaviour m_finalPortal;

    public GameObject explosionAttackPrefab;

    public bool isActive = false;

    int coresFunctional;
    float ATKallFunctional = 3f;
    float ATKthreeFunctional = 2.75f;
    float ATKtwoFunctional = 2.25f;
    float ATKoneFunctional = 2f;

    int currentOpenShutter = -1;

    public int coreHealth;

    void Awake(){
        CharacterController2D.OnPlayerDeath += PlayerDied;
        GameManager.OnGameRestart += GameRestart;
    }

    void Start(){
        coreArray = new FinalBossCoreBehaviour[4] {core1, core2, core3, core4};
        for (int i = 0; i < 4; i++){
            coreArray[i].num_ID = i;
            coreArray[i].health = coreHealth;
            coreArray[i].CoreReset();
        }
    }
    
    float attackCounter;
    float shutterSwitchCounter;
    float shutterSwitchDelayCounter;
    float destructionCounter = 2f;
    int destructionCycle = 0;
    void Update(){
        if (isActive){
            if (attackCounter < 0){
                switch (coresFunctional){
                    case 1:
                        attackCounter = ATKoneFunctional;
                    break;
                    case 2:
                        attackCounter = ATKtwoFunctional;
                    break;
                    case 3:
                        attackCounter = ATKthreeFunctional;
                    break;
                    case 4:
                        attackCounter = ATKallFunctional;
                    break;
                    default:
                        attackCounter = 999f;
                    break;
                }
                Instantiate(explosionAttackPrefab, transform);
            }
            attackCounter -= Time.deltaTime;

            if (shutterSwitchCounter < 0){
                shutterSwitchCounter = 4f + 1f * coresFunctional;
                SwitchShutters();
            }
            shutterSwitchCounter -= Time.deltaTime;
            shutterSwitchDelayCounter -= Time.deltaTime;
        }

        if (isDefeated && destructionCycle < 5){
            if (destructionCounter < 0f){
                m_audioSource.Play();
                m_impulseSource.GenerateImpulse();
                destructionCycle += 1;
                destructionCounter = 0.5f;
            }
            destructionCounter -= Time.deltaTime;
        }
        if (destructionCycle == 5){
            finalGate.GateSwitch();
            m_finalPortal.ActivatePortal();
            destructionCycle += 1;
        }
    }

    void SwitchShutters(){
        // random shutter choice????
        if (coresFunctional > 0){
            if (coresFunctional > 1){
                int rand = Random.Range(0,coresFunctional);
                if (currentOpenShutter == rand){
                    rand += rand == (coresFunctional - 1) ? -1 : 1;
                }

                coreList[rand].SwitchShutter();

                if (currentOpenShutter < 0){
                    currentOpenShutter = rand;
                    return;
                }
                else{
                    coreList[currentOpenShutter].SwitchShutter();
                    currentOpenShutter = rand;
                }
            }
            else{
                coreList[0].SwitchShutter();
            }
            shutterSwitchDelayCounter = 4f;
        }
    }

    public void StartBattle(){
        isActive = true;
        attackCounter = 4f;
        shutterSwitchCounter = 7f;
        currentOpenShutter = -1;
        coresFunctional = 4;
        coresDestroyed = 0;
        coreList = new List<FinalBossCoreBehaviour>(coreArray);
        for (int i = 0; i < 4; i++){
            // coreList[i].CoreReset(); Probably don't need this here
            coreList[i].num_ID = i;
            coreList[i].health = coreHealth;
        }
    }

    // both events is yet to be done
    void PlayerDied(){
        isActive = false;
    }

    void GameRestart(){
        isDefeated = false;
        destructionCounter = 0.5f;
        destructionCycle = 0;
    }

    #region defeat handling

    public void CoreGotDestroyed(int id){
        // need to think about core deletion from list
        for (int i = id; i < coresFunctional; i++){
            coreList[i].num_ID -= 1;
        }

        // it seems that this is not working properly in case of destroying a core when shutter is closing
        coresFunctional -= 1;
        if (currentOpenShutter == id){
            coreList[id].SwitchShutter();
            currentOpenShutter = -1;
        }
        else{
            if (currentOpenShutter >= id){
                currentOpenShutter -= 1;
                // problem occurs when currentOpenShutter is more or equal than id
                // this should work, test is needed
            }
        }
        coreList.RemoveAt(id);

        // overlap protection. kinda works
        if (shutterSwitchCounter > 0){
            if (shutterSwitchDelayCounter < 0){
                shutterSwitchCounter = -1f;
            }
        }
    }

    int coresDestroyed = 0;
    public void CoreIsDestroyed(){
        coresDestroyed += 1;
        if (coresDestroyed == 4){
            BossDeath();
        }
    }

    bool isDefeated = false;
    void BossDeath(){
        isActive = false;
        isDefeated = true;
    }

    #endregion
}
