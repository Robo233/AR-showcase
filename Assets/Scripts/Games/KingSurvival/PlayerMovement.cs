using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the movement of the king, like moving, turning and shooting
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    public Transform Bullet;
    [SerializeField] Transform spawnBulletPosition;

    public AudioSource emptyGunShot;
    public AudioSource gunShot;

    [SerializeField] Color unloadedGunColor = new Color(83f / 255f, 168f / 255f, 202f / 255f, 1);
    [SerializeField] Color loadedGunColor = new Color(56f / 255f, 52f / 255f, 87f / 255f, 1);


    public Image shootButtonImage;

    [SerializeField] float moveSpeed = 0.2f;
    [SerializeField] float shootCooldownTime = 3; // the amount of time, after a shot, the player has to wait before shooting again

    public bool isReloaded = true;
    public bool isRunning;
    public bool isPlayerPlaced;
    bool goodPositionWasSet;
    bool wasCameraRotationAdded;

    [SerializeField] CharacterController controller;
    public GameStarter gameStarter;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        spawnBulletPosition = transform.GetChild(1);
    }

    void Update(){ // the king is rotated towards the pressed point on the screen
        if(gameStarter.isPlaying)
        {
            if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector3 clickedPosition = new Vector3();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    if(Physics.Raycast(ray, out hit))
                    {
                        clickedPosition = hit.point;
                        clickedPosition.y = transform.position.y;
                        Vector3 direction = clickedPosition - transform.position;
                        direction.y = 0;
                        Quaternion rotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                    }
                }
            }
        }
    }

    void FixedUpdate() // the movement of the player
    {
        if(isPlayerPlaced && gameStarter.isPlaying)
        {
            if(isRunning) // The isRunning is set to true from the Run class
            {
                Vector3 MoveVector = transform.TransformDirection(Vector3.forward);
                controller.Move(MoveVector * moveSpeed * Time.deltaTime);
            }
        }
    }

    public void Shoot() // It is called from the ShootButton
    {
        if(isReloaded)
        {
            Transform newBullet = Instantiate(Bullet, spawnBulletPosition.position, transform.rotation);
            newBullet.gameObject.SetActive(true);
            newBullet.GetComponent<BulletProjectile>().OnInstantiated();
            newBullet.name = "bullet";
            gunShot.Play();
            isReloaded = false;
            shootButtonImage.color = unloadedGunColor;
            StartCoroutine(ShootCooldown());
        }
        else
        {
            emptyGunShot.Play();
        }
        
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldownTime);
        isReloaded = true;
        shootButtonImage.color = loadedGunColor;
    }

}