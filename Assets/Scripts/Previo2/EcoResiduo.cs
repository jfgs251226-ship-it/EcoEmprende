using UnityEngine;

public enum EcoTipo { Blanca, Verde, Negra }

// Cada residuo se recoge una sola vez y únicamente cuando la mano está libre.
[RequireComponent(typeof(SpriteRenderer))]
public class EcoResiduo : MonoBehaviour
{
    [SerializeField] private string nombre = "Residuo";
    [SerializeField] private EcoTipo tipo = EcoTipo.Blanca;
    private bool recogido;
    public string Nombre { get { return nombre; } }
    public EcoTipo Tipo { get { return tipo; } }
    public void Configurar(string nombreResiduo, EcoTipo clasificacion)
    { nombre = nombreResiduo; tipo = clasificacion; }
    private void Awake()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(.72f, .84f);
    }
    private void OnTriggerEnter2D(Collider2D otro) { Intentar(otro); }
    private void OnTriggerStay2D(Collider2D otro) { Intentar(otro); }
    private void Intentar(Collider2D otro)
    {
        if (recogido || !otro.CompareTag("Player") || EcoJuego.Instancia == null) return;
        if (EcoJuego.Instancia.Recoger(this))
        {
            recogido = true;
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject);
        }
    }
}
