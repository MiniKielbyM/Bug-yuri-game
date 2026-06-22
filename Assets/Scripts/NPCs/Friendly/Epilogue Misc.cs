using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilogueMisc : MonoBehaviour
{
    public void EndGame()
    {
        UniversalCanvasAnimationController.FadeOut();
        Time.timeScale = 0f;
        StartCoroutine(waitToSwitch(1f));
    }

    private IEnumerator waitToSwitch(float s)
    {
        yield return new WaitForSecondsRealtime(s);

        SaveGame.DeleteSaveFile();
        SceneManager.LoadScene(0);
    }
}
