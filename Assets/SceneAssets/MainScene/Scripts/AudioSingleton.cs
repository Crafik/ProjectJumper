using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSingleton : MonoBehaviour
{
    public static AudioSingleton Instance { get; private set; }

    public AudioClip pickupClip;

    [SerializeField] private AudioSource audioplayer;

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        }
        else{
            Instance = this;
        }
    }

    public void SetPickupSound(){
        audioplayer.clip = pickupClip;
    }

    public void SetVolume(float vol){
        audioplayer.volume = vol;
    }

    public void PlaySound(){
        audioplayer.Play();
    }
}
