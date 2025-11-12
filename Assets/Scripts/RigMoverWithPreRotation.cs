using UnityEngine;
using System.Collections;

public class RigMoverWithPreRotation : MonoBehaviour
{
    public Transform cameraRig;           // Meta Camera Rig
    public Transform targetTransform;     // Target position and rotation
    public float rotateToFaceDuration = 1f;
    public float moveDuration = 2f;
    public float finalRotateDuration = 1f;

    private bool isMoving = false;

    public void StartFullTransition()
    {
        if (!isMoving && cameraRig != null && targetTransform != null)
        {
            StartCoroutine(RotateFaceThenMoveThenRotate(cameraRig, targetTransform));
        }
    }

    private IEnumerator RotateFaceThenMoveThenRotate(Transform rig, Transform target)
    {
        isMoving = true;

        // Step 1: Rotate to face target position
        Vector3 directionToTarget = (target.position - rig.position).normalized;
        directionToTarget.y = 0; // Keep rotation horizontal
        Quaternion startRotation = rig.rotation;
        Quaternion faceTargetRotation = Quaternion.LookRotation(directionToTarget);

        float elapsed = 0f;
        while (elapsed < rotateToFaceDuration)
        {
            rig.rotation = Quaternion.Slerp(startRotation, faceTargetRotation, elapsed / rotateToFaceDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rig.rotation = faceTargetRotation;

        // Step 2: Smooth movement to target position
        Transform centerEye = rig.GetComponentInChildren<Camera>().transform;
        Vector3 headsetOffset = new Vector3(centerEye.localPosition.x, 0, centerEye.localPosition.z);
        Vector3 adjustedEndPos = target.position - headsetOffset;

        Vector3 startPos = rig.position;
        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            rig.position = Vector3.Lerp(startPos, adjustedEndPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rig.position = adjustedEndPos;

        // Step 3: Final rotation to match target orientation
        Quaternion finalRotation = target.rotation;
        startRotation = rig.rotation;
        elapsed = 0f;
        while (elapsed < finalRotateDuration)
        {
            rig.rotation = Quaternion.Slerp(startRotation, finalRotation, elapsed / finalRotateDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rig.rotation = finalRotation;

        isMoving = false;
    }
}