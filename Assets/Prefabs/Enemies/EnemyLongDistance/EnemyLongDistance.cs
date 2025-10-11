using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLongDistance : MonoBehaviour
{
    [SerializeField] private Player player;
    public float speedEnemy;
    [SerializeField] private Rigidbody2D rbEnemy;
    [SerializeField] private bool playerDetected = false;
    [SerializeField] private bool playerAttackable = false;
    private Vector2 initialPositionEnemy;
    public Animator anim;

    [SerializeField] private int maxLife = 30;
    [SerializeField] private int currentLife;
    [SerializeField] private float detectRange, attackRange;
    [SerializeField] private bool isDead = false;
    [SerializeField] private Transform target;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject projectile;
    [SerializeField] private bool onAttackCooldown = false;
    [SerializeField] private float cooldown = 2f; // Changed to float for better control
    NavMeshAgent agent;
    [SerializeField] private int currentWaypoint;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speedEnemy;

        rbEnemy = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        initialPositionEnemy = transform.position;
        currentLife = maxLife;
    }

    void Update()
    {
        if (isDead) return; // Don't do anything if dead

        playerDetected = Physics2D.OverlapCircle(transform.position, detectRange, LayerMask.GetMask("Piveta"));
        playerAttackable = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Piveta"));

        if (!playerDetected && !playerAttackable) return;
        if (playerDetected && !playerAttackable) FollowPlayer();
        if (playerDetected && playerAttackable) AttackPlayer();
    }

    private void FollowPlayer()
    {
        if (player != null && player.gameObject != null)
        {
            agent.SetDestination(player.transform.position);
            FacePlayer();
        }
    }

    private void FacePlayer()
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
        if (player == null) return; // Safety check

        agent.SetDestination(transform.position); // Stop moving
        FacePlayer(); // Face the player before shooting

        if (!onAttackCooldown)
        {
            ShootProjectile();
            onAttackCooldown = true;
            StartCoroutine(AttackCooldown());
        }
    }

    private void ShootProjectile()
    {
        if (projectile == null || projectileSpawnPoint == null || player == null) return;

        Vector2 direction = (player.transform.position - projectileSpawnPoint.position).normalized;

        GameObject proj = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);

        Projectile projScript = proj.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.SetDirection(direction);
        }
    }

    private void DieEnemy()
    {
        isDead = true;
        agent.enabled = false; // Disable NavMeshAgent when dead
    }

    private IEnumerator ReturnToStart()
    {
        agent.SetDestination(initialPositionEnemy);
        yield return new WaitForFixedUpdate();
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        onAttackCooldown = false;
    }

    // Visual debugging in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}