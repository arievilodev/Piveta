using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxLife = 30;
    [SerializeField] private int currentLife;
    [SerializeField] private bool isDead = false;
    private KnockbackComponent knockback;
    public Animator anim;

    // Referência ao player para calcular a direção do knockback
    private Transform playerTransform;

    private void Awake()
    {
        currentLife = maxLife;
        knockback = GetComponent<KnockbackComponent>();

        // Encontra o player na cena
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public void TakeDamageEnemy(int amount)
    {
        if (isDead || isInvulnerable) return;

        currentLife -= amount;
        Debug.Log("Inimigo levou dano! Vida atual: " + currentLife);

        if (currentLife > 0)
        {
            StartCoroutine(InvulnerabilityFrames());

            // Calcula a direção do knockback baseado na posição do player
            if (playerTransform != null)
            {
                Vector2 knockbackDir = (transform.position - playerTransform.position).normalized;
                knockback.knockbackDirection = knockbackDir;
            }

            knockback.Knockbacked();
        }
        else
        {
            DieEnemy();
        }
    }

    private void DieEnemy()
    {
        anim.SetTrigger("attack-enemyshort");
        isDead = true;
        gameObject.SetActive(false);
    }

    private bool isInvulnerable = false;
    [SerializeField] private float invulnerableTime = 0.2f;

    private IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerableTime);
        isInvulnerable = false;
    }
}