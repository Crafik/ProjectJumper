using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemySpawner : MonoBehaviour
{
    // Tag: EnemySpawner
    // Modes:
    // 0 - OnMove           - moving from side to side on a platform
    // 1 - Careless         - moving on platform ignoring pits, but not walls
    // 2 - Sleep            - waiting until player enters proxymity, then start OnMove in player's direction
    // 3 - Sentry           - waiting until player enters proxymity, then start Pursuit
    // 4 - Jumpy            - jumps in random intervals(or not so random)
    // 5 - Pursuit          - pursuits player, jumps over pits and walls
    public GameObject enemyGenericPrefab;

    [Tooltip("0 - OnMove\n1 - Careless\n2 - Sleep\n3 - Sentry\n4 - Jumpy\n5 - Pursuit")]
    public int EnemyMode;
    public int EnemyHealth;
    public float JumpInterval;
    public float jumpMultiplier;
    public bool FacingRight;

    public void SpawnEnemy(){
        GameObject enemy = Instantiate(enemyGenericPrefab, transform.position, Quaternion.identity);
        enemy.GetComponent<EnemyBehaviour>().EnemyInit(EnemyMode, EnemyHealth, FacingRight, JumpInterval, jumpMultiplier);
    }
}
