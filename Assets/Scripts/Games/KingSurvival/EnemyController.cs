using UnityEngine;

/// <summary>
/// Controls the movement of the enemy
/// </summary>

public class EnemyController : MonoBehaviour
{
   public GameObject player;
   public GameObject sceneLoader;

   [SerializeField] float speed = 0.005f;
   [SerializeField] float deathDistance = 0.057f; // if the distance between the player and the enemy is less than this, the player dies

   DeathHandler deathHandler;
   GameStarter gameStarter;

   void Start()
   {
      gameStarter = sceneLoader.GetComponent<GameStarter>();
      deathHandler = sceneLoader.GetComponent<DeathHandler>();
   }
    
   void Update()
   {
      if(gameStarter.isPlaying)
      {
         if(Vector3.Distance(transform.position,player.transform.position)>deathDistance)
         {
            transform.position = Vector3.MoveTowards(transform.position,player.transform.position,speed); 
            transform.LookAt(player.transform.position);
         }
         else
         {
            deathHandler.Death();
         }
      }
   }

}