# Registro de riesgos

Actualizar en cada sprint / retro. Probabilidad e impacto en escala Baja /
Media / Alta. Exposición = combinación de ambas (ej. Alta×Alta = Crítico).

| ID | Riesgo | Tipo | Probabilidad | Impacto | Exposición | Mitigación | Estado |
|---|---|---|---|---|---|---|---|
| R-01 | Documentación (Jira/Confluence) desactualizada respecto al código real subido al repositorio | Gestión | Media | Alto | Alta | Definir Definition of Done que incluya "código pusheado y mergeado" antes de marcar un ticket como Listo; revisar consistencia en cada daily/retro | Abierto |
| R-02 | Sincronización multiplayer inconsistente entre clientes (jitter, desfasaje de posición) | Técnico | Alta | Alto | Crítico | Investigar y prototipar la solución de networking temprano (no dejarlo para el final del Sprint 3); definir tick rate e interpolación | Abierto |
| R-03 | Comportamiento inconsistente del personaje con Rigidbody (resbalones, rebotes no deseados) | Técnico | Media | Medio | Media | Prueba de concepto comparativa Character Controller vs Rigidbody antes de comprometerse (ver ADR-0001) | Abierto |
| R-04 | Subestimación del tiempo necesario para el sistema de multiplayer | Gestión | Alta | Alto | Crítico | Reservar tiempo de investigación dedicado en Sprint 1, elegir una solución con buena documentación (ej. Netcode for GameObjects, Photon Fusion) | Abierto |
| R-05 | Desbalance de carga de trabajo entre integrantes del equipo | Gestión | Media | Medio | Media | Distribución explícita de responsabilidades por épica, seguimiento en el tablero | Abierto |
| R-06 | Falta de tiempo para testing y documentación por priorizar features | Gestión | Media | Alto | Alta | Reservar tiempo fijo de testing/documentación en cada sprint, no dejarlo para el final | Abierto |

## Cómo agregar un riesgo nuevo

1. Agregar una fila con ID correlativo (R-XX).
2. Clasificar tipo (Técnico / Gestión), probabilidad e impacto.
3. Definir una mitigación concreta, no genérica.
4. Revisar el estado (Abierto / Mitigado / Materializado / Cerrado) en cada
   retro y dejar registrado qué pasó si se materializó.
