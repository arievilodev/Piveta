using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallArea : MonoBehaviour
{
    [Header("Lista de inimigos da área")]
    [SerializeField] private List<GameObject> enemiesToDefeat;

    [Header("Collider da parede")]
    [SerializeField] private Collider2D wallCollider;

    [Header("Renderer da parede (opcional)")]
    [SerializeField] private Renderer wallRenderer; // SpriteRenderer, MeshRenderer, etc.

    private void Awake()
    {
        // Se não foi atribuído no Inspector, pega do próprio GameObject
        if (wallCollider == null)
            wallCollider = GetComponent<Collider2D>();
        if (wallRenderer == null)
            wallRenderer = GetComponent<Renderer>();

        // Garante que a parede começa ativa e visível
        if (wallCollider != null)
            wallCollider.enabled = true;
        if (wallRenderer != null)
            wallRenderer.enabled = true;
    }
    public void ActivateWall()
    {
        if (wallCollider != null)
            wallCollider.enabled = true;
        if (wallRenderer != null)
            wallRenderer.enabled = true;
    }

    private void Update()
    {
        // Remove inimigos já destruídos ou desativados da lista
        enemiesToDefeat.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);

        // Se todos os inimigos foram derrotados, libera a parede
        if (enemiesToDefeat.Count == 0)
        {
            Destroy(gameObject);
            // Some visualmente

            // Se quiser que a parede suma completamente do Hierarchy:
            // gameObject.SetActive(false);
        }
    }
}