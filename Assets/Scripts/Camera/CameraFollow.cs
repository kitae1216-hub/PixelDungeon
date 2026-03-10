using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 1.0f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = target.position + offset;

        if (!followX)
            targetPosition.x = currentPosition.x;

        if (!followY)
            targetPosition.y = currentPosition.y;

        targetPosition.z = offset.z;

        transform.position = Vector3.SmoothDamp(
            currentPosition,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}