using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeNode : MonoBehaviour
{
    public bool isVisited;
    public int x;
    public int y;
    public int gridSizeX;
    public int gridSizeY;
    public bool wallUp;
    public bool wallDown;
    public bool wallLeft;
    public bool wallRight;
    public bool isTravelled;
    public struct coordinates {
        public int x;
        public int y;
        public coordinates(int x, int y) { this.x = x; this.y = y; }
    }
    public coordinates coords;

    public MazeNode(bool isVisited, int x, int y, int gridSizeX, int gridSizeY) 
    {
        this.isVisited = isVisited; 
        this.x = x; this.y = y; 
        this.gridSizeX = gridSizeX; this.gridSizeY = gridSizeY; 
        wallDown = true; wallUp = true; wallLeft = true; wallRight = true; 
        coords = new coordinates(x,y); 
        isTravelled = false;
    }
}