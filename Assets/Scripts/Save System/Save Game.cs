using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class PlayerSaveData
{
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;
    public string currentScene;
    public int currentDialogueSet = 0;
    public int currentDialogueIndex = 0;
    public string levelCheckpoint = "start"; // Track checkpoint/level position
    // Add more fields as needed (health, inventory, etc.)
}

[System.Serializable]
public class GameSaveData
{
    public PlayerSaveData playerData;
    public string saveTimestamp;
    public string sessionID; // Unique ID for this playthrough
}

public class SaveGame : MonoBehaviour
{
    private static string savePath = "";
    private static string currentSessionID = "";
    private static bool isInitialized = false;

    /// <summary>
    /// Initializes static fields on first use
    /// </summary>
    private static void EnsureInitialized()
    {
        if (!isInitialized)
        {
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
            }

            if (string.IsNullOrEmpty(currentSessionID))
            {
                currentSessionID = System.Guid.NewGuid().ToString();
            }

            isInitialized = true;
            Debug.Log($"SaveGame system initialized. Session ID: {currentSessionID}");
        }
    }

    private void Awake()
    {
        EnsureInitialized();
    }

    /// <summary>
    /// Initialize a new game session (call this at the start of a new game)
    /// </summary>
    public static void InitializeNewSession()
    {
        EnsureInitialized();
        currentSessionID = System.Guid.NewGuid().ToString();
        Debug.Log($"New game session started with ID: {currentSessionID}");
    }

    /// <summary>
    /// Saves the current game state to a JSON file
    /// </summary>
    public static void SaveCurrentGame(string checkpoint = "default")
    {
        EnsureInitialized();
        try
        {
            GameSaveData saveData = new GameSaveData();

            // Capture player data
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                saveData.playerData = new PlayerSaveData
                {
                    playerPositionX = player.transform.position.x,
                    playerPositionY = player.transform.position.y,
                    playerPositionZ = player.transform.position.z,
                    currentScene = SceneManager.GetActiveScene().name,
                    levelCheckpoint = checkpoint
                };

                // Capture dialogue progress if player has keyboard controls
                KeyboardControls keyboardControls = player.GetComponent<KeyboardControls>();
                if (keyboardControls != null && keyboardControls.interactObject != null)
                {
                    Interactable interactable = keyboardControls.interactObject.GetComponent<Interactable>();
                    if (interactable != null && interactable.interactableType == InteractableType.NPC)
                    {
                        // Save dialogue progress
                        saveData.playerData.currentDialogueSet = 0;
                        saveData.playerData.currentDialogueIndex = 0;
                    }
                }
            }

            saveData.saveTimestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            saveData.sessionID = currentSessionID;

            // Serialize to JSON
            string json = JsonUtility.ToJson(saveData, true);

            // Ensure directory exists
            string directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write to file
            File.WriteAllText(savePath, json);
            Debug.Log($"Game saved successfully at: {savePath} (Checkpoint: {checkpoint}, Session: {currentSessionID})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Loads the saved game state from the JSON file
    /// </summary>
    public static GameSaveData LoadGame()
    {
        EnsureInitialized();
        try
        {
            if (!File.Exists(savePath))
            {
                Debug.LogWarning("Save file not found!");
                return null;
            }

            string json = File.ReadAllText(savePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            Debug.Log($"Game loaded successfully from: {savePath}");
            return saveData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Applies loaded save data to the current game
    /// </summary>
    public static void ApplySaveData(GameSaveData saveData)
    {
        if (saveData == null || saveData.playerData == null)
        {
            Debug.LogWarning("No save data to apply!");
            return;
        }

        try
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector3 newPosition = new Vector3(
                    saveData.playerData.playerPositionX,
                    saveData.playerData.playerPositionY,
                    saveData.playerData.playerPositionZ
                );
                player.transform.position = newPosition;
                Debug.Log($"Player position restored to: {newPosition}");

                // Restore health or other player state if needed
                // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                // if (playerHealth != null)
                // {
                //     playerHealth.ResetHealth();
                // }
            }

            // If you need to load a different scene, uncomment:
            // if (!string.IsNullOrEmpty(saveData.playerData.currentScene))
            // {
            //     SceneManager.LoadScene(saveData.playerData.currentScene);
            // }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to apply save data: {e.Message}");
        }
    }

    /// <summary>
    /// Checks if a save file exists
    /// </summary>
    public static bool SaveFileExists()
    {
        EnsureInitialized();
        return File.Exists(savePath);
    }

    /// <summary>
    /// Checks if the save file is from the current session
    /// </summary>
    private static bool IsSaveFromCurrentSession(GameSaveData saveData)
    {
        return saveData != null && saveData.sessionID == currentSessionID;
    }

    /// <summary>
    /// Handles player death and respawn logic
    /// If save exists in current session, respawn at last save
    /// Otherwise, restart the level from the beginning
    /// </summary>
    public static void HandlePlayerDeath()
    {
        EnsureInitialized();
        GameSaveData saveData = LoadGame();

        // Check if save exists and is from current session
        if (saveData != null && IsSaveFromCurrentSession(saveData))
        {
            Debug.Log("Respawning at last checkpoint...");
            // Use a small delay to allow fade animation to play
            Instance.StartCoroutine(RespawnAtCheckpointAfterDelay(saveData));
        }
        else
        {
            Debug.Log("No valid save found for this session. Restarting level...");
            // Use a small delay to allow fade animation to play
            Instance.StartCoroutine(RestartLevelAfterDelay());
        }
    }

    private static SaveGame Instance
    {
        get
        {
            SaveGame instance = FindObjectOfType<SaveGame>();
            if (instance == null)
            {
                GameObject go = new GameObject("SaveGame");
                instance = go.AddComponent<SaveGame>();
            }
            return instance;
        }
    }

    private static System.Collections.IEnumerator RespawnAtCheckpointAfterDelay(GameSaveData saveData)
    {
        yield return new WaitForSecondsRealtime(1f);
        ApplySaveData(saveData);
        Time.timeScale = 1f;
    }

    private static System.Collections.IEnumerator RestartLevelAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Deletes the save file
    /// </summary>
    public static void DeleteSaveFile()
    {
        EnsureInitialized();
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Save file deleted successfully.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to delete save file: {e.Message}");
        }
    }
}
