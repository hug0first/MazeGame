using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSearch : MonoBehaviour
{
    public int gridSizeX;
    public int gridSizeY;
    public MazeNode[] yNodes;
    public MazeNode[][] xNodes;
    public float complexity = 0;
    public float rawComplexity = 0;
    private System.Random random;

    public MazeSearch(int sizeX, int sizeY, System.Random random) {
        this.gridSizeX = sizeX;
        this.gridSizeY = sizeY;
        this.random = random;

        yNodes = new MazeNode[gridSizeY];
        xNodes = new MazeNode[gridSizeX][];

        for (int x = 0; x < gridSizeX; ++x)
        {
            xNodes[x] = new MazeNode[gridSizeY];
            for (int y = 0; y < gridSizeY; ++y)
            {
                xNodes[x][y] = new MazeNode(false, x, y, gridSizeX, gridSizeY);
                //initializes all nodes in the X by Y size grid to isVisited = false and the appropriate x and y coordinates
            }
        }

        int startX = random.Next(gridSizeX);
        int startY = random.Next(gridSizeY);

        runSearch(startX,startY, -1, -1, ref xNodes, true, ref complexity);

        complexity = Mathf.Sqrt(complexity);
        rawComplexity = complexity;
        complexity = complexity - Mathf.Sqrt(gridSizeX * gridSizeY);
        complexity = complexity * 100;
    }

    void runSearch(int x, int y, int prevX, int prevY, ref MazeNode[][] xNodes, bool lastIsNew, ref float complexity)
    {
        int totalUnvisited = 0;
        for (int xNode = 0; xNode < gridSizeX; ++xNode)
        {
            for (int yNode = 0; yNode < gridSizeY; ++yNode)
            {
                if (!xNodes[xNode][yNode].isVisited) 
                {
                    totalUnvisited++;//if node not yet visited, increment the totalUnvisited counter
                }
            }
        }
        if (totalUnvisited == 0) return; //another termination condition

        MazeNode.coordinates? upCoords = null;
        MazeNode.coordinates? downCoords = null;
        MazeNode.coordinates? leftCoords = null;
        MazeNode.coordinates? rightCoords = null;
        
        bool upVisit = true;
        bool downVisit = true;
        bool leftVisit = true;
        bool rightVisit = true;

        if (x != 0) 
        {
            leftVisit = xNodes[x - 1][y].isVisited;
            leftCoords = xNodes[x - 1][y].coords;
        }
        if (x < (gridSizeX - 1)) 
        {
            rightVisit = xNodes[x + 1][y].isVisited;
            rightCoords = xNodes[x + 1][y].coords;
        }
        if (y != 0) 
        {
            downVisit = xNodes[x][y - 1].isVisited;
            downCoords = xNodes[x][y - 1].coords;
        }
        if (y < (gridSizeY - 1)) 
        {
            upVisit = xNodes[x][y + 1].isVisited;
            upCoords = xNodes[x][y + 1].coords;
        }

        List<MazeNode> isUnvisited = new List<MazeNode>();
        xNodes[x][y].isVisited = true;

        if (upCoords != null) if (!upVisit) isUnvisited.Add(xNodes[x][y + 1]);
        if (downCoords != null) if (!downVisit) isUnvisited.Add(xNodes[x][y - 1]);
        if (leftCoords != null) if (!leftVisit) isUnvisited.Add(xNodes[x - 1][y]);
        if (rightCoords != null) if (!rightVisit) isUnvisited.Add(xNodes[x + 1][y]);

        while (isUnvisited.Count > 0)//termination condition
        {
            int randDirection = random.Next(isUnvisited.Count);
            removeWall(ref xNodes[x][y], randDirection, ref isUnvisited);//pretty sure I can remove the condition statement
            runSearch(isUnvisited[randDirection].x,isUnvisited[randDirection].y,x,y, ref xNodes, lastIsNew, ref complexity);//recursively calls runSearch() with random unvisited direction
            isUnvisited.RemoveAt(randDirection);//removes adjacent node from lst of unvisited nodes
        }

        if(lastIsNew && isUnvisited.Count == 0) 
        {
            lastIsNew = false;
            complexity++;//increments complexity if last node was new (i.e. not a backtrack) and this node has no unvisited neighbors
            //this means it has reached the end of a branching path
        }
    }

    void removeWall(ref MazeNode thisNode, int randDirection, ref List<MazeNode> isUnvisited)
    {
        if (!isUnvisited[randDirection].isVisited) 
        {
            if (thisNode.x > isUnvisited[randDirection].x) 
            {
                thisNode.wallLeft = false;
                isUnvisited[randDirection].wallRight = false;
            }
            if (thisNode.x < isUnvisited[randDirection].x)
            {
                thisNode.wallRight = false;
                isUnvisited[randDirection].wallLeft = false;
            }
            if (thisNode.y < isUnvisited[randDirection].y)
            {
                thisNode.wallUp = false;
                isUnvisited[randDirection].wallDown = false;
            }
            if (thisNode.y > isUnvisited[randDirection].y)
            {
                thisNode.wallDown = false;
                isUnvisited[randDirection].wallUp = false;
            }
        }
    }
}
