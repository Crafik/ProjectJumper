using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class CameraTrigger : MonoBehaviour
{
    // Actually a room trigger.
    // Got their name before i found out that manage room as a whole is better(i hope)
    // Won't rename because too lazy and such.
    public string roomName;
    
    Rigidbody2D body;
    BoxCollider2D box;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        body.bodyType = RigidbodyType2D.Static;
        gameObject.layer = LayerMask.NameToLayer("triggers");
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.CompareTag("Player") && GameManager.Instance.isPlayerActive){
            MainCameraBehaviour.Instance.switcher.Play(roomName);
            GameManager.Instance.currentActiveRoom = roomName;
            if (GameManager.Instance.currentRoomObject != transform.GetChild(0).gameObject){
                GameManager.Instance.currentRoomObject.GetComponent<RoomManager>().SetRoomActive(false);
                
                GameManager.Instance.currentRoomObject = transform.GetChild(0).gameObject;
                
                GameManager.Instance.currentRoomObject.GetComponent<RoomManager>().SetRoomActive(true);
            }
        }
        // Decided to do "optimizashun" the other way, hope it'll work
    }
}
