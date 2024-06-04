using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class ConfineSetup : MonoBehaviour
{
    void Awake(){
        gameObject.layer = LayerMask.NameToLayer("confines");
    }
}
