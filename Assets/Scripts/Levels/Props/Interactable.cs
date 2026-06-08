using NaughtyAttributes;
using UnityEngine;
using System.Collections;

public enum InteractableType
{
    Door,
    NPC
}

public enum NPCType
{
    Player,
    NPC
}

public enum CollisionType
{
    Enter,
    Stay,
    Exit
}

public enum AreaTriggerCondition
{
    None,
    EnemyDefeated,
    ItemCollected
}

[System.Serializable]
public struct NPCDialogue
{
    public NPCType npcType;
    public string dialogueText;
}

[System.Serializable]
public struct NPCDialogueSet
{
    public int nextSetIndex;
    public NPCDialogue[] dialogues;
}

public class Interactable : MonoBehaviour
{

    public InteractableType interactableType;


    [ShowIf("interactableType", InteractableType.Door)]
    public Vector3 playerSpawnPosition;
    [ShowIf("interactableType", InteractableType.Door)]
    public bool changeLevel = false;
    [ShowIf("changeLevel")]
    public string nextLevelName;

    [ShowIf("interactableType", InteractableType.NPC)]
    public NPCDialogueSet[] npcDialogueSets;
    [ShowIf("interactableType", InteractableType.NPC)]
    public Sprite npcSprite;
    [ShowIf("interactableType", InteractableType.NPC)]
    public Sprite playerSprite;
    [ShowIf("interactableType", InteractableType.NPC)]
    public GameObject dialogueUI;


    private int currentDialogueIndex = 0;
    private int currentDialogueSet = 0;
    private GameObject player;
    public void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    public void Interact()
    {

        Debug.Log("Interacted with " + gameObject.name);
        if (interactableType == InteractableType.Door)
        {
            Debug.Log("Player moved to new position: " + playerSpawnPosition);
            player.GetComponent<KeyboardControls>().interactText.SetActive(false);
            StartCoroutine(TeleportPlayerWithFade());
        }
        else if (interactableType == InteractableType.NPC)
        {
            Debug.Log("Starting dialogue with " + gameObject.name);
            Time.timeScale = 0f;
            dialogueUI.SetActive(true);
            AdvanceDialogue();
            player.GetComponent<KeyboardControls>().Interacting = true;
            player.GetComponent<KeyboardControls>().interactText.SetActive(false);
        }
    }
    public void AdvanceDialogue()
    {
        if (interactableType != InteractableType.NPC || dialogueUI == null)
            return;
        Debug.Log("Advancing dialogue for " + gameObject.name);
        NPCDialogueSet currentSet = npcDialogueSets[currentDialogueSet];
        if (currentDialogueIndex < currentSet.dialogues.Length)
        {
            Debug.Log("Showing dialogue: " + currentSet.dialogues[currentDialogueIndex].dialogueText);
            NPCDialogue dialogue = currentSet.dialogues[currentDialogueIndex];
            dialogueUI.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = dialogue.npcType == NPCType.NPC ? npcSprite : playerSprite;
            dialogueUI.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = dialogue.dialogueText;
            currentDialogueIndex++;
        }
        else
        {
            Debug.Log("Dialogue set completed for " + gameObject.name);
            Time.timeScale = 1f;
            dialogueUI.SetActive(false);
            currentDialogueIndex = 0;
            currentDialogueSet = currentSet.nextSetIndex;
            player.GetComponent<KeyboardControls>().interactText.SetActive(true);
            player.GetComponent<KeyboardControls>().Interacting = false;
        }
    }

    private IEnumerator TeleportPlayerWithFade()
    {
        UniversalCanvasAnimationController.FadeOut();
        yield return new WaitForSecondsRealtime(1f);
        if (changeLevel && !string.IsNullOrEmpty(nextLevelName))
        {
            Time.timeScale = 1f;
            SaveGame.SetNextSpawnPosition(playerSpawnPosition);
            SaveGame.SaveCurrentGame(nextLevelName);
        }
        else
        {
            player.transform.position = playerSpawnPosition;
            SaveGame.SetNextSpawnPosition(playerSpawnPosition);
            SaveGame.SaveCurrentGame();
            UniversalCanvasAnimationController.FadeIn();
        }
    }
}