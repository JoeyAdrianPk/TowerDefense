using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    //private string enemyTag = "Enemy";

    public float speed = 3f;
    public int damage = 1;
    public float minDistanceToDamage = 0.5f;
    public float rotationSpeed = 100f;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = currentRotation;
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        float distanceToTarget = (target.position - transform.position).magnitude;
        /*
        if(distanceToTarget < minDistanceToDamage)
        {

            HitTarget();
            return;
            
        }
        */
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Enemy"))
        {
            
            HitTarget(collision);
        }
    }

    void HitTarget(Collider2D enemyHit)
    {
        Destroy(this.gameObject);
        enemyHit.GetComponent<EnemyController>().TakeDamage(damage);
    }
}
