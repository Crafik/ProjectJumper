using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // Maybe i should make multiple sources, so sounds don't cut eachother
    [SerializeField] private AudioSource playerStep;
    [SerializeField] private AudioSource playerJump;
    [SerializeField] private AudioSource playerShoot;
    [SerializeField] private AudioSource playerDeath;

    public void PlayStep(){
        if (playerStep.enabled)
            playerStep.Play();
    }
    public void DisableStep(){
        playerStep.enabled = false;
    }
    public void PlayJump(){
        playerJump.Play();
    }
    public void PlayShoot(){
        playerShoot.Play();
    }
    public void PlayDeath(){
        playerDeath.Play();
    }
}
