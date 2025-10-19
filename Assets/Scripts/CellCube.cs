using Meta.WitAi;
using Oculus.Interaction;
using TMPro;
using UnityEngine;

//Holds the colour and direction (Left or Right only) 
//List of these elsewhere

public class CellCube : MonoBehaviour
{
    public Material currentMaterial;

    public GameObject textObject;

    private bool isSelected;

    public int currentIndex = 0;

    //public RayInteractable rayInteractable;

    public enum Direction
    {
        LEFT = 0,
        RIGHT = 1
    }
    public Direction direction = Direction.LEFT;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if(rayInteractable != null)
        //{
        //    //rayInteractable
        //}      
    }

    private void Update()
    {
        if (isSelected)
        {
            //check for left thumbstick direction
            Vector2 leftThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            if (leftThumbstick.x > 0) 
            {
                direction = Direction.RIGHT;
                ShowDirection();
            }
            else if(leftThumbstick.x < 0)
            {
                direction = Direction.LEFT;
                ShowDirection();
            }            
        }
    }

    public void SetMaterial(Material material)
    {
        currentMaterial = material;
    }
    public void SetDirection(Direction newDir)
    {
        direction = newDir;
    }

    //when hovering, allow for left and right controller positions to change direction
    public void HoverStart()
    {
        Debug.Log("CellCube - OnHover()");
    }    
    public void SetSelected(bool newIsSelected)
    {
        Debug.Log("CellCube - SetSelected: " + newIsSelected);
        isSelected = newIsSelected;
    }
    public void ShowDirection()
    {
        //configMenu.GetComponentInChildren<TextMeshPro>().text = "HELLO!";

        if (textObject != null)
        {
            textObject.GetComponent<TextMeshProUGUI>().text = "Direction: " + direction.ToString();
        }
    }
    public Direction GetDirection()
    {
        return direction;
    }
}
