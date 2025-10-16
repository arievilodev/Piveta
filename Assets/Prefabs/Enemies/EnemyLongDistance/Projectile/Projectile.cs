using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int damage;
    private Vector2 moveDirection;


    void Start()
    {
        StartCoroutine(Destroy());
    }

    void Update()
    {
        // Move o projétil na direção definida
        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Piveta"))
        {
            var knockbackDirection = (other.gameObject.GetComponent<Player>().transform.position - transform.position).normalized;
            other.gameObject.GetComponent<Player>().TakeDamage(damage, knockbackDirection);
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;

        // Rotaciona o projétil para apontar na direção do movimento
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}