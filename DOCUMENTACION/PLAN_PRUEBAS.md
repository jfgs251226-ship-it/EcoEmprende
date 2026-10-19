# EcoEmprende — diez pruebas funcionales (P1–P10)

Registra **Aprobada / Falló** solo después de ejecutar cada caso en Unity.

| Prueba | Cómo comprobarla | Resultado esperado | Estado |
|---|---|---|---|
| P1 | Caminar hasta el límite izquierdo o derecho del parque | No atraviesa el collider del límite | Por probar |
| P2 | Pulsar E lejos de la guía, carteles y canecas | No se abre ningún panel ni se clasifica | Por probar |
| P3 | Recoger una botella y regresar al sitio | Desaparece y no vuelve a recogerse | Por probar |
| P4 | Conversar con la guía usando E repetidas veces | Frases secuenciales y cierre al final | Por probar |
| P5 | Intentar otra interacción con el diálogo abierto | No se superponen paneles | Por probar |
| P6 | Abrir ambos carteles | Información ambiental legible | Por probar |
| P7 | Reiniciar tras depositar varios residuos | Vuelven los cinco residuos, contadores 0/5 | Por probar |
| P8 | Exportar el build y ejecutarlo fuera de Unity | Funciona sin errores de consola | Por probar |
| P9 | Depositar un objeto en la caneca correcta | Contador e inventario se actualizan | Por probar |
| P10 | Clasificar los cinco correctamente | Aparece ¡Parque limpio! | Por probar |

**Prueba adicional de la mecánica principal:** depositar un residuo en la caneca equivocada debe dar aviso y no aumentar el contador.
