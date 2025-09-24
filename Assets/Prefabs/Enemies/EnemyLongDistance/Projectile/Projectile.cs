using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int damage;  
    
    
    void Start()
    {
        StartCoroutine(Destroy());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Vector2.up * speed * Time.deltaTime);       
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
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
