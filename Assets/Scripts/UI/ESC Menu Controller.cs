using UnityEngine;

public class ESCMenuController : MonoBehaviour
{
    public GameObject ESCMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = ESCMenu.activeSelf ? 1f : 0f;
            ESCMenu.SetActive(!ESCMenu.activeSelf);
        }
    }
    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        ESCMenu.SetActive(false);
    }
    public void Save()
    {
        SaveGame.SaveCurrentGame();
        Debug.Log("Game saved.");
    }
    public async void Load()
    {
        ESCMenu.SetActive(false);
        UniversalCanvasAnimationController.FadeOut();
        await System.Threading.Tasks.Task.Delay(1000);
        SaveGame.LoadGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
