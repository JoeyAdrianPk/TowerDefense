using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldGenerator : MonoBehaviour
{
    List<Vector2Int> overworldNodes;
    List<Vector2Int> possibleLevelNodes;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i<=14; i++)
        {
            for (int j = 1; j <= 9; j++)
            {
                possibleLevelNodes.Add(new Vector2Int(i, j));
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}






/*
return new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, 3),
            new Vector2(1, 5),
            new Vector2(1, 7),
            new Vector2(1, 9),
            new Vector2(1, 11),
            new Vector2(1, 13),
            new Vector2(1, 15),
            new Vector2(1, 17),

            new Vector2(3, 1),
            new Vector2(3, 3),
            new Vector2(3, 5),
            new Vector2(3, 7),
            new Vector2(3, 9),
            new Vector2(3, 11),
            new Vector2(3, 13),
            new Vector2(3, 15),
            new Vector2(3, 17),

            new Vector2(5, 1),
            new Vector2(5, 3),
            new Vector2(5, 5),
            new Vector2(5, 7),
            new Vector2(5, 9),
            new Vector2(5, 11),
            new Vector2(5, 13),
            new Vector2(5, 15),
            new Vector2(5, 17),

            new Vector2(7, 1),
            new Vector2(7, 3),
            new Vector2(7, 5),
            new Vector2(7, 7),
            new Vector2(7, 9),
            new Vector2(7, 11),
            new Vector2(7, 13),
            new Vector2(7, 15),
            new Vector2(7, 17),

            new Vector2(9, 1),
            new Vector2(9, 3),
            new Vector2(9, 5),
            new Vector2(9, 7),
            new Vector2(9, 9),
            new Vector2(9, 11),
            new Vector2(9, 13),
            new Vector2(9, 15),
            new Vector2(9, 17),
            // Add more positions as needed
        }; 


 */