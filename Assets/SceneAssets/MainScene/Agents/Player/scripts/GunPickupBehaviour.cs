using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.tag == "Player"){
            collision.gameObject.GetComponent<CharacterController2D>().SetGunActive(true);
            GunSpawnpointBehaviour.Instance.PickUpExists = false;
            AudioSingleton.Instance.SetPickupSound();
            AudioSingleton.Instance.SetVolume(0.7f);
            AudioSingleton.Instance.PlaySound();
            Destroy(transform.parent.gameObject);
        }
    }
}
