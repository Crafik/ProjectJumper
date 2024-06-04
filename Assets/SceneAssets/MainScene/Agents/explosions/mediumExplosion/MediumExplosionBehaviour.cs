using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MediumExplosionBehaviour : MonoBehaviour
{
    public Animator m_anim;
    public CinemachineImpulseSource impulse;
    public AudioSource m_audio;

    void Start()
    {
        impulse.GenerateImpulse();
        
        m_audio.Play();

        // is not needed, but for clarity
        m_anim.Play("medium_explosion");
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player")){
            collision.gameObject.GetComponent<CharacterController2D>().PlayerDeath();
        }
    }

    void AnimFinished(){
        Destroy(gameObject);
    }
}
