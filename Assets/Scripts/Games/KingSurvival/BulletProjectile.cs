using System.Collections;
using UnityEngine;


/// <summary>
/// Controls the projectile which is fired by the player
/// </summary>

public class BulletProjectile : MonoBehaviour
{
    Rigidbody bulletRigidbody;

    ScoreHandler scoreHandler;

    LineRenderer laser;

    AudioSource enemyDeathSound;

    [SerializeField] float speed = 0.5f;
    [SerializeField] float timeUntilEnemyIsCompletelyDestroyed = 5;

    public void OnInstantiated()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletRigidbody.velocity = transform.forward * speed;
        bulletRigidbody = GetComponent<Rigidbody>();
        laser = GameObject.Find("Laser").GetComponent<LineRenderer>(); // the bullet is instantiated from the assets, so we can't use the inspector to assign variables
        scoreHandler = GameObject.Find("SceneLoader").GetComponent<ScoreHandler>();
        enemyDeathSound = GameObject.Find("EnemyDeathSound").GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.transform.parent.GetComponent<MeshDestroyer>().DestroyMesh();
            StartCoroutine(DestroyBullet(other.gameObject));
            enemyDeathSound.Play();
            scoreHandler.score++;
            laser.SetPosition(1, transform.position + (transform.forward * 5000));
            GetComponent<MeshDestroyer>().DestroyMesh();
        }
        if(other.tag == "Border")
        {
            GetComponent<MeshDestroyer>().DestroyMesh();
        }
            
    }

    IEnumerator DestroyBullet(GameObject gameObject)
    {
        yield return new WaitForSeconds(timeUntilEnemyIsCompletelyDestroyed);
        Destroy(gameObject);
    }

}