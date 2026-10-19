using System.Collections.Generic;
using UnityEngine;

// Coordina la misión ambiental y evita acciones simultáneas sobre distintos objetos.
public class EcoJuego : MonoBehaviour
{
    public static EcoJuego Instancia { get; private set; }
    [Header("Guía ambiental - frases editables")]
    [SerializeField, TextArea(2, 5)] private string[] conversacion = {
        "¡Hola! Bienvenido a EcoEmprende. Este parque necesita nuestra ayuda.",
        "Recoge los residuos del sendero y observa de qué material están hechos.",
        "Blanca: residuos aprovechables limpios y secos. Verde: orgánicos. Negra: no aprovechables.",
        "Lleva cada residuo a su caneca y pulsa E. ¡Clasificar bien también es cuidar!"
    };

    private readonly List<EcoPunto> puntos = new List<EcoPunto>();
    private Transform jugador;
    private EcoHUD hud;
    private string residuoEnMano = "";
    private EcoTipo tipoEnMano;
    private int recogidos, clasificados;
    private int lineaDialogo = -1;
    private EcoPunto panelAbierto;
    private bool dialogoAbierto;
    private const int Meta = 5;

    public bool LlevaResiduo { get { return !string.IsNullOrEmpty(residuoEnMano); } }
    public int Recogidos { get { return recogidos; } }
    public int Clasificados { get { return clasificados; } }
    public string EnMano { get { return LlevaResiduo ? residuoEnMano : "Ninguno"; } }

    private void Awake()
    {
        Instancia = this;
        GameObject personaje = GameObject.Find("Player");
        if (personaje == null) { Debug.LogError("EcoEmprende: no se encontró Player en GameScene."); return; }
        jugador = personaje.transform;
        jugador.tag = "Player";
        hud = gameObject.GetComponent<EcoHUD>();
        if (hud == null) hud = gameObject.AddComponent<EcoHUD>();
        CrearParque();
        hud.Actualizar(recogidos, clasificados, EnMano);
    }

    private void OnDestroy() { if (Instancia == this) Instancia = null; }

    private void CrearParque()
    {
        CrearPunto("GuiaAmbiental", "EcoEmprende/GuiaAmbiental", -9f, EcoAccion.Dialogo, "GUÍA AMBIENTAL", "");
        CrearPunto("CartelSeparacion", "EcoEmprende/CartelSeparacion", -2.7f, EcoAccion.Lectura,
            "¿CÓMO SEPARAR LOS RESIDUOS?",
            "BLANCA: plásticos, papel y metales aprovechables, limpios y secos.\nVERDE: restos orgánicos, como cáscaras de frutas.\nNEGRA: residuos no aprovechables, como empaques contaminados.");
        CrearPunto("CartelCuidadoParque", "EcoEmprende/CartelParque", 25f, EcoAccion.Lectura,
            "UN PARQUE LIMPIO ES DE TODOS",
            "La basura abandonada afecta los suelos, el agua y la fauna. Recogerla y separarla facilita su aprovechamiento y reduce la contaminación del parque.");

        CrearPunto("CanecaBlanca", "EcoEmprende/CanecaBlanca", 12f, EcoAccion.CanecaBlanca, "CANECA BLANCA", "Aprovechables limpios y secos");
        CrearPunto("CanecaVerde", "EcoEmprende/CanecaVerde", 14.8f, EcoAccion.CanecaVerde, "CANECA VERDE", "Residuos orgánicos");
        CrearPunto("CanecaNegra", "EcoEmprende/CanecaNegra", 17.6f, EcoAccion.CanecaNegra, "CANECA NEGRA", "No aprovechables");

        CrearResiduo("BotellaPlastica", "EcoEmprende/Botella", -3.8f, "Botella plástica", EcoTipo.Blanca);
        CrearResiduo("Papel", "EcoEmprende/Papel", 2f, "Hoja de papel limpia", EcoTipo.Blanca);
        CrearResiduo("CascaraBanano", "EcoEmprende/Banano", 7f, "Cáscara de banano", EcoTipo.Verde);
        CrearResiduo("EmpaquePapas", "EcoEmprende/Empaque", 21f, "Empaque de papas contaminado", EcoTipo.Negra);
        CrearResiduo("Lata", "EcoEmprende/Lata", 31f, "Lata vacía limpia", EcoTipo.Blanca);

        Decorar("Banca_1", "EcoEmprende/Banca", -12f, -1.15f, 2f);
        Decorar("Banca_2", "EcoEmprende/Banca", 27.7f, -1.15f, 2f);
        Decorar("Arbusto_1", "EcoEmprende/Arbusto", -0.2f, -1.35f, 1.4f);
        Decorar("Arbusto_2", "EcoEmprende/Arbusto", 34.4f, -1.35f, 1.4f);
        CrearMuro("LimiteIzquierdo", -13.7f);
        CrearMuro("LimiteDerecho", 39f);
    }

