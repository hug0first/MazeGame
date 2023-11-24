using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int gridSizeX;
    [SerializeField] int gridSizeY;
    private float pathSize;
    [SerializeField] GameObject path;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject player;
    [SerializeField] GameObject ceiling;
    [SerializeField] float desiredComplexityMin;
    [SerializeField] float desiredComplexityMax;
    private float mazeComplexity;
    private float solverComplexity;
    private ComplexityAlgo complexityAlgo;
    private System.Random random;
    [SerializeField] int seed;
    [SerializeField] bool seedEnabled;
    [SerializeField] int startX;
    [SerializeField] int startY;
    [SerializeField] GameObject wallContainer;
    [SerializeField] GameObject ceilingContainer;
    [SerializeField] GameObject pathContainer;
    [SerializeField] GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        if (seedEnabled) random = new System.Random(seed);
        else 
        {
            random = new System.Random();
            seed = random.Next();
        }
        pathSize = path.transform.localScale.x * 10;//gets the size of the path blocks
        generateMaze(seed);//generates the warmup maze
    }

    // Update is called once per frame
    void Update()
    {

    }

    void generateMaze(int seed)
    {
        Debug.Log("Maze Seed: " + seed);
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Path"))
        {
            Destroy(block);
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Wall"))
        {
            Destroy(block);
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Finish"))
        {
            Destroy(block);
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            Destroy(block);
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Ceiling"))
        {
            Destroy(block);
        }

        random = new System.Random(seed); 
        MazeSearch maze = new MazeSearch(gridSizeX,gridSizeY, random);
        Debug.Log("Raw Complexity: " + maze.rawComplexity + "\nComplexity: " + maze.complexity);

        for (int x = 0; x < gridSizeX; ++x)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                constructMazeBlock(x,y,maze);
            }
        }
        /* if (checkpointEnabled)  */placePlayer(maze,startX,startY);//places player
        //else placeStartFinish(maze);
        if (maze.rawComplexity < desiredComplexityMin || maze.rawComplexity > desiredComplexityMax) generateMaze(seed);//if complexity is not within desired range, generate a new maze
    }

    void constructMazeBlock(float x, float y, MazeSearch maze)
    {
        float trueX = x*pathSize;
        float trueY = y*pathSize;
        GameObject newBlock = Instantiate(path);
        newBlock.transform.parent = pathContainer.transform;
        MazeNode thisNode = maze.xNodes[(int)x][(int)y];
        newBlock.transform.position = new Vector3(trueX, .001f, trueY);

        newBlock = Instantiate(ceiling);
        newBlock.transform.parent = ceilingContainer.transform;
        newBlock.transform.position = new Vector3(trueX, wall.transform.localScale.y, trueY);
        newBlock.transform.rotation = Quaternion.Euler(180, 0, 0);

        if (thisNode.wallUp) 
        {
            GameObject upWall = Instantiate(wall);
            upWall.transform.position = new Vector3(trueX, 1.5f, trueY + 1.38f);
            upWall.transform.parent = wallContainer.transform;
        }
        if (thisNode.wallDown) 
        {
            GameObject downWall = Instantiate(wall);
            downWall.transform.position = new Vector3(trueX, 1.5f, trueY - 1.38f);
            downWall.transform.parent = wallContainer.transform;
        }
        if (thisNode.wallLeft) 
        {
            GameObject leftWall = Instantiate(wall);
            leftWall.transform.position = new Vector3(trueX - 1.38f, 1.5f, trueY);
            leftWall.transform.Rotate(0, 90, 0);
            leftWall.transform.parent = wallContainer.transform;
        }
        if (thisNode.wallRight) 
        {
            GameObject rightWall = Instantiate(wall);
            rightWall.transform.position = new Vector3(trueX + 1.38f, 1.5f, trueY);
            rightWall.transform.Rotate(0, 90, 0);
            rightWall.transform.parent = wallContainer.transform;
        }
    }

    /* void placeStartFinish(MazeSearch maze)//places finish randomly, then places player as far as possible from finish
    {
        var random = new System.Random();
        int finishX = random.Next(gridSizeX);
        int finishY = random.Next(gridSizeY);
        int startX;
        int startY;
        
        do
        {
            if (finishX >= ((gridSizeX-1)/2))  startX = 0;
            else startX = gridSizeX - 1;

            if (finishY >= ((gridSizeY-1)/2))  startY = 0;
            else startY = gridSizeY - 1;
        } while(finishX == startX && finishY == startY);

        player.transform.position = new Vector3(startX*pathSize, 1, startY*pathSize);
        GameObject finishBlock = Instantiate(finish);
        finishBlock.transform.position = new Vector3(finishX*pathSize, 1, finishY*pathSize);
    } */

    /* void placeCheckpoints(MazeSearch maze)
    {
        complexityAlgo = new ComplexityAlgo(maze);
        int checkpointNum = 0;
        var random = new System.Random();
        int startX = random.Next(gridSizeX);
        int startY = random.Next(gridSizeY);
        GameObject[] checkpointList;

        player.transform.position = new Vector3(startX * pathSize, 1, startY * pathSize);

        for (int i = 0; i < numCheckpoints; ++i)
        {
            int checkpointX;
            int checkpointY;
            checkpointList = GameObject.FindGameObjectsWithTag("Checkpoint");
            do
            {
                checkpointX = random.Next(gridSizeX);
                checkpointY = random.Next(gridSizeY);
            
                for (int x = 0; x < checkpointList.Length; ++x)//prevents multiple checkpoints from being placed in the same spot
                {
                    if (checkpointList[x].transform.position.x == checkpointX*pathSize && checkpointList[x].transform.position.z == checkpointY*pathSize)//if checkpoint already exists at this location
                    {
                        checkpointX = random.Next(gridSizeX);
                        checkpointY = random.Next(gridSizeY);
                        x = 0;
                    }
                }
            } while (checkpointX == startX && checkpointY == startY);//prevents checkpoints from being placed on top of start
            GameObject checkpointBlock = Instantiate(checkpoint);
            checkpointBlock.name = "Checkpoint " + checkpointNum.ToString();
            checkpointBlock.transform.position = new Vector3(checkpointX*pathSize, 1, checkpointY*pathSize);//places checkpoints randomly
            checkpointNum++;
        }

        checkpointList = GameObject.FindGameObjectsWithTag("Checkpoint");

        calculateSolverComplexity(maze, startX, startY, checkpointList);
        Debug.Log("Solver Average Complexity: " + solverComplexity);
    } */

    /* void placeCheckpoints(MazeSearch maze)
    {
        complexityAlgo = new ComplexityAlgo(maze);
        int startX = 0;
        int startY = 0;

        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            GameObject startBlock = Instantiate(player);
            startBlock.transform.position = new Vector3(0, 1f, 0);//places player at origin
        }
        else GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(0, 0.5f, 0);//places player at origin
        checkpointList = spawnCheckpoints(random, startX, startY);//places checkpoints randomly

        calculateSolverComplexity(maze, startX, startY, checkpointList);
        Debug.Log("Solver Average Complexity: " + solverComplexity);
    } */

    void placePlayer(MazeSearch mazeSearch, int startX, int startY)
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            GameObject startBlock = Instantiate(player);
            startBlock.transform.position = new Vector3(startX * pathSize, 1f, startY * pathSize);
        }
        else GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(startX * pathSize, 1f, startY * pathSize);

        placeEnemy(startX, startY);
    }

    private void placeEnemy(int playerX, int playerY)
    {
        //spawns enemy at farthest possible location from player
        GameObject newEnemy = GameObject.Instantiate(enemy);

        int enemyX;
        int enemyY;

        if (playerX >= ((gridSizeX - 1) / 2)) enemyX = 0;
        else enemyX = gridSizeX - 1;
        if (playerY >= ((gridSizeY - 1) / 2)) enemyY = 0;
        else enemyY = gridSizeY - 1;

        newEnemy.transform.position = new Vector3(enemyX * pathSize, 1, enemyY * pathSize);
    }
    
    /* private void calculateSolverComplexity(MazeSearch maze, int startX, int startY, GameObject[] checkpointList) 
    { 
        List<float> complexityList = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            ComplexityAlgo complexityCalc = new ComplexityAlgo(maze);  
            complexityCalc.complexityCheckpoint(startX, startY, checkpointList, pathSize);//calculates complexity of the maze
            complexityList.Add(complexityCalc.getRating());
            Debug.Log("Solution " + (i+1) + " Complexity: " + complexityCalc.getRating());//prints complexity of the maze for each solve
        }
        solverComplexity = complexityList.Average();//averages the complexity of the maze over 5 solves
    } */

    /* private GameObject[] spawnCheckpoints(System.Random random, int startX, int startY)
    {
        GameObject[] checkpointList = new GameObject[numCheckpoints];
        for (int i = 0; i < numCheckpoints; i++) //creates a list of checkpoints
        {
            checkpointList[i] = Instantiate(checkpoint);
            checkpointList[i].name = "Checkpoint " + (i + 1);
            placeRandomly(checkpointList[i], random);
        }

        for (int i = 0; i < numCheckpoints; ++i)
        {
            for (int x = 0; x < numCheckpoints; ++x)//prevents multiple checkpoints from being placed in the same spot
            {
                GameObject thisCheckpoint = checkpointList[i];
                GameObject otherCheckpoint = checkpointList[x];
                Vector3 thisCheckpointPos = checkpointList[i].transform.position;
                Vector3 otherCheckpointPos = checkpointList[x].transform.position;
                //Debug.Log("Checking checkpoint " + i + " against checkpoint " + x);

                if (x == i) continue;//prevents the checkpoint from being compared to itself
                if (otherCheckpointPos.x == thisCheckpointPos.x && otherCheckpointPos.z == thisCheckpointPos.z)//if another checkpoint already exists at this location
                {
                    placeRandomly(thisCheckpoint, random);
                    x = 0;
                    continue;
                }
                if (otherCheckpointPos.x == startX * pathSize && thisCheckpointPos.z == startY * pathSize)//if checkpoint is placed on top of the player
                {
                    placeRandomly(thisCheckpoint, random);
                    x = 0;
                    continue;
                }
            }
        }
        //if (checkpointsInOrder) foreach (GameObject checkpoint in checkpointList) checkpoint.SetActive(false);//disables checkpoints until player reaches the previous checkpoint
        List<GameObject> checkpointListSorted = checkpointList.ToList();
        for (int i = 0; i < checkpointList.Length; i++) if (checkpointList[i].name.EndsWith(i.ToString())) checkpointListSorted.Add(checkpointList[i]);//sorts checkpoints by number
        return checkpointListSorted.ToArray();
    } */

    /* private void setCheckpointActive()
    {
        Debug.Log("Setting checkpoint active");
        //gottenCheckpoints = ui.getNumPickups();
        if (gottenCheckpoints < checkpointList.Length && checkpointList[gottenCheckpoints]!=null) checkpointList[gottenCheckpoints].SetActive(true);//enables the next checkpoint when the previous checkpoint is reached
        //Debug.Log(gottenCheckpoints + " " + checkpointList[gottenCheckpoints].name);
        for (int i = (gottenCheckpoints + 1); i < checkpointList.Length; i++) if (checkpointList[i] != null) checkpointList[i].SetActive(false);//disables all checkpoints after the current checkpoint
    } */

    private void placeRandomly(GameObject block, System.Random random) { block.transform.position = new Vector3(random.Next(gridSizeX) * pathSize, 1, random.Next(gridSizeY) * pathSize); }

    private void OnGenerateMaze()
    {
        Debug.Log("OnGenerateMaze");
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Path"))
        {
            Destroy(block);
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Wall"))
        {
            Destroy(block);
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Finish"))
        {
            Destroy(block);
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            Destroy(block);
        }
        generateMaze(seed);
    }

    //public int getTotalNumCheckpoints() { return numCheckpoints; }
    //public bool getOrderedCheckpoints() { return checkpointsInOrder; }
    public int getSeed() { return seed; }
}
