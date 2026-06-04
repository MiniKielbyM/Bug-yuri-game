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
            Time.timeScale = ESCMenu.activeSelf ? 1f : 0f;
            ESCMenu.SetActive(!ESCMenu.activeSelf);
        }
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        ESCMenu.SetActive(false);
    }
    public void Save()
    {
        SaveGame.SaveCurrentGame();
        Debug.Log("Game saved.");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
