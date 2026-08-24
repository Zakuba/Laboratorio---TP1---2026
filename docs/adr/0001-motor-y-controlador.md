# ADR-0001: Motor de desarrollo y enfoque del controlador de personaje

**Fecha:** 2026-08-24
**Estado:** Propuesta — completar con la investigación real del equipo
**Autores:** [completar]

> Este ADR se deja parcialmente completado como modelo. El equipo debe
> reemplazar los contenidos entre corchetes con su propia investigación,
> pruebas y justificación real (sección 5 del enunciado lo pide explícitamente).

## Contexto

El enunciado exige investigar y justificar el motor de desarrollo, el sistema
de física y el enfoque del controlador FPS (Character Controller, Rigidbody
u otra solución), considerando costo monetario, costo de aprendizaje y
costo de complejidad/riesgo — no solo el precio.

## Alternativas consideradas — Motor

### Opción A: Unity (6000.5.8f1, URP)
- Gratuito para uso educativo/individual.
- Documentación y comunidad extensas en español.
- Integración nativa con el nuevo Input System y con soluciones de
  networking (Netcode for GameObjects, Mirror, Photon Fusion).
- Curva de aprendizaje moderada; varios integrantes del equipo ya tienen
  experiencia previa.

### Opción B: Godot
- Gratuito y open source.
- Motor más liviano, pero el ecosistema de networking y assets es más
  chico que el de Unity.
- Curva de aprendizaje similar, pero sin experiencia previa del equipo.

### Opción C: Unreal Engine
- Gratuito hasta cierto umbral de ingresos (no relevante para el TP).
- Motor más potente gráficamente, pero mayor complejidad y curva de
  aprendizaje más alta para un prototipo de 4 semanas.

## Decisión — Motor

**Unity**, por experiencia previa del equipo, curva de aprendizaje más baja
para el tiempo disponible (4 sprints), y soporte directo para las mecánicas
pedidas (Character Controller / Rigidbody, Input System, networking).

## Alternativas consideradas — Controlador de personaje

### Opción A: Character Controller (CharacterController de Unity)
- Basado en cápsula con métodos de movimiento manuales (`Move`).
- No usa el sistema de física (Rigidbody), por lo que el movimiento es más
  predecible y directo de programar.
- Requiere implementar a mano gravedad, salto y detección de pendientes.
- Menor "realismo físico" pero mayor control fino del comportamiento —
  suele preferirse en FPS.

### Opción B: Rigidbody + Collider
- Usa el motor de física (PhysX) para mover al personaje aplicando fuerzas
  o velocidad.
- Interacciones físicas más naturales con objetos del mundo (empujar cosas,
  ser empujado).
- Más difícil de controlar con precisión (el personaje puede "resbalar",
  rebotar o comportarse de forma inconsistente si no se ajustan bien la
  fricción y la masa).

## Criterios de evaluación

| Criterio | Peso | Character Controller | Rigidbody |
|---|---|---|---|
| Costo de aprendizaje | Alto | Bajo — API simple (`Move`) | Medio — requiere entender fuerzas, drag, masa |
| Complejidad / riesgo | Alto | Bajo para movimiento FPS estándar | Medio-alto: riesgo de comportamiento inconsistente |
| Interacción física con objetos | Medio | Limitada, requiere trucos | Nativa |
| Precisión de control (salto, pendientes) | Alto | Alta | Media, requiere tuning |
| Costo monetario | — | Ninguno (nativo) | Ninguno (nativo) |

## Decisión — Controlador

[Completar una vez hecha la prueba de concepto: cuál se usó y por qué,
citando resultados concretos de la experimentación — por ejemplo,
comportamiento en pendientes, consistencia del salto, etc.]

## Consecuencias

- [Completar: qué se gana, qué se sacrifica, qué quedó pendiente de validar
  — por ejemplo, si se necesitará un Rigidbody adicional solo para el
  elemento físico/interactivo (la bola de nieve) aunque el jugador use
  Character Controller.]

## Referencias

- Documentación oficial de Unity sobre CharacterController y Rigidbody.
- EDL-4, EDL-5, EDL-6, EDL-7 (tickets de Jira relacionados con el
  controlador de personaje).
