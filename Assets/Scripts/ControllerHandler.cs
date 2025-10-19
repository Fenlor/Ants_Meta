using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerHandler : MonoBehaviour
{
    public Camera cam;
    private Vector3 targetPos;
    private Quaternion targetRot;
    private float step;

    //cellCubeList ranges in size from 2-12
    //how about it is always 12 but we also hold a value for CellCubes being used
    //make CellCube class that holds colour and direction    
    public GameObject cellCubeObject = null;
    private List<GameObject> cellCubeList = new List<GameObject>();      
    public List<Material> possibleMaterials = new List<Material>();
    public GameObject offCameraPositionObject;
    public GameObject cellCubeConfigPosObject;

    public Slider cellCubeSlider = null;
    public int cellCubeCount = 2;

    public GameObject configMenu = null;
    private int maxCubes = 12;
    public Transform leftHandAnchor = null;
    public Transform rightHandAnchor = null;

    public GameObject AntPlaneObject = null;
    private bool isAntPlaneActive = true;
    private bool hasAntPlaneInit = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = cam.transform.position + cam.transform.forward * 3.0f;

        //set colours here, correspond direction to particular color
        //use possibleMaterials to create cellCubes with colour and direction (alternate direction to start?)
        //then we can use UI to change material, which will also update direction
        //are we showing this to the user?

        //initialise all the CellCube objects with materials and directions even if user isnt going to use them
        //then user can change count and choose which objects to use 
        //how does the user do this?
        //can we show the CellCubes in scene (limited to current cellCubeCount) and when the user ray selects one they can change the material and direction

        //Debug.Log("ControllerHandler - Start()");

        //initialse all possible CellCubes and disable them
        if (cellCubeObject != null)
        {
            //Debug.Log("ControllerHandler - cellCubeObject != null");
            for (int cubeIndex = 0; cubeIndex < 12; ++cubeIndex)
            {
                
                //Debug.Log("ControllerHandler - cubeIndex: " + cubeIndex);
                GameObject cellCube = Instantiate(cellCubeObject, cellCubeConfigPosObject.transform);
                //cellCube.transform.position = cellCubeConfigPosObject.transform.position;

                Vector3 offset = cellCubeConfigPosObject.transform.position;
                offset.x += 0.03f * cubeIndex;
                cellCube.transform.position = offset;

                cellCube.GetComponent<MeshRenderer>().material = possibleMaterials[cubeIndex];
                cellCube.GetComponent<CellCube>().SetMaterial(possibleMaterials[cubeIndex]);
                //if (cubeIndex %2 != 0)
                //{
                //    cellCube.GetComponent<CellCube>().SetDirection(CellCube.Direction.RIGHT);
                //}
                               
                if(cubeIndex == 2)
                {
                    cellCube.GetComponent<CellCube>().SetDirection(CellCube.Direction.RIGHT);
                }
                else if(cubeIndex == 3)
                {
                    cellCube.GetComponent<CellCube>().SetDirection(CellCube.Direction.RIGHT);
                }
                cellCube.SetActive(false);
                cellCubeList.Add(cellCube);            
            }
        }

        cellCubeSlider.onValueChanged.AddListener(delegate { UpdateCellCubeCount(); });

       
    }

    // Update is called once per frame
    void Update()
    {
        //so for ant, dont worry about rotation
        //can we detect what we are raycasting onto here?
        //if so we can then use inputs to call public events that can be linked to the CellCube object
        //CellCube object can act like the start of the ant trail

        //OR what about if we have UI elements to configure the ant trail?
        //i.e.
        //-number of tiles (2-12)
        //-colour for each number of tiles (colour and direction go hand in hand) and the colour array is also the sequence of tile flipping
        //then can grab a "shoot" the cube at a wall or something, then another cube respawns and you can chuck more than 1 ant, they can interact with the others trail

        //UI Slider dictates how big CellCube list is. gets updated when slider changes

        //SHOW CUBES IN SCENE
        //Activate amount of cubes selected, position based on camera OR maybe just place above canvas
        //should be able to config a cube in canvas, can we select with ray and then change canvas elements and then back to cube?

        for (int cubeIndex = 0; cubeIndex < cellCubeCount; ++cubeIndex)
        {
            //Debug.Log("ControllerHandler - UPDATE() - cubeIndex: " + cubeIndex);
            GameObject cellCube = cellCubeList[cubeIndex];
            if (cellCube.activeSelf == false)
            {                
                cellCube.SetActive(true);
            }
        }
        //int deactivateAmount = maxCubes - cellCubeCount;
        for (int cubeIndex = cellCubeCount; cubeIndex < maxCubes; ++cubeIndex)
        {
            GameObject cellCube = cellCubeList[cubeIndex];
            if (cellCube.activeSelf == true)
            {
                cellCube.SetActive(false);
            }
        }

        step += 5.0f * Time.deltaTime;

        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            CenterCube();
        }

        RotateCube();

        //add haptic on A button release
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            OVRInput.SetControllerVibration(0.1f, 1, OVRInput.Controller.RTouch);
            isAntPlaneActive = true;
        }

        //Left controller trigger
        if(OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.0f)
        {
            transform.position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        }

        //right trigger to start off a trail? should have muiltple ant plains and if pointing at one, set it off
        //build them like walls? trap our player inside?
        //make it work with 1 AntPlane first though
        //so use ray interaction on AntPlane? when selected start ant trail
        //if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.0f)
        //{
        //    isAntPlaneActive = true;
        //}

        //attach to hand
        //OVRInput.Controller.LHand
        if (configMenu != null)
        {
            //configMenu.transform.position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            //configMenu.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);

            //use hand anchors?

            if(leftHandAnchor != null)
            {
                configMenu.transform.position = leftHandAnchor.position;
                configMenu.transform.rotation = leftHandAnchor.rotation;
            }
        }

        if (isAntPlaneActive)
        {
            if (AntPlaneObject != null)
            {
                StartAntTrail();
                AntPlane antPlane = AntPlaneObject.GetComponent<AntPlane>();
                if (antPlane != null)
                {
                    Debug.Log("Updating Ant Trail");
                    antPlane.UpdateAntTrail();
                }
            }
        }
    }

    void CenterCube()
    {
        targetPos = cam.transform.position + cam.transform.forward * 3.0f;
        targetRot = Quaternion.LookRotation(transform.position - cam.transform.position);
        transform.position = Vector3.Lerp(transform.position, targetPos, step);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, step);
    }

    void RotateCube()
    {
        if (OVRInput.Get(OVRInput.RawButton.RThumbstickLeft))
        {
            transform.Rotate(0, 5.0f * step, 0);
        }
        if (OVRInput.Get(OVRInput.RawButton.RThumbstickRight))
        {
            transform.Rotate(0, -5.0f * step, 0);
        }
    }

    public void UpdateCellCubeCount()
    {
        cellCubeCount = (int)cellCubeSlider.value;
    }

    //pass in AntTrail index? or could call this directly on AntPlane object, just need to make sure that the mats and dirs are set.
    public void StartAntTrail()
    {
        //initialise ant trail and set to update
        if(AntPlaneObject != null)
        {
            AntPlane antPlane = AntPlaneObject.GetComponent<AntPlane>();
            if (antPlane != null && !hasAntPlaneInit)
            {
                antPlane.SetCellCubeList(GetActiveCubeCells());
                hasAntPlaneInit = true;
            }
        }
    }
    
    //CubeCell list needs to have all the set materials and directions
    //AntPlane can then check what cell it is on and change to the next in the cycle

    public List<GameObject> GetActiveCubeCells()
    {
        List<GameObject> cubeList = new List<GameObject>();
        for(int cubeIndex = 0; cubeIndex < cellCubeCount; cubeIndex++)
        {
            GameObject cube = cellCubeList[cubeIndex];
            if (cube.activeSelf)
            {
                cubeList.Add(cube);
            }
        }

        return cubeList;
    }       

    public void SetAntTrailActive()
    {
        isAntPlaneActive = true;
    }
}
