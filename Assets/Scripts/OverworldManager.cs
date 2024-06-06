using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldManager : MonoBehaviour
{
    public GameObject nodeButtonPrefab;
    public GameObject bossButtonPrefab;
    public Transform nodesParent; // The parent transform where buttons will be instantiated
    public List<Node> nodes = new List<Node>();
    public GameObject linePrefab;

    public int numberOfColumns = 3;
    public Vector2 nodeSpacing = new Vector2(100, 100); // Adjust spacing as needed
    private List<GameObject> lines = new List<GameObject>();

    List<Vector2> currentNodeGrid = new List<Vector2>();

    private void Start()
    {
        GenerateOverworldMap();
    }

    void GenerateOverworldMap()
    {
        // Clear existing nodes
        foreach (Transform child in nodesParent)
        {
            Destroy(child.gameObject);
        }
        nodes.Clear();
        // List<Vector2> currentNodeGrid = new List<Vector2>();
        currentNodeGrid.Clear();

        

        for (int columns = 0; columns < numberOfColumns; columns++)
        {
            List<Vector2> fullCollumnOfNodes = new List<Vector2>();
            Vector2 start = GetRandomNodeFromRow1();
            fullCollumnOfNodes.Add(start);
            // For each node, check that it's a valid placement then add it to the list of nodes to be instantiated
            for (int nodeNumber = 1; nodeNumber < 7; nodeNumber++)
            {
                List<Vector2> possibleSelections = FindValidNodePlacement(start);
                Vector2 lastAdded = possibleSelections[Random.Range(0, possibleSelections.Count)];
                fullCollumnOfNodes.Add(lastAdded);
                start = lastAdded;
            }

            fullCollumnOfNodes.Add(new Vector2(0, 250));

            // Instantiate nodes in the valid positions inside of fullColumnOfNodes list
            for (int i = 0; i <= 6; i++)
            {
                if(i != 6)
                {
                    GameObject nodeButton = Instantiate(nodeButtonPrefab, nodesParent);
                    nodeButton.GetComponent<RectTransform>().anchoredPosition = fullCollumnOfNodes[i];
                    currentNodeGrid.Add(fullCollumnOfNodes[i]);

                    Node node = nodeButton.GetComponent<Node>();
                    node.state = Node.NodeState.Locked;
                    node.encounterType = GetRandomEncounterType();
                    if (i == 0)
                    {
                        node.state = Node.NodeState.Unlocked;
                    }
                    node.UpdateNodeState();

                    nodes.Add(node);
                }
                else
                {
                    GameObject nodeButton = Instantiate(bossButtonPrefab, nodesParent);
                    nodeButton.GetComponent<RectTransform>().anchoredPosition = fullCollumnOfNodes[fullCollumnOfNodes.Count -1];
                    currentNodeGrid.Add(fullCollumnOfNodes[fullCollumnOfNodes.Count -1]);

                    Node node = nodeButton.GetComponent<Node>();
                    node.state = Node.NodeState.Locked;
                    node.encounterType = GetRandomEncounterType();
                    if (i == 0)
                    {
                        node.state = Node.NodeState.Unlocked;
                    }
                    node.UpdateNodeState();

                    nodes.Add(node);
                }
                
            }
        }

        foreach (var node in nodes)
        {
            DrawConnections(node);
        }
        ConnectTopmostNode();
        // Ensure specific rules for encounters, e.g., elite battles after level 5
    }

    void DrawConnections(Node node)
    {
        RectTransform rectTransform = node.GetComponent<RectTransform>();
        Vector2 anchoredPosition = rectTransform.anchoredPosition;

        List<Vector2> possibleConnections = new List<Vector2>
        {
            new Vector2(anchoredPosition.x, anchoredPosition.y + 100), // Up
            new Vector2(anchoredPosition.x - 100, anchoredPosition.y + 100), // Up-Left
            new Vector2(anchoredPosition.x + 100, anchoredPosition.y + 100) // Up-Right
        };

        foreach (var targetAnchoredPosition in possibleConnections)
        {
            if (currentNodeGrid.Contains(targetAnchoredPosition))
            {
                DrawLine(anchoredPosition, targetAnchoredPosition);
            }
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject line = Instantiate(linePrefab, nodesParent);
        RectTransform rectTransform = line.GetComponent<RectTransform>();

        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
        rectTransform.anchoredPosition = start + direction * distance / 2;
        rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        lines.Add(line);
    }

    void ConnectTopmostNode()
    {
        // Find the topmost node
        Node topmostNode = null;
        //float maxY = float.MinValue;

        foreach (var node in nodes)
        {
            RectTransform rectTransform = node.GetComponent<RectTransform>();
            if (rectTransform.anchoredPosition.y >= 150)
            {
                topmostNode = node;
                RectTransform topmostRect = topmostNode.GetComponent<RectTransform>();
                DrawLine(topmostRect.anchoredPosition, new Vector2(0, 250));
            }
        }
    }

    Vector2 GetRandomNodeFromRow1()
    {
        Vector2[] firstRow = {
            new Vector2(-300, -350), //1,1
            new Vector2(-200, -350), //2,1
            new Vector2(-100, -350), //3,1
            new Vector2(0, -350),    //4,1
            new Vector2(100, -350),  //5,1
            new Vector2(200, -350),  //6,1
            new Vector2(300, -350),  //7,1
        };

        Vector2 returnVector2 = firstRow[Random.Range(0, 7)];

        foreach (Vector2 node in currentNodeGrid)
        {
            if (node == returnVector2)
            {
                return GetRandomNodeFromRow1();
            }
        }

        return firstRow[Random.Range(0, 7)];
    }

    Vector2 GetRandomNodePosition(int columnNumber)
    {
        // I changed this to always return a random position from the row that is sent


        // Implement logic to get a random position within the canvas bounds
        // Example: Randomly distribute nodes within a specified range
        Vector2 returnVector2 = new Vector2(0, 0);

        Vector2[] firstRow = {
            new Vector2(-300, -350), //1,1
            new Vector2(-200, -350), //2,1
            new Vector2(-100, -350), //3,1
            new Vector2(0, -350),    //4,1
            new Vector2(100, -350),  //5,1
            new Vector2(200, -350),  //6,1
            new Vector2(300, -350),  //7,1
        };

        Vector2[] secondRow = {
            new Vector2(-300, -250), //1,2
            new Vector2(-200, -250), //2,2
            new Vector2(-100, -250), //3,2
            new Vector2(0, -250),    //4,2
            new Vector2(100, -250),  //5,2
            new Vector2(200, -250),  //6,2
            new Vector2(300, -250),  //7,2
            
        };

        Vector2[] thirdRow = {
            new Vector2(-300, -150),
            new Vector2(-200, -150),
            new Vector2(-100, -150),
            new Vector2(0, -150),
            new Vector2(100, -150),
            new Vector2(200, -150),
            new Vector2(300, -150),
        };

        Vector2[] fourthRow = {
            new Vector2(-300, -50),
            new Vector2(-200, -50),
            new Vector2(-100, -50),
            new Vector2(0, -50),
            new Vector2(100, -50),
            new Vector2(200, -50),
            new Vector2(300, -50),
        };

        Vector2[] fifthRow = {
            new Vector2(-300, 50),
            new Vector2(-200, 50),
            new Vector2(-100, 50),
            new Vector2(0, 50),
            new Vector2(100, 50),
            new Vector2(200, 50),
            new Vector2(300, 50),
        };

        Vector2[] sixthRow = {
            new Vector2(-300, 150),
            new Vector2(-200, 150),
            new Vector2(-100, 150),
            new Vector2(0, 150),
            new Vector2(100, 150),
            new Vector2(200, 150),
            new Vector2(300, 150),
        };

        Vector2[] seventhRow = {
            new Vector2(-300, 250),
            new Vector2(-200, 250),
            new Vector2(-100, 250),
            new Vector2(0, 250),
            new Vector2(100, 250),
            new Vector2(200, 250),
            new Vector2(300, 250),
        };


        switch (columnNumber)
        {
            case 1:
                returnVector2 = firstRow[Random.Range(0, 7)];
                break;
            case 2:
                returnVector2 = secondRow[Random.Range(0, 7)];
                break;
            case 3:
                returnVector2 = thirdRow[Random.Range(0, 7)];
                break;
            case 4:
                returnVector2 = fourthRow[Random.Range(0, 7)];
                break;
            case 5:
                returnVector2 = fifthRow[Random.Range(0, 7)];
                break;
            case 6:
                returnVector2 = sixthRow[Random.Range(0, 7)];
                break;
            case 7:
                returnVector2 = seventhRow[Random.Range(0, 7)];
                break;
        }


        /*
        switch (columnNumber)
        {
            case 1:
                returnVector2 = firstRow[Random.Range(0, 7)];
                break;
            case 2:
                returnVector2 = secondRow[Random.Range(0, 7)];
                break;
            case 3:
                returnVector2 = thirdRow[Random.Range(0, 7)];
                break;
            case 4:
                returnVector2 = fourthRow[Random.Range(0, 7)];
                break;
            case 5:
                returnVector2 = fifthRow[Random.Range(0, 7)];
                break;
            case 6:
                returnVector2 = sixthRow[Random.Range(0, 7)];
                break;
            case 7:
                returnVector2 = seventhRow[Random.Range(0, 7)];
                break;
        }
        */



        // float x = Random.Range(-nodesParent.GetComponent<RectTransform>().rect.width / 2, nodesParent.GetComponent<RectTransform>().rect.width / 2);
        // float y = Random.Range(-nodesParent.GetComponent<RectTransform>().rect.height / 2, nodesParent.GetComponent<RectTransform>().rect.height / 2);
        // return new Vector2(x, y);
        return returnVector2;
    }

    List<Vector2> FindValidNodePlacement(Vector2 start)
    {
        List<Vector2> possibleNodePositions = new List<Vector2>();
        if (start.y + 100 <= 250)
        {
            int counter = 0;
            foreach(Vector2 node in currentNodeGrid)
            {
                if(node == new Vector2(start.x, start.y + 100))
                {
                    counter++;
                }
            }
            if(counter == 0)
            {
                possibleNodePositions.Add(new Vector2(start.x, start.y + 100));
            }
            
        }
        if(start.y + 100 <= 250 && start.x -100 >= -300)
        {
            int counter = 0;
            foreach (Vector2 node in currentNodeGrid)
            {
                if (node == new Vector2(start.x - 100, start.y + 100))
                {
                    counter++;
                }
            }
            if(counter == 0)
            {
                possibleNodePositions.Add(new Vector2(start.x - 100, start.y + 100));
            }
            
        }
        if (start.y + 100 <= 250 && start.x + 100 <= 300)
        {
            int counter = 0;
            foreach (Vector2 node in currentNodeGrid)
            {
                if (node == new Vector2(start.x + 100, start.y + 100))
                {
                    counter++;
                }
            }
            if (counter == 0)
            {
                possibleNodePositions.Add(new Vector2(start.x + 100, start.y + 100));
            }
        }

        return possibleNodePositions;
    }

    string GetRandomEncounterType()
    {
        // Define your encounter types and logic here
        return "combat"; // Example
    }

    public void UnlockNode(Node node)
    {
        node.state = Node.NodeState.Unlocked;
        node.UpdateNodeState();
    }

    // Save and load state methods here
}

