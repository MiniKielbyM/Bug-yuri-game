using UnityEngine;

public class OBCAppear : MonoBehaviour
{
    private Vector3 targetScale;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * 5f);
    }
    public void appear()
    {
        targetScale = new Vector3(0.2f, 0.2f, 1);
        GetComponent<Interactable>().Interact();
    }
    public void disappear()
    {
        GetComponent<Interactable>().FinishCurrentSet();
        targetScale = Vector3.zero;
    }
}
