using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Runtime.CompilerServices;

public class Player : MonoBehaviour
{
    // Movimentação e idle do jogador  
    [SerializeField] float speed = 1;
    public Rigidbody2D rb;
    public Collider2D playerCollider;
    public Vector2 mov;
    public Animator anim;

    private Vector2 lastMoveDir = Vector2.down; // Direção padrão inicial

    // Vida do jogador
    [SerializeField] Image lifeBar;
    [SerializeField] Image redLifeBar;
    [SerializeField] int maxLife;
    [SerializeField] int currentLife;
    public bool isDead;

    // Knockback do jogador ao receber dano
    public KnockbackComponent knockbackComponent;

    //Ataque do jogador
    public bool IsPlayingPunchRightAnimation;
    public bool IsPlayingPunchLeftAnimation;
    public bool IsPlayingPunchKickAnimation;

    [SerializeField] private int punchRightDamage = 5;
    [SerializeField] private int punchLeftDamage = 7;
    [SerializeField] private int kickDamage = 12;
    [SerializeField] private int stealthAttackDamage = 9999;
    private int attackIndex = 0; // 0: soco direito, 1: soco esquerdo, 2: chute

    [SerializeField] private float attackRange = 5f;
    [SerializeField] private LayerMask enemyLayer;
    private bool attackQueued = false;
    [SerializeField] PowerSO basePower;
    [SerializeField] PowerSO assignedPower; //Poder associado ao player por meio do coletável
    [SerializeField] PowerSO activePower; //Poder ativo no momento sendo aplicado ao player
    [SerializeField] private bool powerIsActive = false;
    [SerializeField] private bool powerIsOnCooldown = false;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private PlayerProjectile playerProjectile;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material strengthMaterial;
    [SerializeField] private Material rangedMaterial;
    [SerializeField] private Material invisMaterial;


    void Start()
    {
        knockbackComponent = GetComponent<KnockbackComponent>();
        currentLife = maxLife;

    }

    void Update()
    {
        MoveLogic();
        if (Input.GetKeyDown(KeyCode.P) &&
        !IsPlayingPunchRightAnimation &&
        !IsPlayingPunchLeftAnimation &&
        !IsPlayingPunchKickAnimation)
        {
            attackQueued = true;
            Debug.Log("Ataque acionado");
        }
        chooseAttackByPower();
    

        /*// Teste da barra de vida para perder vida
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(10, new Vector2(0, 0));
        };

        // Teste da barra de vida para ganhar vida
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        };*/

    }

    private void FixedUpdate()
    {
        if (knockbackComponent.isKnockbackActive)
        {
            knockbackComponent.ApplyKnockback();
        }
        else
        {
            rb.linearVelocity = mov.normalized * speed;
        }

    }

