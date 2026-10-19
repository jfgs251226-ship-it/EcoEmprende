using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

// Menú 2D: la ilustración, los botones y los créditos comparten la misma escala.
// De este modo la interfaz se mantiene centrada y no se recorta en distintas resoluciones.
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    private RectTransform contenido;
    private GameObject interfazCreditos;
    private int ultimoAncho = -1;
    private int ultimoAlto = -1;

    private const float Ancho = 960f;
    private const float Alto = 540f;

    private void Awake()
    {
        Sprite arteMenu = null;
        GameObject imagenEscena = GameObject.Find("ImagenFondoMenu");
        if (imagenEscena != null)
        {
            SpriteRenderer renderizador = imagenEscena.GetComponent<SpriteRenderer>();
            if (renderizador != null)
            {
                arteMenu = renderizador.sprite;
                renderizador.enabled = false; // Se usa el fondo de la interfaz para evitar duplicarlo.
            }
        }
        Sprite arteCreditos = null;
        if (creditsPanel != null)
        {
            SpriteRenderer renderizadorCreditos = creditsPanel.GetComponent<SpriteRenderer>();
            if (renderizadorCreditos != null) arteCreditos = renderizadorCreditos.sprite;
            creditsPanel.SetActive(false);
        }

        GameObject lienzo = new GameObject("InterfazPrincipal", typeof(RectTransform),
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = lienzo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        lienzo.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        GameObject grupo = new GameObject("ContenidoCentrado", typeof(RectTransform));
        grupo.transform.SetParent(lienzo.transform, false);
        contenido = grupo.GetComponent<RectTransform>();
        contenido.anchorMin = contenido.anchorMax = new Vector2(0.5f, 0.5f);
        contenido.pivot = new Vector2(0.5f, 0.5f);
        contenido.anchoredPosition = Vector2.zero;
        contenido.sizeDelta = new Vector2(Ancho, Alto);

        CrearImagen(grupo.transform, "FondoMenu", arteMenu);

        // Las posiciones corresponden exactamente a los botones dibujados en MenuInterfaz.png.
        CrearBoton(grupo.transform, "BotonJugar", 12f, 382f, 54f, PlayGame);
        CrearBoton(grupo.transform, "BotonCreditos", -58f, 382f, 54f, OpenCredits);
        CrearBoton(grupo.transform, "BotonSalir", -128f, 382f, 54f, QuitGame);

        RectTransform visualCreditos = CrearImagen(grupo.transform, "VistaCreditos", arteCreditos);
        interfazCreditos = visualCreditos.gameObject;
        CrearBoton(visualCreditos, "BotonVolver", -199f, 263f, 53f, CloseCredits);
        interfazCreditos.SetActive(false);

        if (FindFirstObjectByType<EventSystem>() == null)
            new GameObject("SistemaEventos", typeof(EventSystem), typeof(StandaloneInputModule));
        AjustarTamano();
    }

    private RectTransform CrearImagen(Transform padre, string nombre, Sprite arte)
    {
        GameObject objeto = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        objeto.transform.SetParent(padre, false);
        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(Ancho, Alto);
        Image imagen = objeto.GetComponent<Image>();
        imagen.sprite = arte;
        imagen.raycastTarget = false;
        return rect;
    }

    private void CrearBoton(Transform padre, string nombre, float y, float ancho,
        float alto, UnityAction accion)
    {
        GameObject objeto = new GameObject(nombre, typeof(RectTransform), typeof(Image), typeof(Button));
        objeto.transform.SetParent(padre, false);
        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(ancho, alto);
        Image imagen = objeto.GetComponent<Image>();
        imagen.color = new Color(1f, 1f, 1f, 0f); // Invisible y sin cuadros adicionales.
        imagen.raycastTarget = true;
        Button boton = objeto.GetComponent<Button>();
        boton.transition = Selectable.Transition.None;
        boton.onClick.AddListener(accion);
    }

    private void LateUpdate()
    {
        if (ultimoAncho != Screen.width || ultimoAlto != Screen.height) AjustarTamano();
    }

    private void AjustarTamano()
    {
        ultimoAncho = Screen.width;
        ultimoAlto = Screen.height;
        if (contenido == null) return;
        float factor = Mathf.Min(Screen.width / Ancho, Screen.height / Alto);
        contenido.localScale = Vector3.one * Mathf.Max(0.01f, factor);
    }

    public void PlayGame() { SceneManager.LoadScene("GameScene"); }
    public void OpenCredits() { if (interfazCreditos != null) interfazCreditos.SetActive(true); }
    public void CloseCredits() { if (interfazCreditos != null) interfazCreditos.SetActive(false); }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
