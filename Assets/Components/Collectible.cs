using UnityEngine;
using UnityEngine.UI;

public class Collectible : MonoBehaviour
{
    private GameObject playerInside;
    [SerializeField] private PowerSO powerSO;
    [SerializeField] private Image powerIcon;
    [SerializeField] private bool playerIsInside;
    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = powerSO.icon;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsInside)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                setCurrentPower(powerSO);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = collision.gameObject;
            playerIsInside = true;
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
    public void setCurrentPower(PowerSO powerSO)
    {
        playerInside.GetComponent<Player>().setCurrentPower(powerSO);
        powerIcon.sprite = powerSO.icon;
    }
}
