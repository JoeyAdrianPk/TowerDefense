using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private Transform target;
    [Header("Attributes")]
    public float range = 1.5f;
    public float fireRate = 1;
    public float fireCountdown = 0;
    public int value = 20;

    [Header("Unity Fields")]
    public GameObject rangeIndicator;
    public List<GameObject> _enemies;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public string enemyTag = "Enemy";
    public string obstacleTag = "Scenery";

    private int obstacleLayerMask;
    private Transform rangeCircle;

    private void Awake()
    {
        rangeIndicator.transform.localScale = new Vector3(range, range, 1);
        rangeIndicator.transform.position = this.transform.position;
       // this.transform.Find("RangeIndicator").gameObject.transform.localScale += new Vector3(range * 2, range * 2, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CircleCollider2D>().radius = range;
        
        obstacleLayerMask = LayerMask.GetMask(obstacleTag);
       // rangeIndicator.transform.localScale = new Vector3(range, range, transform.position.z);
       // Instantiate(rangeIndicator, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Something is here!");
        if (other.CompareTag(enemyTag))
        {
            GameObject newEnemy = other.gameObject;
            _enemies.Add(newEnemy);

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            GameObject newEnemy = other.gameObject;
            if (_enemies.Contains(newEnemy))
            {
                _enemies.Remove(newEnemy);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {       
        if (target != null)
        {
            if (!HasLineOfSight(target))
            {
               // _enemies.Remove(target.gameObject);
                UpdateTarget();
            }
            else
            {
                //target = _enemies[0].transform;
                Vector2 direction = target.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                Debug.DrawRay(transform.position, direction, Color.black);
            }
        }
        else
        {
            UpdateTarget();
            Debug.DrawRay(transform.position, transform.up * range, Color.blue);
        }
       
        if (fireCountdown <= 0f)
        {
            if(target != null)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }  
        }

        fireCountdown -= Time.deltaTime;
    }


    void UpdateTarget()
    {
        Transform potentialTarget = null;
        int farthestProgress = -1;

        // Iterate through enemies to find the one with the farthest progress and valid line of sight
        foreach (GameObject enemy in _enemies)
        {
            int currentEnemyProgress = enemy.GetComponent<EnemyController>().progress;

            // Check if this enemy has a valid line of sight
            if (currentEnemyProgress > farthestProgress && HasLineOfSight(enemy.transform))
            {
                farthestProgress = currentEnemyProgress;
                potentialTarget = enemy.transform;
            }
        }

        // Update the target
        target = potentialTarget;
    }

    bool HasLineOfSight(Transform enemy)
    {
        Vector3 direction = enemy.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Scenery", "Enemy"));

        if (hit.collider != null)
        {
            return hit.collider.CompareTag("Enemy");
        }

        return false;
    }

    void Shoot()
    {

        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if(bullet != null && HasLineOfSight(target))
        {
            bullet.Seek(target);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
