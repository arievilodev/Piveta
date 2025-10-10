using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShortDistance : MonoBehaviour
{
    [SerializeField] private Player player; // Variável para armazenar a posição do jogador
    public float speedEnemy;
    [SerializeField] private Rigidbody2D rbEnemy;
    [SerializeField] private bool playerDetected = false;
    [SerializeField] private bool playerAttackable = false;
    [SerializeField] private int damage;
    [SerializeField] private bool onAttackCooldown = false;
    [SerializeField] private int cooldown;
    private Vector2 initialPositionEnemy;
    public Animator anim;

    // Sistema de vida do inimigo

    [SerializeField] private float detectRange, attackRange; //range para detectar e atacar o player    
    [SerializeField] private Transform target;
    NavMeshAgent agent;
    [SerializeField] int currentWaypoint;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Desativa a rotação automática do NavMeshAgent
        agent.updateUpAxis = false;

        agent.speed = speedEnemy; // Define a velocidade do inimigo

        // Encontra o jogador na cena e armazena sua posição

        rbEnemy = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //initialPositionEnemy = rbEnemy.position;

    }

    void Update()
    {

        playerDetected = Physics2D.OverlapCircle(transform.position, detectRange, LayerMask.GetMask("Piveta"));     
        playerAttackable = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Piveta"));
        // teste de dano do inimigo
        if (!playerDetected && !playerAttackable) return;
        if (playerDetected && !playerAttackable) FollowPlayer();
        if (playerDetected && playerAttackable) AttackPlayer();
        
    }


    private void FollowPlayer()
    {
        if (player.gameObject != null)
        {
            agent.SetDestination(player.transform.position); // Move o inimigo em direção ao jogador
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
        if (player != null && !onAttackCooldown)
        {
            anim.SetTrigger("attack-enemyshort");
            var knockbackDirection = (player.transform.position - transform.position).normalized;
            player.TakeDamage(damage, knockbackDirection);
            onAttackCooldown = true;
            StartCoroutine(attackCooldown());
            if (player.isDead)
            {
                playerDetected = false;
                StartCoroutine(ReturnToStart());
            }
            
        }

    }
    





    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }


    // Inicializando a colisão do inimigo
    private void OnCollisionEnter2D(Collision2D collision)
    {
    
    }

    private IEnumerator ReturnToStart()
    {
        agent.SetDestination(initialPositionEnemy); // Move o inimigo de volta para a posição inicial
        yield return new WaitForFixedUpdate();
    }

    private IEnumerator attackCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        onAttackCooldown = false;
    }

}
