using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;
    public Transform currentNPC;
    public bool lockedOnNPC;
    public bool transitioningToPlayer;
    void Update()
    {
        if (lockedOnNPC && currentNPC != null)
        {
            Vector3 targetPos = currentNPC.position;
            targetPos.z = transform.position.z;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                5f * Time.unscaledDeltaTime
            );
        }
        else if (transitioningToPlayer)
        {
            Vector3 targetPos = playerTransform.position + offset;
            targetPos.z = transform.position.z;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                5f * Time.unscaledDeltaTime
            );

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                transitioningToPlayer = false;
            }
        }
        else
        {
            Vector3 targetPos = playerTransform.position + offset;
            targetPos.z = transform.position.z;

            transform.position = targetPos;
        }
    }

    public void LockOnNPC(Transform npcTransform)
    {
        currentNPC = npcTransform;
        lockedOnNPC = true;
        transitioningToPlayer = false;
    }

    public void UnlockFromNPC()
    {
        Debug.Log("test");
        lockedOnNPC = false;
        currentNPC = null;
        transitioningToPlayer = true;
    }
}
