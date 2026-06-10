using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        SaveGame.DeleteSaveFile();
        SceneManager.LoadScene(2);
    }
    public void LoadGame()
    {
        if (SaveGame.SaveFileExists())
        {
            SaveGame.LoadGame();
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