    public void MoveLogic()
    {
        mov.x = Input.GetAxis("Horizontal");
        mov.y = Input.GetAxis("Vertical");

        anim.SetFloat("Horizontal", mov.x);
        anim.SetFloat("Vertical", mov.y);
        anim.SetFloat("Speed", mov.sqrMagnitude);

        // Salva a última direção válida de movimento
        // Só atualiza direção se estiver se movendo
        if (mov.sqrMagnitude > 0.01f)
        {
            lastMoveDir = mov.normalized;
            anim.SetFloat("Horizontal", lastMoveDir.x);
            anim.SetFloat("Vertical", lastMoveDir.y);
        }
        // Se não estiver se movendo, mantém a última direção
        else
        {
            anim.SetFloat("Horizontal", lastMoveDir.x);
            anim.SetFloat("Vertical", lastMoveDir.y);
        }
        if (projectileSpawn != null)
        {
            float spawnDistance = 0.5f;

           
            Vector2 dir = lastMoveDir.sqrMagnitude > 0.01f ? lastMoveDir : Vector2.down;

            // Place launcher in front of the player
            projectileSpawn.localPosition = dir.normalized * spawnDistance;

            // Rotate it to face that direction
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectileSpawn.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }


    public void TakeDamage(int amount, Vector2 knockbackDirection)
    {
        SetLife(-amount);
        anim.SetTrigger("TakeDamage");
        knockbackComponent.Knockbacked();
        knockbackComponent.knockbackDirection = knockbackDirection;
    }

    public void Heal(int amount)
    {
        SetLife(amount);
    }

    public void SetLife(int amount)
    {

        currentLife = Mathf.Clamp(currentLife + amount, 0, maxLife);

        Vector3 lifeBarScale = lifeBar.transform.localScale;
        lifeBarScale.x = (float)currentLife / maxLife;
        lifeBar.rectTransform.localScale = lifeBarScale;
        StartCoroutine(DecreasingRedLifeBar(lifeBarScale));

        DeadState();

    }

    public void chooseAttackByPower()
    {

        if (attackQueued)
        {
            if (activePower.id == 0)
            {
                AttackBase();
            }
            if (activePower.id == 1)
            {
                AttackBase();
            }
            if (activePower.id == 2)
            {
                AttackRanged();
            }
            if (activePower.id == 3)
            {
                AttackInvis();
            }
        }
    }
    private void AttackBase()
    {
        
        Vector3 attackDir = new Vector3(mov.x, mov.y, 0).normalized;
        if (attackDir == Vector3.zero)
            attackDir = Vector3.right;

        switch (attackIndex)
        {
            case 0:
                StartCoroutine(PlayPunchRightAnimation(attackDir, punchRightDamage));
                break;
            case 1:
                StartCoroutine(PlayPunchLeftAnimation(attackDir, punchLeftDamage));
                break;
                case 2:
                StartCoroutine(PlayKickAnimation(attackDir, kickDamage));
                break;
        }
        attackIndex = (attackIndex + 1) % 3;
        attackQueued = false;
    
    }
    private void AttackRanged()
    {
        Vector3 attackDir = new Vector3(mov.x, mov.y, 0).normalized;
        if (attackDir == Vector3.zero)
            attackDir = Vector3.right;

        StartCoroutine(PlayPunchRightAnimation(attackDir, 0));
        Vector2 direction = lastMoveDir.sqrMagnitude > 0.01f ? CorrectDirection(lastMoveDir) : Vector2.down;

        // Spawn projectile
        GameObject proj = Instantiate(playerProjectile.gameObject, projectileSpawn.position, Quaternion.identity);
        attackQueued = false;
        proj.GetComponent<PlayerProjectile>().SetDirection(direction);     
        

    }
    private void AttackInvis()
    {
        Vector3 attackDir = new Vector3(mov.x, mov.y, 0).normalized;
        if (attackDir == Vector3.zero)
            attackDir = Vector3.right;
        StartCoroutine(PlayStealthAttackAnimation(attackDir, stealthAttackDamage));        
    }

    private IEnumerator PlayPunchRightAnimation(Vector3 dir, int damage)
    {
        IsPlayingPunchRightAnimation = true;
        anim.Play("attack-piveta-punchRight");
        yield return new WaitForSeconds(0.1f); // Momento do impacto
        ApplyDamageToEnemies(damage);
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength - 0.1f);
        IsPlayingPunchRightAnimation = false;
        VoltarParaIdleOuWalk();
    }

