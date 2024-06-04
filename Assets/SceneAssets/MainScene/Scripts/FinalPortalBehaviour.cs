using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalPortalBehaviour : MonoBehaviour
{
    public ParticleSystem m_particles;
    public CircleCollider2D m_collider;
    
    public bool isFinished = false;

    float fadeinAlpha = 0f;
    void Update(){
        if (isFinished){
            // here be fadein and transition to ending scene
            GameManager.Instance.FadeInScreen.GetComponent<Image>().color = new Color(1f, 1f, 1f, fadeinAlpha);
            fadeinAlpha += 0.4f * Time.deltaTime;
            // fade in works, is good, now to transition
            if (fadeinAlpha > 1f){
                SceneManager.LoadScene("EndingScene");
            }
        }
    }

    public void ActivatePortal(){
        m_particles.Play();
        m_collider.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player") && GameManager.Instance.isPlayerActive){
            collision.GetComponent<CharacterController2D>().PlayerFinish();
            isFinished = true;
            GameManager.Instance.isInFocus = false;
            GameManager.Instance.FadeInScreen.SetActive(true);
        }
    }
}
