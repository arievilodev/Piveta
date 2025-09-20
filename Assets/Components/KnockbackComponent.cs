using UnityEngine;

public class KnockbackComponent : MonoBehaviour
{
    // Components
    Rigidbody2D rb;

    //States
    public bool isKnockbackActive = false;

    //Settings
    public Vector2 knockbackDirection;
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.1f;
    public float knockbackTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        // if (isKnockbackActive)
        // {
        //     ApplyKnockback();
        // }
    }

    public void Knockbacked()
    {
        isKnockbackActive = true;
    }

    public void ApplyKnockback()
    {
        if (isKnockbackActive && knockbackTimer < knockbackDuration)
        {
            rb.linearVelocity = knockbackDirection.normalized * knockbackForce;
            knockbackTimer += Time.deltaTime;
        }
        else
        {

            knockbackTimer = 0f;
            rb.linearVelocity = Vector2.zero; // Reset velocity after knockback
            isKnockbackActive = false;
        }
    }
}
