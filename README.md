# EcoEmprende 🌱

**Serious Game educativo en Unity 6.3 LTS (2D)**.

El jugador recorre un parque, recoge cinco residuos y los lleva a la caneca correcta. Aprende a distinguir aprovechables, orgánicos y no aprovechables a través de un guía ambiental y dos carteles.

## Controles
- **A/D** o flechas: caminar.
- **E**: hablar con la guía, leer carteles o depositar en las canecas.
- Los residuos se recogen al tocarlos cuando no llevas otro.

## Separación de residuos (contexto colombiano)
- **Blanca:** botella plástica limpia, papel limpio, lata limpia.
- **Verde:** cáscara de banano.
- **Negra:** empaque de papas contaminado/no aprovechable.

El jugador solo puede cargar un residuo a la vez. La clasificación incorrecta no suma al HUD. La partida termina con cinco residuos clasificados correctamente.

## Estructura
- `Assets/Scenes/MainMenu.unity`: menú principal, créditos, salida.
- `Assets/Scenes/GameScene.unity`: nivel con personaje, terreno y cámara.
- `Assets/Scripts/Previo2/EcoJuego.cs`: misión, NPC, carteles, canecas y lógica de depósito.
- `Assets/Scripts/Previo2/EcoResiduo.cs`: recogida única por trigger.
- `Assets/Scripts/Previo2/EcoPunto.cs`: datos editables de puntos interactivos.
- `Assets/Scripts/Previo2/EcoHUD.cs`: interfaz e inventario.
- `Assets/Scripts/Player/PlayerController.cs`: desplazamiento lateral.
- `Assets/Scripts/Core/ConstruirEntorno.cs`: suelo con Tilemap y Collider2D.
- `Assets/Scripts/UI/MainMenuManager.cs`: botones de inicio y créditos.

## Ejecución
Abrir la carpeta del proyecto con Unity Hub y Unity 6.3.24f1; abrir `Assets/Scenes/MainMenu.unity`, pulsar Play. Si la importación solicita confirmar versión, verificar ProjectVersion.txt y la versión del editor.

## Pruebas
Se incluye `DOCUMENTACION/PLAN_PRUEBAS.md` con P1–P10. Las pruebas deben ejecutarse y registrarse sobre la versión realmente compilada.

## Requisitos de entrega del Previo 2
Informe PDF (mínimo diez capturas de pantalla), evidencias gráficas, video de 5–10 minutos con voz, build jugable y repositorio público de GitHub con README y .gitignore.

**Autores:** José Francisco García Santana y Juan Camilo Gallego Callejas.

## Diseño visual HD
Esta edición incorpora fondos vectoriales rasterizados a 1920 × 1080, sprites de personajes redibujados, recursos del parque a 256 × 256 e interfaz actualizada para pantallas panorámicas.

En Unity, seleccione la ventana **Game** y ajuste la relación a 16:9 para evaluar la composición y ponga el control Scale en **1x** al inspeccionar nitidez.

## Ajuste visual del HUD

La barra superior del juego reserva líneas independientes para el nombre de la misión, el progreso de clasificación y el residuo que lleva el personaje. Se revisaron las posiciones para evitar superposiciones en resolución 1920 x 1080.


## Ajuste de presentación del HUD (octubre de 2026)
- Barra superior reorganizada para 1920 x 1080 con etiquetas sin superposiciones.
- Contador y casillas de progreso independientes del título.
- Información del residuo dentro de la barra, sin cortar el texto.
- Instrucciones en una tarjeta inferior; se ocultan al abrir diálogos o al completar la misión.
- Botones de misión cumplida desplazados hacia el interior del panel.
- Se conservaron los scripts y la lógica de separación de residuos.
