using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    public GameObject prefabToInstantiate;
    public GameObject rangeIndicator;
    public TowerTypeObject basicTower;
    public RectTransform scrollViewViewport;
    public GameObject gameManager;

    private bool canAfford;
    private PlayerManager playerManager;
    private int price = 20;
    private int currentGold;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private GameObject draggingRangeIndicator;
    private GameObject draggingPrefab;
    private bool isDraggingPrefab;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
    }
    private void Start()
    {
        playerManager = gameManager.GetComponent<PlayerManager>();
        currentGold = playerManager.gold;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        if (currentGold - price < 0)
        {
            canAfford = false;
           // Destroy(draggingPrefab);
            return;

        }
        else
        {
            canAfford = true;
            Debug.Log("drag begin");
            originalPosition = rectTransform.anchoredPosition; // Save original position
            canvasGroup.alpha = 0.6f; // Make the item semi-transparent
            canvasGroup.blocksRaycasts = false; // Allow raycasts to pass through the item

            // Create the prefab but disable it for now
            draggingPrefab = Instantiate(basicTower.towerPrefab);
            draggingRangeIndicator = Instantiate(rangeIndicator);
            
            

            //draggingPrefab.SetActive(false);
            DisableShooting(draggingPrefab);
          //  Debug.Log(draggingPrefab.transform.position.x + " " + draggingPrefab.transform.position.y);
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
      //  Debug.Log("ON DRAG" + canvas.scaleFactor);
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Check if the sprite is dragged out of the ScrollView area
        if (!RectTransformUtility.RectangleContainsScreenPoint(scrollViewViewport, Input.mousePosition, eventData.pressEventCamera) && canAfford)
        {
            
            if (!isDraggingPrefab)
            {
                isDraggingPrefab = true;
               // Debug.Log(isDraggingPrefab);
                // Hide the sprite and enable the prefab
                canvasGroup.alpha = 0f;
                draggingPrefab.SetActive(true);
                draggingRangeIndicator.SetActive(true);


                // Make the prefab follow the mouse position
                UpdatePrefabPosition(eventData);
            }
            else
            {
                // Continue to update the prefab's position
                UpdatePrefabPosition(eventData);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Ending Drag");
        if (isDraggingPrefab)
        {
            if (IsValidPlacement(draggingPrefab))
            {
                playerManager.LoseGold(20);
                Vector3 worldPosition = GetWorldPosition(eventData);
                draggingPrefab.transform.position = worldPosition;
                draggingPrefab.SetActive(true);
                EnableShooting(draggingPrefab);
                draggingPrefab.transform.position = new Vector3(draggingPrefab.transform.position.x, draggingPrefab.transform.position.y, 0f);
                
                draggingRangeIndicator.transform.position = new Vector3(draggingPrefab.transform.position.x, draggingPrefab.transform.position.y, 0f);
                draggingRangeIndicator.SetActive(false);

                draggingPrefab = Instantiate(basicTower.towerPrefab, new Vector2(originalPosition.x, originalPosition.y), Quaternion.identity);
                draggingRangeIndicator = Instantiate(rangeIndicator, new Vector2(originalPosition.x, originalPosition.y), Quaternion.identity);

                draggingRangeIndicator.SetActive(false);
                draggingPrefab.SetActive(false);


                Debug.Log("Prefab instantiated at: " + worldPosition);

                // Destroy the temporary prefab
                //Destroy(draggingPrefab);
            }
            else
            {
                Destroy(draggingPrefab);
                Destroy(draggingRangeIndicator);
            }

        }
        else
        {
            Destroy(draggingPrefab);
            Destroy(draggingRangeIndicator);
        }

        // Reset the sprite visibility and position
        canvasGroup.alpha = 1f; // Make the item opaque again
        canvasGroup.blocksRaycasts = true; // Block raycasts again
        rectTransform.anchoredPosition = originalPosition;
        isDraggingPrefab = false;
    }

    private void UpdatePrefabPosition(PointerEventData eventData)
    {
        Vector3 worldPosition = GetWorldPosition(eventData);
        draggingPrefab.transform.position = worldPosition;
        draggingRangeIndicator.transform.position = worldPosition;
    }

    private Vector3 GetWorldPosition(PointerEventData eventData)
    {
        // Convert the screen position to a world position
        Vector3 screenPosition = new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    private void Update()
    {
        currentGold = playerManager.gold;
    }

    private void DisableShooting(GameObject tower)
    {
        var shooter = tower.GetComponent<TurretController>();
        if (shooter != null)
        {
            shooter.enabled = false;
        }
    }

    private void EnableShooting(GameObject tower)
    {
        var shooter = tower.GetComponent<TurretController>();
        if (shooter != null)
        {
            shooter.enabled = true;
        }
    }
    private bool IsValidPlacement(GameObject instance)
    {
        BoxCollider2D collider = instance.GetComponentInChildren<BoxCollider2D>();
        if (collider != null)
        {
            Vector2 size = collider.size;
            Vector2 position = (Vector2)collider.transform.position + collider.offset;

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Buildable", "Tower", "Path", "Scenery")); // Include only relevant layers
            filter.useTriggers = true;

            Collider2D[] results = new Collider2D[10]; // Adjust size as needed
            int count = Physics2D.OverlapBox(position, size, 0f, filter, results);

            for (int i = 0; i < count; i++)
            {
                if (results[i] != null && results[i].gameObject != instance && results[i].GetComponentInChildren<BoxCollider2D>() != instance.GetComponentInChildren<BoxCollider2D>())
                {
                    Debug.Log("Detected collider: " + results[i].name + ", Tag: " + results[i].tag);
                    if (results[i].CompareTag("Scenery") || results[i].CompareTag("Tower"))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    

}



