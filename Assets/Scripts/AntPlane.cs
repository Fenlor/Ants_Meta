using System.Collections.Generic;
using System.Xml.Serialization;
using NUnit.Framework;
using UnityEngine;

public class AntPlane : MonoBehaviour
{
    //take in ground cell object
    public GameObject cellObject;
    public int gridSize = 256;
    private GameObject[,] cellGrid;

    //public Material colourOneMaterial;
    //public Material colourTwoMaterial;

    private List<Material> materials;

    //When activating ant trail, add only the used materials to this list
    //hmm instead of material list, we should get a list of active CellCubes and pull the material and direction from them
    private List<GameObject> cellCubeList;
    private int materialIndex = 0;
    private int materialCount;
    private GameObject activeCell;
    private int currentCellIndex;
    private int nextCellIndex;
    private int activeRow;
    private int activeColumn;    
    //public GameObject antObject;
    private bool isRowMove = false;

    //use timer after it works
    public float moveTimeDelta;
    private float moveTimer;
    private float stride;

    private enum DIRECTION
    {
        UP,
        DOWN, 
        LEFT, 
        RIGHT
    }
    DIRECTION dir = DIRECTION.UP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        if (cellObject != null)
        {
            cellGrid = new GameObject[gridSize, gridSize];

            //Debug.Log("AntPlane - Start() - cellGrid Length:" + cellGrid.Length);
            //Debug.Log("AntPlane - Start() - cellGrid[0] Length:" + cellGrid[0].Length);
           
            float cellSize = cellObject.GetComponent<MeshRenderer>().bounds.size.x;
            float halfCellSize = cellSize * 0.5f;
            //assume square for cell?
            //use middle of this object as middle of grid
            Vector3 startPos = transform.position; 
            float startX = transform.position.x - ((gridSize * 0.5f) * cellSize);
            float startY = transform.position.z - ((gridSize * 0.5f) * cellSize);
            startPos.x = startX;
            startPos.z = startY;

            //create 2D array of grid objects so we can change material and stuff?

            stride = cellObject.GetComponent<MeshRenderer>().bounds.size.x * 0.5f;
            for (int gridX = 0; gridX < cellGrid.GetLength(0); gridX++)
            {
                for (int gridY = 0; gridY < cellGrid.GetLength(1); gridY++)
                {
                    cellGrid[gridX, gridY] = Instantiate(cellObject, startPos, transform.rotation);
                    cellGrid[gridX, gridY].SetActive(false);
                    startPos.x += cellSize;                    
                }
                startPos.x = startX;
                startPos.z += cellSize;
            }

            //ant to find cell near center to start on
            //ant will always move from cell center to cell center, teleport to start. Lerp is easy though, might be cool looking as well.
            
            int halfCells = (int)(gridSize * 0.5f);
            activeCell = cellGrid[halfCells, halfCells];
            activeRow = halfCells;
            activeColumn = halfCells;
            //Debug.Log("AntPlane - Start() - activeRow, activeColumn: " + activeRow + ", " + activeColumn);
            //Debug.Log("AntPlane - Start() - cellGrid Length:" + cellGrid.Length);
            //Debug.Log("AntPlane - Start() - cellGrid[0][0] Object:" + cellGrid[0,0]);
        }

        //assign tags so we can tell which colour we are on?
        //or is there a better way?
        //colourOneMaterial.SetOverrideTag("SurfaceType", "ColourOne");
        //colourTwoMaterial.SetOverrideTag("SurfaceType", "ColourTwo");

        cellCubeList = new List<GameObject>();
        currentCellIndex = 0;
        nextCellIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //ant need to look at active cell, use tag in material? cant see how atm, maybe just name
        //then change colour and turn facing both depending on current colour
        //then move forward 1 cell, how to tell if facing row or coloum, could just pick one to start with and keep a hold of it