    private GameObject CrearVisual(string nombre, string recurso, float x, float y, float escala, int orden)
    {
        Sprite sprite = Resources.Load<Sprite>(recurso);
        if (sprite == null) { Debug.LogError("EcoEmprende: falta imagen " + recurso); return null; }
        GameObject obj = GameObject.Find(nombre);
        if (obj != null) return obj;
        obj = new GameObject(nombre, typeof(SpriteRenderer));
        obj.transform.position = new Vector3(x, y, 0);
        obj.transform.localScale = new Vector3(escala, escala, 1);
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = sprite; sr.sortingOrder = orden;
        return obj;
    }
    private void CrearPunto(string nombre, string recurso, float x, EcoAccion accion, string titulo, string contenido)
    {
        GameObject obj = CrearVisual(nombre, recurso, x, -1.65f, 1.65f, 30);
        if (obj == null) return;
        EcoPunto punto = obj.GetComponent<EcoPunto>();
        if (punto == null)
        {
            punto = obj.AddComponent<EcoPunto>();
            punto.Configurar(accion, titulo, contenido);
        }
        if (!puntos.Contains(punto)) puntos.Add(punto);
    }
    private void CrearResiduo(string nombre, string recurso, float x, string etiqueta, EcoTipo tipo)
    {
        GameObject obj = CrearVisual("Residuo_" + nombre, recurso, x, -1.83f, 1.5f, 34);
        if (obj == null) return;
        EcoResiduo residuo = obj.GetComponent<EcoResiduo>();
        if (residuo == null)
        {
            residuo = obj.AddComponent<EcoResiduo>();
            residuo.Configurar(etiqueta, tipo);
        }
    }
    private void Decorar(string nombre, string recurso, float x, float y, float escala)
    { CrearVisual(nombre, recurso, x, y, escala, 9); }
    private void CrearMuro(string nombre, float x)
    {
        GameObject obj = new GameObject(nombre, typeof(BoxCollider2D), typeof(SpriteRenderer));
        obj.transform.position = new Vector3(x, -0.4f, 0);
        obj.transform.localScale = new Vector3(1.2f, 2.6f, 1);
        BoxCollider2D col = obj.GetComponent<BoxCollider2D>(); col.isTrigger = false;
        col.size = new Vector2(.8f, 1.8f);
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("EcoEmprende/Arbusto"); sr.sortingOrder = 28;
    }

    public bool Recoger(EcoResiduo residuo)
    {
        if (jugador == null || hud == null || hud.EstaBloqueado || LlevaResiduo || clasificados >= Meta) return false;
        residuoEnMano = residuo.Nombre;
        tipoEnMano = residuo.Tipo;
        recogidos++;
        hud.Actualizar(recogidos, clasificados, EnMano);
        hud.MensajeTemporal("Recogiste: " + residuoEnMano + ". Llévalo a una caneca.", 3.5f);
        return true;
    }

    private void Update()
    {
        if (jugador == null || hud == null || hud.Terminada) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (dialogoAbierto) { ContinuarDialogo(); return; }
            if (panelAbierto != null) { hud.CerrarPanel(); panelAbierto = null; return; }
            EcoPunto masCercano = PuntoMasCercano();
            if (masCercano != null) Interactuar(masCercano);
            return;
        }
        if (!dialogoAbierto && panelAbierto == null)
        {
            EcoPunto cerca = PuntoMasCercano();
            hud.Pista(cerca == null ? "A/D: mover  ·  E: interactuar" : "E: " + cerca.TextoAccion);
        }
    }

    private EcoPunto PuntoMasCercano()
    {
        EcoPunto elegido = null;
        float distancia = 1.18f;
        foreach (EcoPunto p in puntos)
        {
            if (p == null) continue;
            float d = Mathf.Abs(jugador.position.x - p.transform.position.x);
            if (d < distancia) { distancia = d; elegido = p; }
        }
        return elegido;
    }
    private void Interactuar(EcoPunto punto)
    {
        if (punto.Accion == EcoAccion.Dialogo)
        {
            dialogoAbierto = true; lineaDialogo = 0;
            hud.AbrirPanel("GUÍA AMBIENTAL", conversacion.Length > 0 ? conversacion[0] : "¡Cuidemos el parque!");
        }
        else if (punto.Accion == EcoAccion.Lectura)
        {
            panelAbierto = punto;
            hud.AbrirPanel(punto.Titulo, punto.Contenido);
        }
        else
        {
            EcoTipo tipoCaneca = punto.Accion == EcoAccion.CanecaBlanca ? EcoTipo.Blanca :
                punto.Accion == EcoAccion.CanecaVerde ? EcoTipo.Verde : EcoTipo.Negra;
            if (!LlevaResiduo) { hud.MensajeTemporal("Primero recoge un residuo del parque.", 2.8f); return; }
            if (tipoEnMano != tipoCaneca)
            {
                hud.MensajeTemporal("Caneca incorrecta. Observa el material y prueba otra vez.", 4f);
                return;
            }
            clasificados++;
            string entregado = residuoEnMano;
            residuoEnMano = "";
            hud.Actualizar(recogidos, clasificados, EnMano);
            if (clasificados == Meta) hud.Completar();
            else hud.MensajeTemporal("¡Correcto! " + entregado + " clasificado. ¡Continúa!", 3f);
        }
    }
    private void ContinuarDialogo()
    {
        lineaDialogo++;
        if (lineaDialogo < conversacion.Length) hud.AbrirPanel("GUÍA AMBIENTAL", conversacion[lineaDialogo]);
        else { dialogoAbierto = false; lineaDialogo = -1; hud.CerrarPanel(); }
    }
}
