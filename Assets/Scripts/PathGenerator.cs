using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator
{
    private int width, height, depth;
    private List<Vector2Int> pathCells;
    private List<Vector2Int> route;

    //private HashSet<Vector2Int> pathCells;

    public Vector2Int endPoint;
    private int endPointX;



    public PathGenerator(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;

    }

    public List<Vector2Int> GeneratePath(string enterScreenFrom, Vector2Int? nullableEndpoint = null)
    {
        Debug.Log("Generating Path - enterScreenFrom = " + enterScreenFrom + " nullable endpoint = " + nullableEndpoint);
        endPointX = (width / 2) % 2 != 0 ? width / 2 + 1 : width / 2;
        if(nullableEndpoint == null)
        {
            Debug.Log("NullableEndpoint is Null D=");
            endPoint = new Vector2Int(endPointX, height / 2);
        }
        else
        {
            endPoint = nullableEndpoint.Value;
        }

        int safetyCounter = 0; // Safety counter to prevent infinite loops
        int maxIterations = width * height * 2; // Arbitrary large number for max iterations

        if (enterScreenFrom == "left")
        {
            pathCells = new List<Vector2Int>();
            int y = Random.Range(2, height -1); //height / 2;
            int x = 0;
            Vector2Int currentPosition = new Vector2Int(x, y);
            Debug.Log(y);

            while (x < endPoint.x + 1 && safetyCounter < maxIterations)
            {
                pathCells.Add(new Vector2Int(x, y));
                Vector2Int lastPathCell = new Vector2Int(x, y);
                if (x == endPoint.x && y == endPoint.y)
                {
                    break;
                }

                bool validMove = false;

                while (!validMove && safetyCounter < maxIterations)
                {
                    int move = Random.Range(0, 3); // Updated to 4 to include left turn
                    safetyCounter++; // Increment safety counter to avoid infinite loop

                    if ((move == 0 && x < endPoint.x && CellIsFree(x + 1, y)) || (x % 2 == 0 && endPoint.x > x && CellIsFree(x + 1, y)) || new Vector2(x + 1, y) == new Vector2(endPoint.x, endPoint.y))
                    {
                        x++;
                        validMove = true;
                    }
                    else if (move == 1 && CellIsFree(x, y + 1) && y + 1 < height - 1 && ((x != endPoint.x) || new Vector2(x, y + 1) == new Vector2(endPoint.x, endPoint.y) || (x == endPoint.x && endPoint.y >= y)))
                    {
                        y++;
                        validMove = true;
                    }
                    else if (move == 2 && CellIsFree(x, y - 1) && y - 1 > 0 && ((x != endPoint.x) || (x == endPoint.x && endPoint.y <= y)))
                    {
                        y--;
                        validMove = true;
                    }
                    /*
                    else if (move == 3 && CellIsFree(x - 1, y) && x - 1 >= 0 && ((x != endPoint.x) || new Vector2(x - 1, y) == new Vector2(endPoint.x, endPoint.y) || (x > endPoint.x)))
                    {
                        x--;
                        validMove = true;
                    }
                    */

                    // Check if new position is already in pathCells
                    foreach (Vector2Int vector in pathCells)
                    {
                        Vector2Int newVector = new Vector2Int(x, y);
                        if (vector == newVector)
                        {
                            validMove = false;
                            break;
                        }
                    }
                }

                if (safetyCounter >= maxIterations)
                {
                    Debug.LogError("Max iterations reached, breaking to prevent infinite loop");
                    break;
                }
            }

            if (currentPosition != endPoint)
            {

                pathCells.Add(endPoint);
            }
            
            return pathCells;
        }

        if (enterScreenFrom == "top")
        {
            Debug.Log("Entered Screen From Top!!!!!!!");
            pathCells = new List<Vector2Int>();
            int x = Random.Range(2, width - 1); //width / 2;
            int y = height;

            Vector2Int currentPosition = new Vector2Int(x, y + 1);
            pathCells.Add(new Vector2Int(x, y + 1));

            while (y > endPoint.y - 1 && safetyCounter < maxIterations)
            {
                pathCells.Add(new Vector2Int(x, y));
                Vector2Int lastPathCell = new Vector2Int(x, y);
                // Debug.Log(lastPathCell.x + " " + lastPathCell.y);

                if (x == endPoint.x && y == endPoint.y)
                {
                    break;
                }

                bool validMove = false;

                while (!validMove && safetyCounter < maxIterations)
                {
                    int move = Random.Range(0, 3); // Updated to 4 to include left turn
                    safetyCounter++; // Increment safety counter to avoid infinite loop
                    // Path move down
                    if ((move == 2 && y > endPoint.y && CellIsFree(x, y - 1)) ||
                ((y % 2 == 0) && y > endPoint.y && CellIsFree(x, y - 1)) ||
                (new Vector2(x, y - 1) == new Vector2(endPoint.x, endPoint.y)))
                    {
                        y--;
                        validMove = true;
                    }
                    // Path move right
                    else if ((move == 0 && CellIsFree(x + 1, y) && x + 1 < width - 1 && (x != endPoint.x)) || (endPoint.y == y && endPoint.x > x) ||
                     (new Vector2(x + 1, y) == new Vector2(endPoint.x, endPoint.y)))// x != endPoint.x ||
                    {
                       // Debug.Log("Move Right");
                        x++;
                        validMove = true;
                    }
                    //  Path move left
                    else if ((move == 1 && CellIsFree(x - 1, y) && x - 1 > 0 && (x != endPoint.x)) || (x > endPoint.x && y == endPoint.y) ||
                      (new Vector2(x - 1, y) == new Vector2(endPoint.x, endPoint.y)))// x != endPoint.x ||
                    {
                        x--;
                        validMove = true;
                    }


                    // Check if new position is already in pathCells
                    foreach (Vector2Int vector in pathCells)
                    {
                        Vector2Int newVector = new Vector2Int(x, y);
                        if (vector == newVector)
                        {
                            validMove = false;
                            break;
                        }
                    }
                }

                if (safetyCounter >= maxIterations)
                {
                    Debug.LogError("Max iterations reached, breaking to prevent infinite loop");
                    break;
                }
            }
            // pathCells.Insert(0, new Vector2Int(pathCells[0].x, pathCells[0].y + 1));

            if (currentPosition != endPoint)
            {
                
                pathCells.Add(endPoint);
            }
            Debug.Log("Returning TOP path");
            return pathCells;
        }
        Debug.Log("Returning TOP path");
        return pathCells;

    }

    public List<Vector2Int> GenerateRoute(List<Vector2Int> leftPathCells, List<Vector2Int> topPathCells, List<Vector2Int> rightPathCells, List<Vector2Int> bottomPathCells, Vector2Int exit)
    {
        endPointX = (width / 2) % 2 != 0 ? width / 2 + 1 : width / 2;
       // endPoint = new Vector2Int(endPointX, height / 2);
       endPoint = exit;

        if (leftPathCells.Count > 0)
        {
            Vector2Int direction = Vector2Int.right;
            route = new List<Vector2Int>();
            Vector2Int currentCell = leftPathCells[0];
            // currentCell.x < width -1
            int index = 0;
            while (new Vector2Int(currentCell.x, currentCell.y) != endPoint) //new Vector2Int(currentCell.x, currentCell.y) != pathCells[pathCells.Count - 1] && 
            {

                route.Add(new Vector2Int(currentCell.x, currentCell.y));
                index++;
                currentCell = leftPathCells[index];

            }
            route.Add(new Vector2Int(leftPathCells[leftPathCells.Count - 1].x, leftPathCells[leftPathCells.Count - 1].y));

            return route;
        }

        if (rightPathCells.Count > 0)
        {
            Vector2Int direction = Vector2Int.right;
            route = new List<Vector2Int>();
            Vector2Int currentCell = rightPathCells[0];
            // currentCell.x < width -1
            int index = 0;
            while (new Vector2Int(currentCell.x, currentCell.y) != endPoint) //new Vector2Int(currentCell.x, currentCell.y) != pathCells[pathCells.Count - 1] && 
            {

                route.Add(new Vector2Int(currentCell.x, currentCell.y));
                index++;
                currentCell = rightPathCells[index];

            }
            route.Add(new Vector2Int(rightPathCells[rightPathCells.Count - 1].x, rightPathCells[rightPathCells.Count - 1].y));

            return route;
        }

        if (topPathCells.Count > 0)
        {
            Vector2Int direction = Vector2Int.right;
            route = new List<Vector2Int>();
            Vector2Int currentCell = topPathCells[0];
            // currentCell.x < width -1
            int index = 0;
            while (new Vector2Int(currentCell.x, currentCell.y) != endPoint) //new Vector2Int(currentCell.x, currentCell.y) != pathCells[pathCells.Count - 1] && 
            {

                route.Add(new Vector2Int(currentCell.x, currentCell.y));
                index++;
                currentCell = topPathCells[index];

            }
            route.Add(new Vector2Int(topPathCells[topPathCells.Count - 1].x, topPathCells[topPathCells.Count - 1].y));

            return route;
        }

        if (bottomPathCells.Count > 0)
        {
            Vector2Int direction = Vector2Int.right;
            route = new List<Vector2Int>();
            Vector2Int currentCell = bottomPathCells[0];
            // currentCell.x < width -1
            int index = 0;
            while (new Vector2Int(currentCell.x, currentCell.y) != endPoint) //new Vector2Int(currentCell.x, currentCell.y) != pathCells[pathCells.Count - 1] && 
            {

                route.Add(new Vector2Int(currentCell.x, currentCell.y));
                index++;
                currentCell = bottomPathCells[index];

            }
            route.Add(new Vector2Int(bottomPathCells[bottomPathCells.Count - 1].x, bottomPathCells[bottomPathCells.Count - 1].y));

            return route;
        }


        return route;
    }

    private bool IsValidMove(Vector2Int move)
    {
        return move.x < width && move.y > 0 && move.y < height && !pathCells.Contains(move);
    }

    public List<Vector2Int> GenerateCrossroads(List<Vector2Int> pathCells, string direction, int size)
    {
        // Generate crossroads if there is space for it
        Debug.Log("pathcells count before Gen Crossroads = " + pathCells.Count);
        int originalSize = pathCells.Count;
        // iterate through each cell to check if there is potential for a crossroad there
        for (int i = 0; i < pathCells.Count; i++)
        {
            Vector2Int pathCell = pathCells[i];
            // check initial boundries are within the game grid
            if (pathCell.x > 2 && pathCell.x < width - 2 && pathCell.y > 2 && pathCell.y < height - 2 && pathCell != endPoint)
            {
                if (direction == "up")
                {

                    if (IsSpaceForCrossroads(pathCells, (new Vector2Int(pathCell.x, pathCell.y)), direction, size))
                    {
                        // insert the new crossroads into the original path
                        pathCells.InsertRange(i + 1, MakeCrossroadCoordinates(i, "up", size));
                    }

                }


                if (direction == "down")
                {
                    if (IsSpaceForCrossroads(pathCells, (new Vector2Int(pathCell.x, pathCell.y)), direction, size))
                    {
                        // insert the new crossroads into the original path
                        pathCells.InsertRange(i + 1, MakeCrossroadCoordinates(i, "down", size));
                    }
                }
            }

            // if a crossroad was inserted - return the new path
            // if not continue to next loop
            if (originalSize != pathCells.Count)
            {
                Debug.Log("pathcells count AFTER Gen Crossroads = " + pathCells.Count);
                return pathCells;
            }

        }
        Debug.Log("pathcells count AFTER Gen Crossroads = " + pathCells.Count);
        return pathCells;

    }

    private List<Vector2Int> MakeCrossroadCoordinates(int index, string direction, int size)
    {
        List<Vector2Int> returnCoordinates = new List<Vector2Int>();
        switch (direction)
        {
            case "up":
                switch (size)
                {
                    case 1:
                        // insert a crossroad with 1 cell in middle
                        returnCoordinates = new List<Vector2Int> { new Vector2Int(pathCells[index].x + 1, pathCells[index].y), new Vector2Int(pathCells[index].x + 2, pathCells[index].y),
                    new Vector2Int(pathCells[index].x + 2, pathCells[index].y + 1), new Vector2Int(pathCells[index].x + 2, pathCells[index].y + 2), new Vector2Int(pathCells[index].x + 1, pathCells[index].y + 2),
                    new Vector2Int(pathCells[index].x, pathCells[index].y + 2), new Vector2Int(pathCells[index].x, pathCells[index].y + 1) };
                        break;
                    case 2:
                        // insert a crossroad with 4 cells in middle
                        returnCoordinates = new List<Vector2Int> { new Vector2Int(pathCells[index].x + 1, pathCells[index].y), new Vector2Int(pathCells[index].x + 2, pathCells[index].y), new Vector2Int(pathCells[index].x + 3, pathCells[index].y),
                     new Vector2Int(pathCells[index].x + 3, pathCells[index].y + 1), new Vector2Int(pathCells[index].x + 3, pathCells[index].y + 2), new Vector2Int(pathCells[index].x + 3, pathCells[index].y + 3),
                    new Vector2Int(pathCells[index].x + 2, pathCells[index].y + 3), new Vector2Int(pathCells[index].x + 1, pathCells[index].y + 3), new Vector2Int(pathCells[index].x, pathCells[index].y + 3),
                        new Vector2Int(pathCells[index].x , pathCells[index].y + 2), new Vector2Int(pathCells[index].x, pathCells[index].y + 1)};
                        break;
                }

                break;

            case "down":
                returnCoordinates = new List<Vector2Int> { new Vector2Int(pathCells[index].x + 1, pathCells[index].y), new Vector2Int(pathCells[index].x + 2, pathCells[index].y),
                    new Vector2Int(pathCells[index].x + 2, pathCells[index].y - 1), new Vector2Int(pathCells[index].x + 2, pathCells[index].y - 2), new Vector2Int(pathCells[index].x + 1, pathCells[index].y - 2),
                    new Vector2Int(pathCells[index].x, pathCells[index].y - 2), new Vector2Int(pathCells[index].x, pathCells[index].y - 1) };
                break;

        }
        return returnCoordinates;
    }

    private bool IsSpaceForCrossroads(List<Vector2Int> pathCells, Vector2Int startingPoint, string direction, int size)
    {

        if (direction == "up")
        {
            if (size == 1)
            {
                if (CellIsFree(startingPoint.x, startingPoint.y + 3) && CellIsFree(startingPoint.x + 1, startingPoint.y + 3) && CellIsFree(startingPoint.x + 2, startingPoint.y + 3)
              && CellIsFree(startingPoint.x, startingPoint.y + 2) && CellIsFree(startingPoint.x + 1, startingPoint.y + 2) && CellIsFree(startingPoint.x + 2, startingPoint.y + 2) && CellIsFree(startingPoint.x + 3, startingPoint.y + 2)
              && CellIsFree(startingPoint.x - 1, startingPoint.y + 1) && CellIsFree(startingPoint.x, startingPoint.y + 1) && CellIsFree(startingPoint.x + 1, startingPoint.y + 1) && CellIsFree(startingPoint.x + 2, startingPoint.y + 1)
              && CellIsFree(startingPoint.x + 1, startingPoint.y) && CellIsFree(startingPoint.x + 2, startingPoint.y) && CellIsFree(startingPoint.x + 3, startingPoint.y)
              && CellIsFree(startingPoint.x + 1, startingPoint.y - 1) && CellIsFree(startingPoint.x + 2, startingPoint.y - 1) && CellIsFree(startingPoint.x + 3, startingPoint.y - 1))
                {
                    return true;
                }
            }

            if (size == 2)
            {
                if (CellIsFree(startingPoint.x - 1, startingPoint.y + 4) && CellIsFree(startingPoint.x, startingPoint.y + 4) && CellIsFree(startingPoint.x + 1, startingPoint.y + 4) && CellIsFree(startingPoint.x + 2, startingPoint.y + 4) && CellIsFree(startingPoint.x + 3, startingPoint.y + 4) && CellIsFree(startingPoint.x + 4, startingPoint.y + 4)
              && CellIsFree(startingPoint.x - 1, startingPoint.y + 3) && CellIsFree(startingPoint.x, startingPoint.y + 3) && CellIsFree(startingPoint.x + 1, startingPoint.y + 3) && CellIsFree(startingPoint.x + 2, startingPoint.y + 3) && CellIsFree(startingPoint.x + 3, startingPoint.y + 3) && CellIsFree(startingPoint.x + 4, startingPoint.y + 3)
              && CellIsFree(startingPoint.x - 1, startingPoint.y + 2) && CellIsFree(startingPoint.x, startingPoint.y + 2) && CellIsFree(startingPoint.x + 3, startingPoint.y + 2) && CellIsFree(startingPoint.x + 4, startingPoint.y + 2)
              && CellIsFree(startingPoint.x - 1, startingPoint.y + 1) && CellIsFree(startingPoint.x, startingPoint.y + 1) && CellIsFree(startingPoint.x + 3, startingPoint.y + 1) && CellIsFree(startingPoint.x + 4, startingPoint.y + 1)
              && CellIsFree(startingPoint.x + 1, startingPoint.y) && CellIsFree(startingPoint.x + 2, startingPoint.y) && CellIsFree(startingPoint.x + 3, startingPoint.y) && CellIsFree(startingPoint.x + 4, startingPoint.y)
              && CellIsFree(startingPoint.x + 1, startingPoint.y - 1) && CellIsFree(startingPoint.x + 2, startingPoint.y - 1) && CellIsFree(startingPoint.x + 3, startingPoint.y - 1) && CellIsFree(startingPoint.x + 4, startingPoint.y - 1))
                {
                    return true;
                }
            }

        }
        else if (direction == "down")
        {
            if (CellIsFree(startingPoint.x + 1, startingPoint.y + 1) && CellIsFree(startingPoint.x + 2, startingPoint.y + 1) && CellIsFree(startingPoint.x + 3, startingPoint.y + 1)
                && CellIsFree(startingPoint.x + 1, startingPoint.y) && CellIsFree(startingPoint.x + 2, startingPoint.y) && CellIsFree(startingPoint.x + 3, startingPoint.y)
                && CellIsFree(startingPoint.x - 1, startingPoint.y - 1) && CellIsFree(startingPoint.x, startingPoint.y - 1) && CellIsFree(startingPoint.x + 2, startingPoint.y - 1) && CellIsFree(startingPoint.x + 3, startingPoint.y - 1)
                && CellIsFree(startingPoint.x - 1, startingPoint.y - 2) && CellIsFree(startingPoint.x, startingPoint.y - 2) && CellIsFree(startingPoint.x + 1, startingPoint.y - 2) && CellIsFree(startingPoint.x + 2, startingPoint.y - 2) && CellIsFree(startingPoint.x + 3, startingPoint.y - 2)
                && CellIsFree(startingPoint.x - 1, startingPoint.y - 3) && CellIsFree(startingPoint.x, startingPoint.y - 3) && CellIsFree(startingPoint.x + 1, startingPoint.y - 3) && CellIsFree(startingPoint.x + 2, startingPoint.y - 3) && CellIsFree(startingPoint.x + 3, startingPoint.y - 3))
            {
                return true;
            }
        }



        return false;
    }

    public bool CellIsFree(int x, int y)
    {
        return !pathCells.Contains(new Vector2Int(x, y));
    }

    public bool CellIsTaken(List<Vector2Int> pathCells, int x, int y)
    {
        return pathCells.Contains(new Vector2Int(x, y));
    }

    public bool CellIsTaken(Vector2Int cell)
    {
        return pathCells.Contains(cell);
    }

    public int GetCellNeighborValue(List<Vector2Int> pathCells, int x, int y)
    {
        int returnValue = 0;

        if (CellIsTaken(pathCells, x, y + 1)) // Up
        {
            returnValue |= 1 << 3; // 8
        }

        if (CellIsTaken(pathCells, x + 1, y)) // Right
        {
            returnValue |= 1 << 2; // 4
        }

        if (CellIsTaken(pathCells, x - 1, y)) // Left
        {
            returnValue |= 1 << 1; // 2
        }

        if (CellIsTaken(pathCells, x, y - 1)) // Down
        {
            returnValue |= 1 << 0; // 1
        }

        return returnValue;
    }

}