        //moveTimer += Time.deltaTime;
        //if (moveTimer > moveTimeDelta)
        //{
        //    Debug.Log("AntMove - ACTING!");
        //    moveTimer = 0f;
        //    if (activeCell != null)
        //    {
        //        AntActOnActiveCell();
        //    }
        //}
    }

    public void UpdateAntTrail()
    {

        //ant need to look at active cell, use tag in material? cant see how atm, maybe just name
        //then change colour and turn facing both depending on current colour
        //then move forward 1 cell, how to tell if facing row or coloum, could just pick one to start with and keep a hold of it

        moveTimer += Time.deltaTime;
        if (moveTimer > moveTimeDelta)
        {
           
            moveTimer = 0f;
            if (activeCell != null)
            {
                Debug.Log("AntMove - ACTING ON ACTIVE CELL!");
                AntActOnActiveCell();
            }
        }
    }

    //Call InitAntTrail just before we kick off the trail so we can update it with the newest UI info
    //public void InitAntTrail(List<Material> matList)
    //{
    //    //add to material list
    //    for(int matIndex = 0; matIndex < matList.Count; ++matIndex)
    //    {
    //        Material mat = matList[matIndex];
    //        mat.SetOverrideTag("SurfaceType", "Colour" + matIndex.ToString());
    //        materialList.Add(mat);
    //    }
    //    materialCount = materialList.Count;
    //}
    
    private void AntActOnActiveCell()
    {
        //Debug.Log("AntMove - AntActOnActiveCell, material.name: " + activeCell.GetComponent<MeshRenderer>().material.name);
        //Debug.Log("AntMove - AntActOnActiveCell, material: " + activeCell.GetComponent<MeshRenderer>().material);
        //Debug.Log("AntMove - Before row col change, isRowMove, isPosMove: " + isRowMove + ", " + isPosMove);
        //Debug.Log("AntMove - Before row col change, activeRow, activeColumn: " + activeRow + ", " + activeColumn);

        //VR PORT: OK, we have a cellList from the ControllerHandler 
        //set active cell starts off in the middle (or close enough)
        //the list we have is just to get the material and direction off it. so do we set the material tag in ControllerHandler?
        //assume a cell will always change to the next in the cycle, so can we just use an index? start at 0 for white cell
        //get direction, set cell to next index (looping at end), then move?

        Debug.Log("ActiveCell Count: " + cellCubeList.Count);
        Debug.Log("CURRENT CELL ACTIVE?: " + activeCell.activeSelf + ", " + currentCellIndex);

        if (!activeCell.activeSelf)
        {
            activeCell.SetActive(true);
        }

        currentCellIndex = activeCell.GetComponent<CellCube>().currentIndex;
        //NEED TO SET THIS PROPERLY!!! to get index? set on cellCube
        if (currentCellIndex + 1 >= cellCubeList.Count)
        {
            nextCellIndex = 0;
        }
        else
        {
            nextCellIndex = currentCellIndex + 1;
        }

        CellCube currentCellCube = cellCubeList[currentCellIndex].GetComponent<CellCube>();
        CellCube nextCellCube = cellCubeList[nextCellIndex].GetComponent<CellCube>();
        activeCell.GetComponent<MeshRenderer>().material = nextCellCube.currentMaterial;
        
        Vector3 pos = activeCell.transform.position;

        Debug.Log("CURRENT CELL DIR?: " + currentCellCube.GetDirection() + ", " + nextCellIndex);

        //OK, so we have the direction of the current colour we are on. 
        //we set the material on active Cell
        //we have this dir

        //lets run through it
        //we start with a count of 2
        //first cell activates and material gets changed to next in cycle
        //direction gets handled using current cell direction.
        //eg. first cell changes from white to black and turns left
        //now currentCellIndex++ to 1
        //we cannot then use that index to determine direction, we have to check the material...
        //do we set a direction on the instanced cell cube in the grid? probably easiest to do
        //it seems to already have the script on it

        //move depending on direction
        if (activeCell.GetComponent<CellCube>().GetDirection() == CellCube.Direction.LEFT)
        {
            RightMovement();
        }
        else
        {
            LeftMovement();
        }


        activeCell.GetComponent<CellCube>().SetDirection(nextCellCube.GetDirection());
        activeCell.GetComponent<CellCube>().currentIndex = nextCellIndex;      

        AntMove();
    }
    
    private void OriginalMovement()
    {
        switch (dir)
        {
            case DIRECTION.UP:
                activeColumn -= 1;
                dir = DIRECTION.LEFT;
                break;
            case DIRECTION.DOWN:
                activeColumn += 1;
                dir = DIRECTION.RIGHT;
                break;
            case DIRECTION.LEFT:
                activeRow += 1;
                dir = DIRECTION.DOWN;
                break;
            case DIRECTION.RIGHT:
                activeRow -= 1;
                dir = DIRECTION.UP;
                break;
            default:
                break;
        }
    }

    private void LeftMovement()
    {
        switch (dir)
        {
            case DIRECTION.UP:
                activeColumn -= 1;
                dir = DIRECTION.LEFT;
                break;
            case DIRECTION.DOWN:
                activeColumn += 1;
                dir = DIRECTION.RIGHT;
                break;
            case DIRECTION.LEFT:
                activeRow += 1;
                dir = DIRECTION.DOWN;
                break;
            case DIRECTION.RIGHT:
                activeRow -= 1;
                dir = DIRECTION.UP;
                break;
            default:
                break;
        }
    }

    private void RightMovement()
    {
        switch (dir)
        {
            case DIRECTION.UP:
                activeColumn += 1;
                dir = DIRECTION.RIGHT;
                break;
            case DIRECTION.DOWN:
                activeColumn -= 1;
                dir = DIRECTION.LEFT;
                break;
            case DIRECTION.LEFT:
                //pos.y += stride * 2f;
                //activeCell.transform.position = pos;
                activeRow -= 1;
                dir = DIRECTION.UP;
                break;
            case DIRECTION.RIGHT:
                //pos.y -= stride * 2f;
                //activeCell.transform.position = pos;
                activeRow += 1;
                dir = DIRECTION.DOWN;
                break;
            default:
                break;
        }
    }

    private void AntMove()
    {
        //Debug.Log("AntMove - activeRow, activeColumn: " + activeRow + ", " + activeColumn);
        
        if (activeColumn >= 1 && activeColumn < gridSize-1)
        {               
            activeCell = cellGrid[activeRow, activeColumn];           
        }
        else
        {
            //hit an edge, what do we do?
        }
    }

    public void SetCellCubeList(List<GameObject> cubeList)
    {
        cellCubeList = cubeList;
    }

    public void SetMaterials(List<Material> mats)
    {
        materials = mats;
    }
}
