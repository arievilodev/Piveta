using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShortDistance : MonoBehaviour
{
    private Transform posPlayer; // Variável para armazenar a posição do jogador
    public float speedEnemy;
    [SerializeField] private Rigidbody2D rbEnemy;
    private bool playerDetected = false;
    private Vector2 initialPositionEnemy;
    public Animator anim;

    // Sistema de vida do inimigo
    [SerializeField] private int maxLife = 30;
    [SerializeField] private int currentLife;
    [SerializeField] private bool isDead = false;
    [SerializeField] private Transform target;
    NavMeshAgent agent;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Desativa a rotação automática do NavMeshAgent
        agent.updateUpAxis = false;

        agent.speed = speedEnemy; // Define a velocidade do inimigo

        posPlayer = GameObject.FindGameObjectWithTag("Player").transform; // Encontra o jogador na cena e armazena sua posição

        rbEnemy = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        initialPositionEnemy = rbEnemy.position;
        currentLife = maxLife; // Inicializa a vida do inimigo com o valor máximo
    }

    void Update()
    {
        if (playerDetected)
        {
            FollowPlayer();
        }

        // teste de dano do inimigo
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamageEnemy(10);
        }
    }

    private void FollowPlayer()
    {
        if (posPlayer.gameObject != null)
        {
            agent.SetDestination(posPlayer.position); // Move o inimigo em direção ao jogador
        }

    }

    public void TakeDamageEnemy(int amount)
    {
        anim.SetTrigger("hit");

        if (isDead || isInvulnerable) return;

        currentLife -= amount;
        Debug.Log("Inimigo levou dano! Vida atual: " + currentLife);

        if (currentLife > 0)
        {
            anim.SetTrigger("hit");
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
        // Exemplo: desativa o inimigo
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!playerDetected && other.CompareTag("Player"))
        {
            playerDetected = true;
        }
    }


    // Inicializando a colisão do inimigo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto colidido é o jogador a partir da tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obt�m o componente Player, guarda os resultados no objeto player
            Player player = collision.gameObject.GetComponent<Player>();
            /* Se o componente PlayerMov não for nulo, ou seja, se tiver sido encontrado, então o método TakeDamage é chamado,
             tirando 10 pontos de vida do jogador */
            if (player != null)
            {
                var knockbackDirection = (player.transform.position - transform.position).normalized;
                player.TakeDamage(10, knockbackDirection);
                if (player.isDead)
                {
                    playerDetected = false;
                    StartCoroutine(ReturnToStart());
                }

            }

        }

    }

    private IEnumerator ReturnToStart()
    {
        agent.SetDestination(initialPositionEnemy); // Move o inimigo de volta para a posição inicial
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
