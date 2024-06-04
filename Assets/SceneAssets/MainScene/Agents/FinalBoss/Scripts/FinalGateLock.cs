using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FinalGateLock : MonoBehaviour
{
    public GateBehaviour m_gate;

    private bool isActivated;
    private bool isActive;

    void Awake(){
        isActive = true;
        isActivated = false;

        GameManager.OnGameRestart += GateReset;
        SpawnpointBehaviour.OnCheckpointEnter += StateSwitch;
    }

    public void GateSwitch(){
        m_gate.OpenGate();
        isActivated = true;
    }

    void GateReset(){
        if (isActive){
            m_gate.BuildGate();
            isActivated = false;
        }
    }

    void StateSwitch(GameObject omitted){
        if (isActivated){
            isActive = false;
        }
    }
}
