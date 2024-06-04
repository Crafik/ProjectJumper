using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SmallExplosionBehaviour : MonoBehaviour
{
    public Animator m_anim;
    public CinemachineImpulseSource impulse;
    public AudioSource m_audio;

    void Start()
    {
        impulse.GenerateImpulse();
        m_audio.Play();
        m_anim.Play("small_explosion");
    }

    void AnimFinished(){
        Destroy(gameObject);
    }
}
