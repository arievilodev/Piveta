using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxLife = 30;
    [SerializeField] private int currentLife;
    [SerializeField] private bool isDead = false;
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        currentLife = maxLife; // Inicializa a vida do inimigo com o valor máximo
    }
    public void TakeDamageEnemy(int amount)
    {
        //anim.SetTrigger("hit"); // FALTA SPRITES DE ANIMAÇÃO DE DANO

        if (isDead || isInvulnerable) return;

        currentLife -= amount;
        Debug.Log("Inimigo levou dano! Vida atual: " + currentLife);

        if (currentLife > 0)
        {
            //anim.SetTrigger("hit");
            StartCoroutine(InvulnerabilityFrames());
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
        // Exemplo: desativa o inimigo
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
