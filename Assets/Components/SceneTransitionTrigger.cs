using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Nome da próxima cena")]
    [SerializeField] private string NomeDoLevel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o jogador entrou no trigger
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(NomeDoLevel);
        }
    }
}
