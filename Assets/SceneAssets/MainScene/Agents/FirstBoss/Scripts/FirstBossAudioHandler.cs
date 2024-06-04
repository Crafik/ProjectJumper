using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossAudioHandler : MonoBehaviour
{
    public AudioSource bossHurt;
    public AudioSource bossLand;

    public AudioClip bossDeath;
    public AudioClip enemyHit;

    public void PlayHurt(){
        bossHurt.Play();
    }

    public void PlayLand(){
        bossLand.Play();
    }

    public void PlayDeath(){
        bossHurt.clip = bossDeath;
        bossHurt.volume *= 0.7f;

        bossLand.clip = enemyHit;
        
        bossLand.Play();
        bossHurt.Play();
    }
}
