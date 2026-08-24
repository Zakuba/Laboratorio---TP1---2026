# Documentación — "¿Me quedo o me voy?"

Esta carpeta centraliza la documentación técnica y de proceso del proyecto,
en línea con lo pedido en el enunciado del TP (sección 7 — Proceso de trabajo):
registro de decisiones técnicas, evidencias de pruebas, seguimiento de riesgos
y métricas.

## Estructura

```
docs/
├── README.md                  # este archivo
├── adr/                        # Registro de decisiones técnicas (ADRs)
│   ├── template.md
│   └── 0001-motor-y-controlador.md
├── testing/                    # Plan de pruebas y registro de ejecución
│   ├── plan-de-pruebas.md
│   └── casos-de-prueba.md
├── risks/                       # Gestión de riesgos
│   └── registro-de-riesgos.md
└── metrics/                     # Métricas de seguimiento del proyecto
    └── metricas.md
```

## Cómo se usa esto durante el proyecto

- Cada vez que se toma una decisión técnica relevante (motor, controlador de
  personaje, sistema de física, solución de networking, etc.) se crea un
  nuevo ADR a partir de `adr/template.md`, numerado correlativamente.
- El plan de pruebas se actualiza en cada sprint a medida que se agregan
  funcionalidades, y los resultados de ejecución quedan registrados en
  `testing/casos-de-prueba.md`.
- Los riesgos se cargan apenas se identifican (no al final del sprint) y se
  revisan en cada retro.
- Las métricas se cargan sprint a sprint en `metrics/metricas.md`.

## Convención de commits para documentación

Se sugiere prefijo `docs:` para diferenciar estos commits de los de código,
por ejemplo:

```
docs: agregar ADR-0002 sistema de networking
docs: actualizar plan de pruebas sprint 2
docs: registrar riesgo sincronización de salto
```
