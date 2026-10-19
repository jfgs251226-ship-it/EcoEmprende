using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// HUD ambiental: inventario, clasificación, mensajes y final de misión.
public class EcoHUD : MonoBehaviour
{
    public bool Terminada { get; private set; }
    public bool EstaBloqueado { get { return panelVisible || Terminada; } }
    private bool panelVisible;
    private float avisoHasta;
    private Font fuente;
    private Text contador, objetivo, mano, pista, tituloPanel, cuerpoPanel;
    private readonly Image[] casillas = new Image[5];
    private GameObject panel, victoria, contenedorPista;
    private static readonly Color Esmeralda = new Color(.09f,.59f,.42f);
    private static readonly Color Crema = new Color(.12f,.38f,.31f);

    private void Awake()
    {
        fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        CrearInterfaz();
        Actualizar(0,0,"Ninguno");
    }
    private void CrearInterfaz()
    {
        GameObject raiz = new GameObject("Canvas_EcoEmprende",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        Canvas canvas = raiz.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler escala = raiz.GetComponent<CanvasScaler>();
        escala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escala.referenceResolution = new Vector2(1920,1080);
        escala.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        escala.matchWidthOrHeight = .5f;
        if (FindFirstObjectByType<EventSystem>() == null)
            new GameObject("SistemaEventos",typeof(EventSystem),typeof(StandaloneInputModule));

        RectTransform barra = Panel(raiz.transform,"BarraDeMision",Color.white);
        barra.GetComponent<Image>().sprite = Resources.Load<Sprite>("EcoEmprende/UIBar");
        barra.anchorMin = new Vector2(0,1);barra.anchorMax = new Vector2(1,1);
        barra.pivot = new Vector2(.5f,1);barra.offsetMin = new Vector2(0,-205);barra.offsetMax=Vector2.zero;
        // Distribución fija en unidades del lienzo de 1920 x 1080 para evitar superposiciones.
        objetivo=Texto(barra,"ObjetivoActual","",34,TextAnchor.MiddleLeft,new Color(.07f,.32f,.25f));
        DesdeArriba(objetivo.rectTransform,78,69,1150,53);
        contador=Texto(barra,"ContadorClasificados","",32,TextAnchor.MiddleLeft,new Color(.05f,.35f,.27f));
        DesdeArriba(contador.rectTransform,1390,70,445,46);
        mano=Texto(barra,"ResiduoEnMano","",26,TextAnchor.MiddleLeft,new Color(.16f,.42f,.33f));
        DesdeArriba(mano.rectTransform,78,143,1170,39);
        for (int i=0;i<5;i++)
        {
            RectTransform celda=Panel(barra,"Indicador_"+(i+1),new Color(.69f,.84f,.77f));
            DesdeArriba(celda,1390 + i*88,132,67,43);
            casillas[i]=celda.GetComponent<Image>();
            Text marca=Texto(celda,"Marca","•",30,TextAnchor.MiddleCenter,Color.white);
            Rect(marca.rectTransform,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(61,40));
        }
        RectTransform fondoPista = Panel(raiz.transform,"AyudaDeControles",new Color(.94f,.99f,.96f,.95f));
        Rect(fondoPista,new Vector2(.5f,0),new Vector2(.5f,0),new Vector2(965,64));
        fondoPista.anchoredPosition=new Vector2(0,46);
        contenedorPista=fondoPista.gameObject;
        pista=Texto(fondoPista,"Indicaciones","A/D: mover  ·  E: interactuar",28,TextAnchor.MiddleCenter,new Color(.07f,.40f,.30f));
        Rect(pista.rectTransform,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(915,54));
        panel = new GameObject("PanelDeInteraccion",typeof(RectTransform),typeof(Image));
        panel.transform.SetParent(raiz.transform,false);
        RectTransform r=panel.GetComponent<RectTransform>();
        Rect(r,new Vector2(.5f,.35f),new Vector2(.5f,.5f),new Vector2(1440,390));
        panel.GetComponent<Image>().sprite=Resources.Load<Sprite>("EcoEmprende/UICard");
        panel.GetComponent<Image>().color=Color.white;
        tituloPanel=Texto(r,"TituloInteraccion","",43,TextAnchor.MiddleLeft,new Color(.07f,.44f,.34f));
        Rect(tituloPanel.rectTransform,new Vector2(.49f,.80f),new Vector2(.5f,.5f),new Vector2(1280,67));
        cuerpoPanel=Texto(r,"ContenidoInteraccion","",33,TextAnchor.MiddleLeft,new Color(.15f,.30f,.27f));
        Rect(cuerpoPanel.rectTransform,new Vector2(.50f,.45f),new Vector2(.5f,.5f),new Vector2(1305,190));
        Text ayuda=Texto(r,"Ayuda","E: continuar / cerrar",24,TextAnchor.MiddleRight,new Color(.14f,.49f,.38f));
        Rect(ayuda.rectTransform,new Vector2(.86f,.10f),new Vector2(.5f,.5f),new Vector2(410,44));

        victoria=new GameObject("PanelMisionCumplida",typeof(RectTransform),typeof(Image));
        victoria.transform.SetParent(raiz.transform,false);
        RectTransform win=victoria.GetComponent<RectTransform>();
        Rect(win,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(1140,650));
        victoria.GetComponent<Image>().sprite=Resources.Load<Sprite>("EcoEmprende/UIVictory");
        victoria.GetComponent<Image>().color=Color.white;
        Text granTitulo=Texto(win,"TituloVictoria","¡PARQUE LIMPIO!",66,TextAnchor.MiddleCenter,Color.white);
        Rect(granTitulo.rectTransform,new Vector2(.5f,.86f),new Vector2(.5f,.5f),new Vector2(1050,100));
        Text cuerpo=Texto(win,"MensajeVictoria","¡Clasificaste correctamente los cinco residuos!\n\nCon pequeñas acciones cuidamos nuestra comunidad.\n¡Gracias por ser parte de EcoEmprende!",35,TextAnchor.MiddleCenter,new Color(.12f,.35f,.29f));
        Rect(cuerpo.rectTransform,new Vector2(.5f,.50f),new Vector2(.5f,.5f),new Vector2(1050,290));
        Boton(win,"BotonVolverAJugar","VOLVER A JUGAR",new Vector2(-257,-187),()=>SceneManager.LoadScene("GameScene"));
        Boton(win,"BotonRegresarMenu","MENÚ PRINCIPAL",new Vector2(257,-187),()=>SceneManager.LoadScene("MainMenu"));
        panel.SetActive(false);victoria.SetActive(false);
    }
    public void Actualizar(int recuperados,int clasificados,string enMano)
    {
        if (contador != null) contador.text = "CLASIFICADOS  " + clasificados + " / 5";
        if (objetivo != null) objetivo.text = "Recoge y clasifica 5 residuos";
        if (mano != null) mano.text = "EN MANO: " + enMano + "   ·   RECOGIDOS: " + recuperados + "/5";
        for(int i=0;i<5;i++) if(casillas[i]!=null)
            casillas[i].color=i<clasificados ? new Color(.12f,.64f,.43f) : new Color(.73f,.84f,.78f);
    }
    public void Pista(string texto)
    {
        if (pista != null && Time.time >= avisoHasta && !EstaBloqueado) pista.text=texto;
    }
    public void MensajeTemporal(string texto,float segundos)
    {
        if(pista==null)return;
        pista.text=texto;avisoHasta=Time.time+segundos;
    }
    public void AbrirPanel(string titulo,string mensaje)
    {
        if(panel==null)return;
        panelVisible=true; tituloPanel.text=titulo; cuerpoPanel.text=mensaje; panel.SetActive(true);
        if (contenedorPista != null) contenedorPista.SetActive(false);
    }
    public void CerrarPanel()
    {
        panelVisible=false;
        if(panel!=null)panel.SetActive(false);
        if(pista!=null)pista.text="A/D: mover  ·  E: interactuar";
        if(contenedorPista!=null)contenedorPista.SetActive(true);
    }
    public void Completar()
    {
        Terminada=true; panelVisible=false;
        if(panel!=null)panel.SetActive(false);
        if(victoria!=null)victoria.SetActive(true);
        if(contenedorPista!=null)contenedorPista.SetActive(false);
    }
    private static RectTransform Panel(Transform parent,string name,Color color)
    {
        GameObject go=new GameObject(name,typeof(RectTransform),typeof(Image)); go.transform.SetParent(parent,false);
        go.GetComponent<Image>().color=color;
        return go.GetComponent<RectTransform>();
    }
    private Text Texto(Transform parent,string name,string texto,int size,TextAnchor align,Color color)
    {
        GameObject go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);
        Text t=go.GetComponent<Text>(); t.font=fuente;t.fontSize=size;t.alignment=align;t.text=texto;t.color=color;
        t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;
        return t;
    }
    private static void DesdeArriba(RectTransform r,float izquierda,float arriba,float ancho,float alto)
    {
        r.anchorMin = new Vector2(0,1);
        r.anchorMax = new Vector2(0,1);
        r.pivot = new Vector2(0,1);
        r.anchoredPosition = new Vector2(izquierda,-arriba);
        r.sizeDelta = new Vector2(ancho,alto);
    }
    private static void Rect(RectTransform r,Vector2 anchor,Vector2 pivot,Vector2 size)
    {r.anchorMin=anchor;r.anchorMax=anchor;r.pivot=pivot;r.anchoredPosition=Vector2.zero;r.sizeDelta=size;}
    private void Boton(Transform padre,string nombre,string etiqueta,Vector2 posicion,UnityEngine.Events.UnityAction accion)
    {
        GameObject go = new GameObject(nombre,typeof(RectTransform),typeof(Image),typeof(Button));
        go.transform.SetParent(padre,false); RectTransform r=go.GetComponent<RectTransform>();
        Rect(r,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(425,93));r.anchoredPosition=posicion;
        go.GetComponent<Image>().color = Esmeralda;
        go.GetComponent<Button>().onClick.AddListener(accion);
        Text t=Texto(r,"Etiqueta",etiqueta,31,TextAnchor.MiddleCenter,Color.white);
        Rect(t.rectTransform,new Vector2(.5f,.5f),new Vector2(.5f,.5f),new Vector2(421,87));
    }
}
