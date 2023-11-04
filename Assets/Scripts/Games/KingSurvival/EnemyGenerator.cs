using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Spawns the enemies around the player
/// </summary>

public class EnemyGenerator : MonoBehaviour
{
    public GameObject enemyModel;
    public GameObject player;

    [SerializeField] Transform groundPlaneStage;

    [SerializeField] GameStarter gameStarter;

    [SerializeField] float distance;
    [SerializeField] float timeUntilNextEnemyIsSpawned;
    float period;

    void Update()
    {
        if(gameStarter.isPlaying)
        {
            if(period > timeUntilNextEnemyIsSpawned)
            {
                EnemyGeneratorFunction();
                period = 0;
            }
            period += UnityEngine.Time.deltaTime;
        }
    }

    void EnemyGeneratorFunction()
    {
        GameObject newEnemy = GameObject.Instantiate(enemyModel);
        newEnemy.GetComponent<EnemyController>().player = player;
        newEnemy.GetComponent<EnemyController>().sceneLoader = this.gameObject;
        newEnemy.SetActive(true);
        newEnemy.transform.parent = groundPlaneStage;
        newEnemy.transform.localPosition = RandomPositionAwayFromPlayer();
        newEnemy.transform.name = "Enemy";
        
    }

    Vector3 RandomPositionAwayFromPlayer()
    {
        float angle = UnityEngine.Random.Range(0f,360f) * Mathf.Deg2Rad;
        Vector3 delta = new Vector3(Mathf.Cos( angle ) * distance, 0, Mathf.Sin( angle ) * distance);
        return player.transform.localPosition + delta;
    }
}