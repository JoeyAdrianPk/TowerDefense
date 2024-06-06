using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public int health;
    public int damage;
    public int goldValue;
    public delegate void Destroyed();
    public event Destroyed OnDestroyed;
    public int progress;

    private int nextPathIndex = 1;
    private Vector3 nextPathVector3;

    //[HideInInspector] public GameStateManager stateManager;
   // private PathManager pathManager;
    private EnemyWaveManager enemyWaveManager;
    private PlayerManager playerManager;
    public List<Vector2Int> pathRoute;
    private Vector2Int endPoint;

    void Awake()
    {
        GameManager.OnGameStateChanged += HandleStateChange;
    }

    private void HandleStateChange(GameState state)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        enemyWaveManager = GameObject.Find("GameManager").GetComponent<EnemyWaveManager>();
        playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
    }

    public void SetPathRoute(List<Vector2Int> pathCells)
    {
        for (int i = 0; i < pathCells.Count; i++)
        {
            if(pathCells[i] != endPoint)
            {
                pathRoute.Add(pathCells[i]);
            }
            else
            {
                break;
            }
        }       
    }

    // Update is called once per frame
    void Update()
    {
        if (nextPathVector3 == new Vector3(0, 0, 0))
        {
          //  Debug.Log("pathRoute Count = " + pathRoute.Count);
            nextPathVector3 = new Vector3(pathRoute[nextPathIndex].x, pathRoute[nextPathIndex].y, 0f);
        }
        nextPathVector3 = new Vector3(pathRoute[nextPathIndex].x, pathRoute[nextPathIndex].y, 0f);
        transform.position = Vector3.MoveTowards(transform.position, nextPathVector3, speed * Time.deltaTime);

        Vector2 direction = transform.position - nextPathVector3;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        


        this.GetComponent<Rigidbody2D>().rotation = angle;

        if (Vector3.Distance(transform.position, nextPathVector3) < 0.05f)
        {
            nextPathIndex++;
            progress += 1;
            if (nextPathIndex == pathRoute.Count)
            {
                //stateManager.playerRemainingHealth -= damage;
                //stateManager.activeEnemies.Remove(this.gameObject);
                Destroy(true, false);
                
            }
            else
            {
                nextPathVector3 = new Vector3(pathRoute[nextPathIndex].x, pathRoute[nextPathIndex].y, 0 );
            }
            
        }
        if(new Vector2(transform.position.x, transform.position.y) == endPoint)
        {
            Destroy(true, false);
        }

        if(health <= 0)
        {
            Destroy(false, true);
        }

        
    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
    }

    void Destroy(bool takeDamage, bool gainGold)
    {
        if (gainGold)
        {
            playerManager.GainGold(goldValue);
        }
        if (takeDamage)
        {
            playerManager.TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed();
        }
    }
}
