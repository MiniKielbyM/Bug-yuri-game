using NaughtyAttributes;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

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
    public UnityEvent function;
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
    [ShowIf("interactableType", InteractableType.Door)]
    public UnityEvent pointer;
    [ShowIf("interactableType", InteractableType.NPC)]
    public NPCDialogueSet[] npcDialogueSets;
    [ShowIf("interactableType", InteractableType.NPC)]
    public Sprite npcSprite;
    [ShowIf("interactableType", InteractableType.NPC)]
    public Sprite playerSprite;
    [ShowIf("interactableType", InteractableType.NPC)]
    public GameObject dialogueUI;
    [ShowIf("interactableType", InteractableType.NPC)]
    public int currentDialogueIndex = 0;
    [ShowIf("interactableType", InteractableType.NPC)]
    public int currentDialogueSet = 0;
    private GameObject player;
    public void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    public void setDialogueSet(int newSet)
    {
        currentDialogueSet = newSet;
        currentDialogueIndex = 0;
        FinishCurrentSet();
    }
    public void Interact()
    {
        player.GetComponent<KeyboardControls>().interactObject = gameObject;
        Debug.Log("Interacted with " + gameObject.name);
        if (interactableType == InteractableType.Door)
        {
            if (pointer != null)
            {
                pointer.Invoke();
            }
            Debug.Log("Player moved to new position: " + playerSpawnPosition);
            player.GetComponent<KeyboardControls>().interactText.SetActive(false);
            StartCoroutine(TeleportPlayerWithFade());
        }
        else if (interactableType == InteractableType.NPC)
        {
            currentDialogueIndex = 0;
            Debug.Log("Starting dialogue with " + gameObject.name);
            Time.timeScale = 0f;
            dialogueUI.SetActive(true);
            AdvanceDialogue();
            player.GetComponent<KeyboardControls>().Interacting = true;
            player.GetComponent<KeyboardControls>().interactObject = gameObject;
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
            if (currentSet.dialogues[currentDialogueIndex].function != null)
            {
                currentSet.dialogues[currentDialogueIndex].function?.Invoke();
            }
            Debug.Log("Showing dialogue: " + currentSet.dialogues[currentDialogueIndex].dialogueText);
            NPCDialogue dialogue = currentSet.dialogues[currentDialogueIndex];
            dialogueUI.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = dialogue.npcType == NPCType.NPC ? npcSprite : playerSprite;
            dialogueUI.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = dialogue.dialogueText;
            Camera.main.gameObject.GetComponent<CameraFollowPlayer>().LockOnNPC(dialogue.npcType == NPCType.NPC ? gameObject.transform : player.transform);
            currentDialogueIndex++;
        }
        else
        {
            Debug.Log("Dialogue set completed for " + gameObject.name);
            Time.timeScale = 1f;
            dialogueUI.SetActive(false);

            Camera.main.gameObject.GetComponent<CameraFollowPlayer>().UnlockFromNPC();
            currentDialogueSet = currentSet.nextSetIndex;
            player.GetComponent<KeyboardControls>().interactText.SetActive(true);
            player.GetComponent<KeyboardControls>().Interacting = false;
        }
    }
    public void FinishCurrentSet()
    {
        NPCDialogueSet currentSet = npcDialogueSets[currentDialogueSet];
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().UnlockFromNPC();
        currentDialogueSet = currentSet.nextSetIndex;
    }
    public IEnumerator TeleportPlayerWithFade()
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