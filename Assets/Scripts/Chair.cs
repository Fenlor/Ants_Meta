using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class Chair : MonoBehaviour
{
    public GameObject coinEffectPrefab;    
    public TeleportationProvider teleportationProvider;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCoin(int value)
    {
        if (coinEffectPrefab != null) 
        {
            GameObject coin = Instantiate(coinEffectPrefab, transform.position, Quaternion.identity);
            coin.SetActive(true);
            coin.GetComponent<Coin>().coinValue = value;
        }

        if (teleportationProvider != null) 
        {
            TeleportRequest request = new TeleportRequest
            {
                destinationPosition = transform.position,
                destinationRotation = transform.rotation,
                matchOrientation = MatchOrientation.TargetUpAndForward,
            };

            teleportationProvider.QueueTeleportRequest(request);
            
        }
    }
}
