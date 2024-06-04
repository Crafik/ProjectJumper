using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class debug_tool : MonoBehaviour
{
    private Controls controls;

    public GameObject gunPrefab;

    void Awake(){
        controls = new Controls();
    }

    void OnEnable(){
        controls.Enable();

        controls.debug.teleport_to_point.performed += OnTeleportPerformed;
        controls.debug.spawn_gun.performed += OnGunSpawnPerformed;
        controls.debug.teleport_to_ending.performed += OnToEndingPerformed;
    }

    void OnDisable(){
        controls.Disable();

        controls.debug.teleport_to_point.performed -= OnTeleportPerformed;
        controls.debug.spawn_gun.performed -= OnGunSpawnPerformed;
        controls.debug.teleport_to_ending.performed -= OnToEndingPerformed;
    }

    void OnTeleportPerformed(InputAction.CallbackContext ctx){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = this.transform.position;
    }

    void OnGunSpawnPerformed(InputAction.CallbackContext ctx){
        Instantiate(gunPrefab, transform);
    }

    void OnToEndingPerformed(InputAction.CallbackContext ctx){
        SceneManager.LoadScene(2);
    }
}
