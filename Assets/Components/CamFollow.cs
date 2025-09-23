using UnityEngine;

public class CamFollow : MonoBehaviour
{

    [SerializeField] GameObject target;
    [SerializeField] Vector2 distancia;
    [SerializeField] float velocidade;
    [SerializeField] Vector2 posicao;

    private void FixedUpdate()
    {
        posicao.x = Mathf.SmoothDamp(transform.position.x, target.transform.position.x, ref distancia.x, velocidade);
        posicao.y = Mathf.SmoothDamp(transform.position.y, target.transform.position.y, ref distancia.y, velocidade);

        transform.position = new Vector3(posicao.x, posicao.y, transform.position.z);
    }
}
