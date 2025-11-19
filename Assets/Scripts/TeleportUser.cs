using UnityEngine;

public class TeleportUser : MonoBehaviour
{
    public Transform cameraRig;

    public void TeleportTo(Vector3 targetPos, Quaternion orientation)
    {
        Vector3 cameraOffset = cameraRig.GetComponentInChildren<Camera>().transform.localPosition;
        Vector3 offset = new Vector3(cameraOffset.x, 0, cameraOffset.z);
        cameraRig.transform.position = targetPos - offset;
        cameraRig.transform.rotation = orientation;

        SnapToGround();
    }

    public void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraRig.transform.position, Vector3.down, out hit, 5f))
        {
            Vector3 groundedPosition = new Vector3(
                cameraRig.transform.position.x,
                hit.point.y,
                cameraRig.transform.position.z
            );
            cameraRig.transform.position = groundedPosition;
        }
    }
}