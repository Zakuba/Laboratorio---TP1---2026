# Casos de prueba

Formato: cada fila es un caso de prueba. Completar "Resultado" y "Evidencia"
a medida que se ejecutan. Actualizar esta tabla en cada sprint.

## Personaje y movimiento

| ID | Caso | Pasos | Resultado esperado | Resultado | Evidencia | Ticket relacionado |
|---|---|---|---|---|---|---|
| TC-01 | Movimiento básico en las 4 direcciones | Mover el personaje con WASD sobre el piso de prueba | El personaje se desplaza de forma fluida en las 4 direcciones | | | EDL-4 |
| TC-02 | Salto simple | Presionar el botón de salto en superficie plana | El personaje despega y vuelve a tocar el piso de forma consistente | | | EDL-7 |
| TC-03 | Salto contra techo bajo | Saltar debajo de una superficie baja | El personaje no atraviesa el techo, se detiene la trayectoria vertical | | | EDL-7, EDL-10 |
| TC-04 | Caminar sobre pendiente transitable | Caminar sobre una rampa dentro del rango permitido | El personaje sube sin perder velocidad significativa ni "trabarse" | | | EDL-11 |
| TC-05 | Caminar sobre pendiente no transitable | Intentar subir una pendiente demasiado inclinada | El personaje resbala o no puede subir, sin comportamiento errático | | | EDL-11 |
| TC-06 | Colisión contra pared | Caminar directo contra una pared | El personaje se detiene, no la atraviesa ni queda trabado | | | EDL-10 |
| TC-07 | Caída fuera del escenario | Caminar/saltar fuera de los límites del mapa | Se dispara el mecanismo de reaparición en el punto de control correspondiente | | | EDL-16, EDL-17 |
| TC-08 | Superficie con comportamiento diferente | Caminar sobre la superficie especial (ej. hielo) | El personaje presenta el comportamiento distinto documentado (ej. menor fricción) | | | EDL-13 |
| TC-09 | Elemento físico interactivo | Disparar/lanzar la bola de nieve contra un objeto | El objeto reacciona de forma físicamente consistente (empuje, rebote, etc.) | | | EDL-14, EDL-36 |

## Multiplayer (Sprint 3+)

| ID | Caso | Pasos | Resultado esperado | Resultado | Evidencia | Ticket relacionado |
|---|---|---|---|---|---|---|
| TC-10 | Crear e ingresar a una sala | Un jugador crea sala, otro se conecta con el mismo código/servidor | Ambos quedan en la misma instancia del escenario | | | |
| TC-11 | Sincronización de posición | Jugador A se mueve, observar en cliente B | El movimiento de A se refleja en B sin desfasajes notorios | | | |
| TC-12 | Sincronización de salto y caída | Jugador A salta/cae, observar en cliente B | La animación/posición vertical de A se refleja correctamente en B | | | |
| TC-13 | Reconexión / desconexión | Un jugador se desconecta a mitad de partida | El otro jugador recibe una señal clara de que el jugador se fue, sin crashear | | | |

## Condiciones de juego (Sprint 4)

| ID | Caso | Pasos | Resultado esperado | Resultado | Evidencia | Ticket relacionado |
|---|---|---|---|---|---|---|
| TC-14 | Condición de victoria | Cumplir el objetivo definido del juego | Se muestra claramente el estado de victoria | | | |
| TC-15 | Condición de derrota | Provocar la condición de derrota definida | Se muestra claramente el estado de derrota | | | |
| TC-16 | Fin de partida sincronizado | En multiplayer, un jugador gana/pierde | Ambos clientes reciben y muestran el mismo resultado | | | |