    private IEnumerator PlayPunchLeftAnimation(Vector3 dir, int damage)
    {
        IsPlayingPunchLeftAnimation = true;
        anim.Play("attack-piveta-punchLeft");
        yield return new WaitForSeconds(0.1f);
        ApplyDamageToEnemies(damage);
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength - 0.1f);
        IsPlayingPunchLeftAnimation = false;
        VoltarParaIdleOuWalk();
    }

    private IEnumerator PlayKickAnimation(Vector3 dir, int damage)
    {
        IsPlayingPunchKickAnimation = true;
        anim.Play("attack-piveta-kick");
        yield return new WaitForSeconds(0.1f);
        ApplyDamageToEnemies(damage);
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength - 0.1f);
        IsPlayingPunchKickAnimation = false;
        VoltarParaIdleOuWalk();
    }
    private IEnumerator PlayStealthAttackAnimation(Vector3 dir, int damage)
    {
        IsPlayingPunchRightAnimation = true;
        anim.Play("attack-piveta-punchRight");
        yield return new WaitForSeconds(0.1f); // Momento do impacto
        ApplyDamageToEnemies(damage);
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength - 0.1f);
        IsPlayingPunchRightAnimation = false;
        VoltarParaIdleOuWalk();
        deactivatePower();
    }

    private void VoltarParaIdleOuWalk()
    {
        if (Input.GetKey(KeyCode.P))
            return; // Não volta para idle/walk se P ainda estiver pressionado

        if (mov.sqrMagnitude > 0.01f)
            anim.Play("walk-piveta");
        else
            anim.SetFloat("Horizontal", lastMoveDir.x);
            anim.SetFloat("Vertical", lastMoveDir.y);
            anim.SetFloat("Speed", 0);
            anim.Play("idle-piveta");

    }

    private void ApplyDamageToEnemies(int damage)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>()?.TakeDamageEnemy(damage);            
        }
    }

    // Visualizar range de ataque do jogador no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void changeMaterial(Material mat)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.material = mat;
    }
    public void activatePower()
    {
        if (!powerIsOnCooldown && !powerIsActive)
        {
            activePower = assignedPower;
            if (assignedPower.id == 0) return;
            if (assignedPower.id == 1)
            {
                punchLeftDamage *= 2;
                punchRightDamage *= 2;
                kickDamage *= 2;
                changeMaterial(strengthMaterial);
            }
            if (assignedPower.id == 2)
            {
                changeMaterial(rangedMaterial);
            }
            if (assignedPower.id == 3)
            {
                changeMaterial(invisMaterial);
                gameObject.layer = LayerMask.NameToLayer("Invisible");
                powerIsActive = true; 
                return;
            }
            powerIsActive = true;
            StartCoroutine(powerActive());
        }
    }
    private void deactivatePower()
    {
        if (activePower.id == 1)
        {
            punchLeftDamage /= 2;
            punchRightDamage /= 2;
            kickDamage /= 2;
        }
        if (activePower.id == 3)
        {
            gameObject.layer = LayerMask.NameToLayer("Piveta");
        }
        changeMaterial(defaultMaterial);
        powerIsActive = false;
        powerIsOnCooldown = true;
        activePower = basePower;
        StartCoroutine(powerCooldown());
    }
    private IEnumerator powerActive()
    {
        yield return new WaitForSeconds(activePower.duration);
        //powerIsActive = false;
        deactivatePower();
    }
    private IEnumerator powerCooldown()
    {
        yield return new WaitForSeconds(assignedPower.cooldown);
        powerIsOnCooldown = false;
    }

    IEnumerator DecreasingRedLifeBar(Vector3 Scale)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 redLifeBarScale = redLifeBar.transform.localScale;


        while (redLifeBar.transform.localScale.x > Scale.x)
        {
            redLifeBarScale.x -= Time.deltaTime * 0.25f;
            redLifeBar.transform.localScale = redLifeBarScale;

            yield return null;
        }

        redLifeBar.transform.localScale = Scale;
    }

    private void DeadState()
    {

        if (currentLife <= 0)
        {

            isDead = true;
            anim.SetBool("IsDead", isDead);

            // Desativa a simulação do Rigidbody
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            // Desativa o Collider
            if (playerCollider != null)
            {
                playerCollider.enabled = false;
            }

        }
    }
    private Vector2 CorrectDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return dir.y > 0 ? Vector2.up : Vector2.down;
        }
    }
    public void SetCurrentPower(PowerSO power)
    {
        deactivatePower();
        powerIsOnCooldown = false;
        assignedPower = power;
    }
};