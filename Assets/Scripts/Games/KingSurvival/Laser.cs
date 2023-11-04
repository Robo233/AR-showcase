using UnityEngine;

/// <summary>
/// Handles the laser which cmoes from the mouth of the king and it is used to shoot the enemies
/// </summary>

public class Laser : MonoBehaviour
{
    LineRenderer laser;

    public Material enemyIdentifiedMaterial;
    public Material laserMaterial;
    public Material cannotShootMaterial;

    public PlayerMovement playerMovement;

    void Start()
    {
        laser = GetComponent<LineRenderer>();
        laser.material = laserMaterial;
    }
 
    void Update()
    {
        laser.SetPosition(0, transform.position);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit))
        {   
            if (hit.collider)
            {
                if(playerMovement && playerMovement.isReloaded)
                {
                    if(hit.transform.name.Substring(0,5)=="Enemy")
                    {
                        laser.material = enemyIdentifiedMaterial;
                    }
                    else
                    {
                        laser.material = laserMaterial;
                    }
                }
                else
                {
                    laser.material = cannotShootMaterial;
                }
                laser.SetPosition(1, hit.point);
            }
        }
        else
        {
            laser.SetPosition(1, transform.position + (transform.forward * 5000));
        }
    }

}