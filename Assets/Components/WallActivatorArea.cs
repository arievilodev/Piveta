using UnityEngine;

public class WallActivatorArea : MonoBehaviour
{
    [Header("Parede a ser ativada")]
    [SerializeField] private InvisibleWallArea wallToActivate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o jogador entrou na área
        if (other.CompareTag("Player"))
        {
            if (wallToActivate != null)
                wallToActivate.ActivateWall();
        }
    }
}