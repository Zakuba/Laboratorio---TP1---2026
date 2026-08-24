# Plan de pruebas

## Objetivo

Verificar que las funcionalidades implementadas cumplen los criterios de
aceptación definidos en el backlog, y detectar problemas de movimiento,
física, colisiones y (a partir del Sprint 3) sincronización multiplayer.

## Alcance por sprint

| Sprint | Foco de testing |
|---|---|
| Sprint 1 | No aplica (investigación y prueba de concepto) |
| Sprint 2 | Movimiento, cámara, salto, gravedad, colisiones, caídas |
| Sprint 3 | Sincronización multiplayer, consistencia de estado entre clientes |
| Sprint 4 | Condiciones de victoria/derrota, flujo completo de partida |

## Tipos de prueba

- **Pruebas manuales exploratorias:** recorrer el escenario de prueba
  probando cada superficie, plataforma y obstáculo.
- **Pruebas de caso límite:** situaciones borde (caer fuera del mapa,
  saltar contra una pared, moverse sobre una pendiente muy inclinada).
- **Pruebas de regresión:** repetir casos de sprints anteriores tras
  agregar nueva funcionalidad, para asegurar que no se rompió nada.
- **Pruebas multiplayer (Sprint 3+):** con al menos 2 clientes conectados
  simultáneamente, verificando que las acciones de un jugador se reflejen
  correctamente en el otro.

## Entorno de pruebas

- [Completar: versión de Unity, plataforma de build usada para probar
  (Editor / standalone), cantidad de máquinas usadas para pruebas
  multiplayer, tipo de conexión.]

## Registro de resultados

Cada caso de prueba ejecutado se registra en `casos-de-prueba.md` con:
fecha, resultado (OK / Falla), evidencia (captura, video o log) y, si
falló, el ticket de Jira creado para el bug.
