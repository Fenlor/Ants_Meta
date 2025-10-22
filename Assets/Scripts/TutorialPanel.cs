using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float speed = 1.0f;
    private float startPosY;

    //random the start time by a small amount
    private float startOffset;
    private float startTimer = 0f;

    private bool isSelected = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosY = transform.position.y;
        //Random rand = new Random(); 
        //Random.InitState(int )
        //startOffset = Random.Range(0f, 5f);
        Debug.Log("startOffset" + startOffset);
    }

    // Update is called once per frame
    void Update()
    {
        //startTimer += Time.deltaTime;
        //if (startTimer > startOffset)
        //{
        //    Vector3 newPos = transform.position;
        //    newPos.y = startPosY + Mathf.Sin(Time.time * speed) * amplitude;
        //    transform.position = newPos;
        //}        
    }

    public void Selected()
    {
        isSelected = true;  
        gameObject.SetActive(false);
    }

    public void SelectedExit()
    {
        isSelected = false;
    }

    public bool IsSelected() { return isSelected; }
}
