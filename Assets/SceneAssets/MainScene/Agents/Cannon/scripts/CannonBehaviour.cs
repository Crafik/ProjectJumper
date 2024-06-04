using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject cannonballPrefab;

    public float cannonballSpeed;
    public float cooldown;
    private float CDcounter;
    
    [SerializeField] private GameObject point;
    public AudioSource audioplayer;

    void Update()
    {
        if (CDcounter < 0){
            Shoot();
            CDcounter = cooldown;
        }
        CDcounter -= Time.deltaTime;
    }

    private void Shoot(){
        GameObject ball = Instantiate(cannonballPrefab, point.transform);
        ball.GetComponent<CannonballBehaviour>().Init(cannonballSpeed);
        audioplayer.Play();
    }
}
