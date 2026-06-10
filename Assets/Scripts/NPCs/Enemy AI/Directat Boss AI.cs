using System;
using System.Collections;
using UnityEngine;

public class DirectatBossAI : MonoBehaviour
{
    public int health = 100;
    public GameObject blow;
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
    private void Start()
    {
        hpBar.SetActive(false);
        fillRect = hpBar.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        displayedHealthPercent = health / 100f;
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
        }
        else
        {
            fillRect.anchorMax = new Vector2(displayedHealthPercent, 1f);
        }
        if (da)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0.25f, 0), Time.deltaTime * 2.5f);
        }
        if (Math.Floor(transform.localScale.y * 1000) == 0)
        {
            Destroy(gameObject);
        }
    }
    public void leave()
    {
        da = true;
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
            animator.SetTrigger("Slam");
            yield return new WaitForSeconds(attackCooldown);
        }
        if (health == 0)
        {
            animator.enabled = false;
        }
    }
    
    public void SummonWave()
    {
        Instantiate(blow, transform.position + Vector3.right * -1f + new Vector3(0, 1, 0), transform.rotation);
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
