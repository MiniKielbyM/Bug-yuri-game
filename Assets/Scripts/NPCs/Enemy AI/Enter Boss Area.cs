using UnityEngine;
using System.Collections;

public class EnterBossArea : MonoBehaviour
{
    public GameObject[] WallsToActivate;
    public NPCDialogue[] BossIntro;
    public NPCDialogue[] BossExtro;
    private bool hasActivated = false;
    public bool bossDefeated = false;
    public GameObject dialogueUI;
    public GameObject player;
    public GameObject boss;
    public Sprite npcSprite;
    public Sprite playerSprite;
    private int currentDialogueIndex = 0;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasActivated && other.CompareTag("Player"))
        {
            foreach (GameObject wall in WallsToActivate)
            {
                wall.SetActive(true);
            }
            Debug.Log("Starting dialogue with " + gameObject.name);
            Time.timeScale = 0f;
            dialogueUI.SetActive(true);
            AdvanceDialogue();
            player.GetComponent<KeyboardControls>().Interacting = true;
            player.GetComponent<KeyboardControls>().interactObject = gameObject;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            player.GetComponent<KeyboardControls>().inBossDialogue = true;
            hasActivated = true;
        }
    }
    public void BossDefeat()
    {
        foreach (GameObject wall in WallsToActivate)
        {
            wall.SetActive(false);
        }
        currentDialogueIndex = 0;
        bossDefeated = true;
        Debug.Log("Starting dialogue with " + gameObject.name);
        Time.timeScale = 0f;
        dialogueUI.SetActive(true);
        AdvanceDialogue();
        player.GetComponent<KeyboardControls>().Interacting = true;
        player.GetComponent<KeyboardControls>().interactObject = gameObject;
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player.GetComponent<KeyboardControls>().inBossDialogue = true;

    }
    public void AdvanceDialogue()
    {
        NPCDialogue[] dia = BossIntro;
        if (bossDefeated)
        {
            dia = BossExtro;
        }
        Debug.Log("Advancing dialogue for " + gameObject.name);
        if (currentDialogueIndex < dia.Length)
        {

            Debug.Log("Showing dialogue: " + dia[currentDialogueIndex].dialogueText);
            NPCDialogue dialogue = dia[currentDialogueIndex];
            if (dialogue.npcType == NPCType.NPC)
            {
                Camera.main.gameObject.GetComponent<CameraFollowPlayer>().LockOnNPC(boss.transform);
            }
            else
            {
                Camera.main.gameObject.GetComponent<CameraFollowPlayer>().LockOnNPC(player.transform);
            }
            dialogueUI.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = dialogue.npcType == NPCType.NPC ? npcSprite : playerSprite;
            dialogueUI.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = dialogue.dialogueText;
            if (dia[currentDialogueIndex].function != null)
            {
                dia[currentDialogueIndex].function?.Invoke();
            }
            currentDialogueIndex++;
        }
        else
        {
            Camera.main.gameObject.GetComponent<CameraFollowPlayer>().UnlockFromNPC();
            Debug.Log("Dialogue set completed for " + gameObject.name);
            Time.timeScale = 1f;
            dialogueUI.SetActive(false);
            currentDialogueIndex = 0;
            player.GetComponent<KeyboardControls>().Interacting = false;
            player.GetComponent<KeyboardControls>().inBossDialogue = false;
            if(dia == BossIntro)
            {
                if (boss.GetComponent<DirectatBossAI>() != null)
                {
                    boss.GetComponent<DirectatBossAI>().StartAI();
                }
                else if (boss.GetComponent<CentrexusBossAI>() != null)
                {
                    boss.GetComponent<CentrexusBossAI>().StartAI();
                }
            }

        }
    }
}
