using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerProjectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float speed;
    [SerializeField] int damage;
    private Vector2 moveDirection;


    void Start()
    {
        StartCoroutine(Destroy());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var knockbackDirection = (other.gameObject.transform.position - transform.position).normalized;            
            other.gameObject.GetComponent<EnemyHealth>().TakeDamageEnemy(damage);
            Destroy(gameObject);
        }
    }
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;


    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
