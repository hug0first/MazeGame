using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexityAlgo : MonoBehaviour
{
    private float rating = 0;//rating of maze complexity set to 0 by default
    private int gottenCheckpoints = 0;//number of gotten checkpoints set to 0 by default
    MazeSearch maze;

    public ComplexityAlgo(MazeSearch maze) 
    { 
        this.maze = maze; 
        for (int xNode = 0; xNode < maze.gridSizeX; ++xNode)
        {
            for (int yNode = 0; yNode < maze.gridSizeY; ++yNode)
            {
                maze.xNodes[xNode][yNode].isTravelled = false;//setting all nodes to not travelled by complexity algorithm
            }
        }
    }

    public float getRating() { return rating; }

    public float complexityFinish(int startX, int startY, int finishX, int finishY)
    {
        float rating = 0;

        return rating;
    }

    public void complexityCheckpoint(int x, int y, GameObject[] checkpointList, float pathSize)
    {
        maze.xNodes[x][y].isTravelled = true;//marking current node as travelled by complexity algorithm

        List<MazeNode.coordinates> checkpointCoords = new List<MazeNode.coordinates>();
        foreach (GameObject checkpoint in checkpointList)
        {
            checkpointCoords.Add(new MazeNode.coordinates((int)(checkpoint.transform.position.x / pathSize), (int)(checkpoint.transform.position.z / pathSize)));//getting a list of checkpoint grid coordinates
        }
        foreach (MazeNode.coordinates checkpointCoord in checkpointCoords)
        {
            if (checkpointCoord.x == x && checkpointCoord.y == y) gottenCheckpoints++;//if current node is at a checkpoint, increment gottenCheckpoints
        }
        if (gottenCheckpoints == checkpointList.Length) 
        {
            return;//if all checkpoints have been gotten, return with the current rating
        }

        int totalUntravelled = 0;
        for (int xNode = 0; xNode < maze.gridSizeX; ++xNode)
        {
            for (int yNode = 0; yNode < maze.gridSizeY; ++yNode)
            {
                if (!maze.xNodes[xNode][yNode].isTravelled) 
                {
                    totalUntravelled++;//if node not yet travelled, increment totalUntravelled
                }
            }
        }
        if (totalUntravelled == 0) return; //if all nodes have been travelled, return with the current rating

        MazeNode.coordinates? upCoords = null;
        MazeNode.coordinates? downCoords = null;
        MazeNode.coordinates? leftCoords = null;
        MazeNode.coordinates? rightCoords = null;

        bool upTravel = true;
        bool downTravel = true;
        bool leftTravel = true;
        bool rightTravel = true;

        bool upAvailable = true;
        bool downAvailable = true;
        bool leftAvailable = true;
        bool rightAvailable = true;

        if (x != 0) 
        {
            leftTravel = maze.xNodes[x - 1][y].isTravelled;
            leftCoords = maze.xNodes[x - 1][y].coords;
            leftAvailable = !maze.xNodes[x][y].wallLeft;
        }
        if (x < (maze.gridSizeX - 1)) 
        {
            rightTravel = maze.xNodes[x + 1][y].isTravelled;
            rightCoords = maze.xNodes[x + 1][y].coords;
            rightAvailable = !maze.xNodes[x][y].wallRight;
        }
        if (y != 0) 
        {
            downTravel = maze.xNodes[x][y - 1].isTravelled;
            downCoords = maze.xNodes[x][y - 1].coords;
            downAvailable = !maze.xNodes[x][y].wallDown;
        }
        if (y < (maze.gridSizeY - 1)) 
        {
            upTravel = maze.xNodes[x][y + 1].isTravelled;
            upCoords = maze.xNodes[x][y + 1].coords;
            upAvailable = !maze.xNodes[x][y].wallUp;
        }

        List<MazeNode> isTravelled = new List<MazeNode>();
        //if adjacent node is not travelled and has no wall blocking its path from the current node, add it to lost of untravelled nodes
        if (upCoords != null) if (!upTravel && upAvailable) isTravelled.Add(maze.xNodes[x][y + 1]);
        if (downCoords != null) if (!downTravel && downAvailable) isTravelled.Add(maze.xNodes[x][y - 1]);
        if (leftCoords != null) if (!leftTravel && leftAvailable) isTravelled.Add(maze.xNodes[x - 1][y]);
        if (rightCoords != null) if (!rightTravel && rightAvailable) isTravelled.Add(maze.xNodes[x + 1][y]);

        if (isTravelled.Count == 0) 
        {
            rating++;//if no valid adjacent nodes, increment rating
            return;//if no valid adjacent nodes, return with current rating
        }

        while (isTravelled.Count > 0)
        {
            int randDirection = new System.Random().Next(isTravelled.Count);//randomly choosing a direction to go in from the list of valid adjacent nodes
            complexityCheckpoint(isTravelled[randDirection].x, isTravelled[randDirection].y, checkpointList, pathSize);//recursive call to complexityCheckpoint with the randomly chosen node
            isTravelled.RemoveAt(randDirection);//removing the randomly chosen node from the list of valid adjacent nodes
            isTravelled = findValidAdjacentNodes(maze, x, y); //getting a new list of valid adjacent nodes
        }

        return;
    }

    List<MazeNode> findValidAdjacentNodes(MazeSearch maze, int x, int y)
    {
        MazeNode upNode = null;
        MazeNode downNode = null;
        MazeNode leftNode = null;
        MazeNode rightNode = null;

        if (y + 1 < maze.gridSizeY) upNode = maze.xNodes[x][y + 1];
        if (y - 1 >= 0) downNode = maze.xNodes[x][y - 1];
        if (x - 1 >= 0) leftNode = maze.xNodes[x - 1][y];
        if (x + 1 < maze.gridSizeX) rightNode = maze.xNodes[x + 1][y];

        List<MazeNode> adjacentNodes = new List<MazeNode>();
        MazeNode.coordinates? upCoords = null;
        MazeNode.coordinates? downCoords = null;
        MazeNode.coordinates? leftCoords = null;
        MazeNode.coordinates? rightCoords = null;
        
        bool upTravel = true;
        bool downTravel = true;
        bool leftTravel = true;
        bool rightTravel = true;

        if (x != 0) 
        {
            leftTravel = maze.xNodes[x - 1][y].isTravelled;
            leftCoords = maze.xNodes[x - 1][y].coords;
        }
        if (x < (maze.gridSizeX - 1)) 
        {
            rightTravel = maze.xNodes[x + 1][y].isTravelled;
            rightCoords = maze.xNodes[x + 1][y].coords;
        }
        if (y != 0) 
        {
            downTravel = maze.xNodes[x][y - 1].isTravelled;
            downCoords = maze.xNodes[x][y - 1].coords;
        }
        if (y < (maze.gridSizeY - 1)) 
        {
            upTravel = maze.xNodes[x][y + 1].isTravelled;
            upCoords = maze.xNodes[x][y + 1].coords;
        }

        if (upCoords != null) if (!upTravel) adjacentNodes.Add(maze.xNodes[x][y + 1]);//if adjacent node is not travelled, add it to list of untravelled nodes
        if (downCoords != null) if (!downTravel) adjacentNodes.Add(maze.xNodes[x][y - 1]);
        if (leftCoords != null) if (!leftTravel) adjacentNodes.Add(maze.xNodes[x - 1][y]);
        if (rightCoords != null) if (!rightTravel) adjacentNodes.Add(maze.xNodes[x + 1][y]);

        return adjacentNodes;
    }
}
