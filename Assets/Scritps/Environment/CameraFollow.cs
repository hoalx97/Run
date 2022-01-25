using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.zero;
    public Vector3 offset;
    public float speedFollow = 5f;
    private float maxHeightOffset;


    void LateUpdate()
    {
        Vector3 followPos = target.position + offset;

        RaycastHit hit;
        if (Physics.Raycast(target.position, Vector3.down, out hit, 2.5f))
        {
            maxHeightOffset = Mathf.Lerp(maxHeightOffset, hit.point.y, Time.deltaTime * speedFollow);
        }else
        {
            maxHeightOffset = Mathf.Lerp(maxHeightOffset, target.position.y, Time.deltaTime * speedFollow);
        }
        followPos.y = offset.y + maxHeightOffset;
        transform.position = followPos;
    }

}