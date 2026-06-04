using UnityEngine;

public class UniversalCanvasAnimationController : MonoBehaviour
{
    static Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Time.timeScale = 0f;
        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void FadeOut()
    {
        Time.timeScale = 0f;
        animator.SetTrigger("FadeOut");
    }
    public static void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }
    public void ResetTrigger(string triggerName)
    {
        if (triggerName == "FadeIn")
        {
            Time.timeScale = 1f;
        }
        animator.ResetTrigger(triggerName);
    }
}
