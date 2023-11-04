using UnityEngine;

/// <summary>
/// After an enemy, or a bullet is destroyed, the remaining parts of the enemy or the bullet are destroyed after a certain amount of time, because otherwise, after a time there will be tooo many parts which consume memory
/// </summary>

public class PartDestroyer : MonoBehaviour
{
    [SerializeField] float interval; // after this amount of seconds the parts are destroyed

    float timer;

    private void Start()
    {
        timer = interval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            DestroyParts();
            timer = interval;
        }
    }

    public void DestroyParts()
    {
        GameObject[] parts = GameObject.FindGameObjectsWithTag("Part");
        foreach (GameObject part in parts)
        {
            Destroy(part);
        }
    }
}
