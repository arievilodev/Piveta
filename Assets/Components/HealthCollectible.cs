using UnityEngine;
using UnityEngine.UI;

public class HealthCollectible : MonoBehaviour
{
    private GameObject playerInside;
    [SerializeField] private bool playerIsInside;
    [SerializeField] private int valueToHeal = 30;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = collision.gameObject;
            playerIsInside = true;
            playerInside.GetComponent<Player>().Heal(valueToHeal);
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = null;
            playerIsInside = false;
        }
    }

}
