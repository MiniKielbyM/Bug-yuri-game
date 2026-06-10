using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveData
{
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;
    public string currentScene;
    public int currentDialogueSet = 0;
    public int currentDialogueIndex = 0;
    public string levelCheckpoint = "start";
    public bool hasSpawnPosition = false;
    public float spawnPositionX;
    public float spawnPositionY;
    public float spawnPositionZ;
}

[System.Serializable]
public class EnemySaveData
{
    public string enemyID;
}

[System.Serializable]
public class GameSaveData
{
    public PlayerSaveData playerData;
    public string saveTimestamp;
    public string sessionID;

    public List<EnemySaveData> deadEnemies = new List<EnemySaveData>();
}

public class SaveGame : MonoBehaviour
{
    private static string savePath = "";
    private static string currentSessionID = ":3";
    private static bool isInitialized = false;
    private static Vector3? tempSpawnPosition = null;

    private static GameSaveData pendingLoadData = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EnsureInitialized();
    }

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
                currentSessionID = ":3";
            }

            isInitialized = true;
        }
    }

    public static void InitializeNewSession()
    {
        EnsureInitialized();
        currentSessionID = ":3";
    }

    public static void SaveCurrentGame(string checkpoint = "default")
    {
        EnsureInitialized();

        try
        {
            GameSaveData saveData = new GameSaveData();

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
            }

            foreach (string deadEnemyID in EnemyManager.DeadEnemies)
            {
                saveData.deadEnemies.Add(new EnemySaveData
                {
                    enemyID = deadEnemyID
                });
            }

            saveData.saveTimestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            saveData.sessionID = ":3";

            string json = JsonUtility.ToJson(saveData, true);

            string directory = Path.GetDirectoryName(savePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(savePath, json);

            Debug.Log("Game saved.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    public static GameSaveData LoadGame(bool reloadScene = true)
    {
        EnsureInitialized();

        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found.");
            return null;
        }

        string json = File.ReadAllText(savePath);

        pendingLoadData =
            JsonUtility.FromJson<GameSaveData>(json);

        Instance.StartCoroutine(
            LoadSaveRoutine(reloadScene)
        );

        return pendingLoadData;
    }

    private static IEnumerator LoadSaveRoutine(bool reload)
    {
        if (pendingLoadData == null)
            yield break;

        EnemyManager.Clear();

        foreach (EnemySaveData enemy in pendingLoadData.deadEnemies)
        {
            EnemyManager.MarkDead(enemy.enemyID);
        }

        if (reload)
        {
            AsyncOperation loadOperation =
                SceneManager.LoadSceneAsync(pendingLoadData.playerData.currentScene);

            while (!loadOperation.isDone)
                yield return null;
        }

        ApplySaveData(pendingLoadData);
    }

    public static void ApplySaveData(GameSaveData saveData)
    {

        if (saveData == null || saveData.playerData == null)
            return;
        Debug.Log($"Save contains {saveData.deadEnemies.Count} dead enemies");

        EnemyManager.Clear();

        foreach (EnemySaveData enemy in saveData.deadEnemies)
        {
            Debug.Log($"Loading dead enemy ID: {enemy.enemyID}");
            EnemyManager.MarkDead(enemy.enemyID);
        }
        EnemyManager.Clear();

        foreach (EnemySaveData enemy in saveData.deadEnemies)
        {
            EnemyManager.MarkDead(enemy.enemyID);
        }

        Debug.Log($"Before Clear: {EnemyManager.DeadEnemies.Count}");

        EnemyManager.Clear();

        Debug.Log($"After Clear: {EnemyManager.DeadEnemies.Count}");

        foreach (EnemySaveData enemy in saveData.deadEnemies)
        {
            EnemyManager.MarkDead(enemy.enemyID);
        }

        Debug.Log($"After Load: {EnemyManager.DeadEnemies.Count}");

        EnemySaveID[] enemies =
            Object.FindObjectsOfType<EnemySaveID>();

        foreach (EnemySaveID enemy in enemies)
        {
            if (EnemyManager.DeadEnemies.Contains(enemy.enemyID))
            {
                Object.Destroy(enemy.gameObject);
            }
        }

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            player.transform.position = new Vector3(
                saveData.playerData.playerPositionX,
                saveData.playerData.playerPositionY,
                saveData.playerData.playerPositionZ
            );
        }
    }

    public static bool SaveFileExists()
    {
        EnsureInitialized();
        return File.Exists(savePath);
    }

    private static bool IsSaveFromCurrentSession(GameSaveData saveData)
    {
        return saveData != null && saveData.sessionID == ":3";
    }

    public static void HandlePlayerDeath()
    {
        EnsureInitialized();

        GameSaveData saveData = LoadGame();

        if (saveData != null &&
            IsSaveFromCurrentSession(saveData))
        {
            Debug.Log("Loading last save...");
        }
        else
        {
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

    public static void SetNextSpawnPosition(Vector3 position)
    {
        tempSpawnPosition = position;
    }

    public static Vector3? GetAndClearTempSpawnPosition()
    {
        Vector3? result = tempSpawnPosition;
        tempSpawnPosition = null;
        return result;
    }

    private static IEnumerator RestartLevelAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public static void DeleteSaveFile()
    {
        EnsureInitialized();

        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Delete failed: {e.Message}");
        }
    }
}