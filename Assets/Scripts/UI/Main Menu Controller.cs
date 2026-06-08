using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        SaveGame.DeleteSaveFile();
        SceneManager.LoadScene(1);
    }
    public void LoadGame()
    {
        SaveGame.LoadGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
