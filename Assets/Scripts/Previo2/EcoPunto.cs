using UnityEngine;

public enum EcoAccion { Dialogo, Lectura, CanecaBlanca, CanecaVerde, CanecaNegra }

// Carteles y canecas se interactúan cerca del personaje pulsando la tecla E.
public class EcoPunto : MonoBehaviour
{
    [SerializeField] private EcoAccion accion = EcoAccion.Lectura;
    [SerializeField] private string titulo = "Punto educativo";
    [SerializeField, TextArea(3, 7)] private string contenido = "";
    public EcoAccion Accion { get { return accion; } }
    public string Titulo { get { return titulo; } }
    public string Contenido { get { return contenido; } }
    public string TextoAccion
    {
        get
        {
            if (accion == EcoAccion.Dialogo) return "hablar con la guía";
            if (accion == EcoAccion.Lectura) return "leer cartel";
            return "depositar en caneca " + (accion == EcoAccion.CanecaBlanca ? "blanca" : accion == EcoAccion.CanecaVerde ? "verde" : "negra");
        }
    }
    private void Awake()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.4f, 1.7f);
    }
    public void Configurar(EcoAccion tipo, string encabezado, string mensaje)
    { accion = tipo; titulo = encabezado; contenido = mensaje; }
}
