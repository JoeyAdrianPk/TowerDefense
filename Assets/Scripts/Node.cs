using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public enum NodeState { Locked, Unlocked, Visited }
    public NodeState state;
    public string encounterType;
    public Button button;
    public Image nodeImage;

    void Start()
    {
        button = GetComponent<Button>();
        nodeImage = GetComponent<Image>();
        button.onClick.AddListener(OnNodeClicked);
        UpdateNodeState();
    }

    public void UpdateNodeState()
    {
        switch (state)
        {
            case NodeState.Locked:
                button.interactable = false;
                nodeImage.color = Color.gray; // Example color
                break;
            case NodeState.Unlocked:
                button.interactable = true;
                nodeImage.color = Color.white; // Example color
                break;
            case NodeState.Visited:
                button.interactable = false;
                nodeImage.color = Color.green; // Example color
                break;
        }
    }

    void OnNodeClicked()
    {
        Debug.Log("clicked");
        if (state == NodeState.Unlocked)
        {
            state = NodeState.Visited;
            UpdateNodeState();
            // Transition to level associated with this node
            LevelManager.Instance.LoadNextLevel();
        }
    }
}

