//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;


//public class ChairTeleport : MonoBehaviour
//{
//    public UnityEvent onTeleportEvent;
//    private bool hasTeleported = false;

//    public TeleportationProvider teleportationProvider;

//    private void Awake()
//    {
//        if (onTeleportEvent == null)
//        {
//            onTeleportEvent = new UnityEvent();
//        }
//    }

//    private void OnEnable()
//    {
//        if(teleportationProvider != null)
//        {
//            teleportationProvider.teleporting += OnTeleporting;
//        }
//    }

//    private void OnDisable()
//    {
//        if (teleportationProvider != null)
//        {

//        }
//    }

//    public void OnTeleporting(TeleportingEventArgs args)
//    {
//        onTeleportEvent.Invoke();
//        Debug.Log("OnTeleport: " + args);
//    }
//}
