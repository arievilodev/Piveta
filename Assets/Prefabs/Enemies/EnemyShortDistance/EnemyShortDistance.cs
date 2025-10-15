using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShortDistance : MonoBehaviour
{
    [SerializeField] private Player player;
    public float speedEnemy;
    [SerializeField] private Rigidbody2D rbEnemy;
    [SerializeField] private bool playerDetected = false;
    [SerializeField] private bool playerAttackable = false;
    [SerializeField] private int damage;
    private Vector2 initialPositionEnemy;
    public Animator anim;

    // Sistema de vida do inimigo
    [SerializeField] private int maxLife = 30;
    [SerializeField] private int currentLife;
    [SerializeField] private float detectRange, attackRange;
    [SerializeField] private bool isDead = false;
    [SerializeField] private Transform target;

    NavMeshAgent agent;

    // ✅ Sistema de cooldown de ataque
    [SerializeField] private float attackCooldown = 1.5f;
    private bool canAttack = true;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speedEnemy;

        rbEnemy = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentLife = maxLife;
    }

    void Update()
    {
        playerDetected = Physics2D.OverlapCircle(transform.position, detectRange, LayerMask.GetMask("Piveta"));
        playerAttackable = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Piveta"));

        // Teste de dano do inimigo
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamageEnemy(10);
        }

        if (playerDetected && !playerAttackable) FollowPlayer();
        if (playerDetected && playerAttackable) AttackPlayer();
    }

    private void FollowPlayer()
    {
        if (player.gameObject != null)
        {
            agent.SetDestination(player.transform.position);
            RotateTowardsPlayer();
        }
    }

    private void RotateTowardsPlayer()
    {
        if (player == null) return;

        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void AttackPlayer()
    {
        if (player != null && canAttack)
        {
            anim.SetTrigger("attack-enemyshort");
            var knockbackDirection = (player.transform.position - transform.position).normalized;
            player.TakeDamage(damage, knockbackDirection);

            if (player.isDead)
            {
                playerDetected = false;
                StartCoroutine(ReturnToStart());
            }

            // ✅ Inicia o cooldown após atacar
            StartCoroutine(AttackCooldown());
        }
    }

    // ✅ Corrotina de cooldown do ataque
    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamageEnemy(int amount)
    {
        if (isDead || isInvulnerable) return;

        currentLife -= amount;
        Debug.Log("Inimigo levou dano! Vida atual: " + currentLife);

        if (currentLife > 0)
        {
            StartCoroutine(InvulnerabilityFrames());
        }
        else
        {
            DieEnemy();
        }
    }

    private void DieEnemy()
    {
        isDead = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private IEnumerator ReturnToStart()
    {
        agent.SetDestination(initialPositionEnemy);
        yield return new WaitForFixedUpdate();
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