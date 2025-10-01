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

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);       
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
       

    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
