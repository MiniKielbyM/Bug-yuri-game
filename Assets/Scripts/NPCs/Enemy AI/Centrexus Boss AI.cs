using System;
using System.Collections;
using UnityEngine;

public class CentrexusBossAI : MonoBehaviour
{
    public int health = 100;
    public GameObject[] Geysers;
    public GameObject Coin;
    public Animator animator;
    public float attackCooldown = 1f;
    public GameObject hpBar;
    private RectTransform fillRect;
    private float displayedHealthPercent;
    private float velocity;
    public EnterBossArea dialogueManager;
    public float smoothTime = 0.2f;
    public bool bossStarted = false;
    private bool da = false;
    private int currentAttack = 0;
    private string[] attackTriggers = { "Geyser", "Coin" };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        hpBar.SetActive(false);
        fillRect = hpBar.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        displayedHealthPercent = health / 100f;
    }
    public void StartAI()
    {
        hpBar.SetActive(true);
        bossStarted = true;
        StartCoroutine(BattlePhase());
    }
    private IEnumerator BattlePhase()
    {
        while (health > 0)
        {
            animator.SetTrigger(attackTriggers[currentAttack]);
            yield return new WaitForSeconds(attackCooldown);
            currentAttack = (currentAttack + 1) % 2;
        }
        if (health == 0)
        {
            animator.enabled = false;
        }
    }

    private void Update()
    {
        float targetPercent = Mathf.Clamp01(health / 100f);

        displayedHealthPercent = Mathf.SmoothDamp(
            displayedHealthPercent,
            targetPercent,
            ref velocity,
            smoothTime
        );

        if (Math.Floor(displayedHealthPercent * 1000) == 0 && !dialogueManager.bossDefeated)
        {
            fillRect.anchorMax = new Vector2(0f, 1f);
            dialogueManager.BossDefeat();
            hpBar.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            fillRect.anchorMax = new Vector2(displayedHealthPercent, 1f);
        }
    }
    public void leave()
    {
        da = true;
    }
    public void SummonGeyser()
    {
        Geysers[new System.Random().Next(0, Geysers.Length)].GetComponent<CentrexusGeyserAttack>().fire = true;
    }
    public void SummonCoin()
    {
        Instantiate(Coin, GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 5, 0), transform.rotation);
    }
    public void ResetTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);
    }
    public void Hit(int Damage)
    {
        health -= Damage;
    }
}
