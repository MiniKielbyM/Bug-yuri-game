using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;
    private Transform currentNPC;
    private bool lockedOnNPC;
    private bool transitioningToPlayer;
    void Update()
    {
        if (lockedOnNPC && currentNPC != null)
        {
            Vector3 targetPos = currentNPC.position + offset;
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
        lockedOnNPC = false;
        currentNPC = null;
        transitioningToPlayer = true;
    }
}
