using UnityEngine;

public class CoinCanvas : MonoBehaviour
{
    public Transform headsetTransform = null;
    public float angleThreshold = 30f;
    public Canvas coinCanvas = null;
    public GameObject coinObject = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (headsetTransform != null && coinCanvas != null && coinObject != null)
        {
            Vector3 toCanvas = (coinCanvas.transform.position - headsetTransform.position).normalized;
            float angle = Vector3.Angle(headsetTransform.forward, toCanvas);
            //Debug.Log("CoinCanvas, angle< " + angle);

            bool isActive = angle < angleThreshold;
            coinCanvas.enabled = isActive;
            coinObject.SetActive(isActive);
        }        
    }
}
