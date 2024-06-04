using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GateBehaviour : MonoBehaviour
{
    public Sprite gateBlock;
    public Sprite gateBeam;
    public GameObject gateBlockPrefab;

    [Tooltip("Horizontal(Rightward) size of a gate from the point. [> 0]")]
    public int sizeX;
    [Tooltip("Vertical(Downward) size of a gate from the point. [> 0]")]
    public int sizeY;

    List<List<GameObject>> gateTiles;

    void Awake(){
        gateTiles = new List<List<GameObject>>();
    }

    public void BuildGate(){
        OpenGate();
        Vector2 currentPos = transform.position;
        for (int i = 0; i < sizeY; ++i){
            gateTiles.Add(new List<GameObject>());
            for (int j = 0; j < sizeX; ++j){
                gateTiles[i].Add(Instantiate(gateBlockPrefab, currentPos, Quaternion.identity));
                currentPos.x += 1f;
            }
            currentPos.x = transform.position.x;
            currentPos.y -= 1f;
        }
        RefreshSprites();
    }

    void RefreshSprites(){
        if (gateTiles.Count == 0){
            return;
        }
        if (gateTiles.Count > 1){
            for (int i = 0; i < sizeX; ++i){
                gateTiles[gateTiles.Count - 1][i].GetComponent<SpriteRenderer>().sprite = gateBlock;
            }
        }
        if (gateTiles.Count > 2){
            for (int i = 1; i < gateTiles.Count - 1; ++i){
                for (int j = 0; j < sizeX; ++j){
                    gateTiles[i][j].GetComponent<SpriteRenderer>().sprite = gateBeam;
                }
            }
        }
        for (int i = 0; i < sizeX; ++i){
            gateTiles[0][i].GetComponent<SpriteRenderer>().sprite = gateBlock;
        }
    }

    public void OpenGate(){
        for (int i = 0; i < gateTiles.Count; ++i){
            for (int j = 0; j < sizeX; ++j){
                Destroy(gateTiles[i][j]);
            }
        }
        gateTiles.Clear();
    }

    public void OpeningSequenceStep(){
        for (int i = 0; i < sizeX; ++i){
            Destroy(gateTiles[0][i]);
        }
        gateTiles.RemoveAt(0);

        RefreshSprites();
    }
}
