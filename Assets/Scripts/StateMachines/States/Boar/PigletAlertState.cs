using UnityEngine;

/// <summary>
/// Estado de Alerta del Jabato:
/// - Condición: Contacto visual con el jugador.
/// - Acción:
///   1. Hace un ruido de alerta para el alfa.
///   2. Reproduce el sonido audible del jabato.
///   3. Movimiento hacia el jugador manteniendo distancia.
/// - Cono de visión activado.
/// - Oído desactivado.
/// - Salida: Perder contacto visual durante un tiempo.
/// </summary>
public class PigletAlertState : State
{
    [Header("Objetivo")]
    [Tooltip("El jugador a vigilar.")]
    [SerializeField] private Transform target;

    [Header("Sonido de Alerta - IA")]
    [Tooltip("Prefab del sonido emitido para avisar al Jabalí Alfa.")]
    [SerializeField] private GeneratedSound alertSoundPrefab;

    [Header("Sonido de Alerta - Audio")]
    [SerializeField] private AudioClip alertAudioClip;

    [Range(0f, 1f)]
    [SerializeField] private float alertVolume = 1f;

    [Header("Distancia y Velocidad")]
    [Tooltip("Distancia de respeto para no chocar con el jugador.")]
    [SerializeField] private float keepDistance = 1.15f;

    [Tooltip("Velocidad de persecución.")]
    [SerializeField] private float alertSpeed = 2.4f;

    private EnemyPathAgent pathAgent;
    private MovementSystemController movementController;
    private KinematicMovement kinematicMovement;

    private HearingSense pigletHearingSense;

    private float previousSpeed;
    private bool isHoldingDistance = false;

    public override void OnEntry()
    {
        Debug.Log(
            "[PigletAlertState] ¡Jabato en ALERTA! Avisando al alfa."
        );

        movementController =
            GetComponentInParent<MovementSystemController>();

        pathAgent =
            GetComponentInParent<EnemyPathAgent>();

        kinematicMovement =
            GetComponentInParent<KinematicMovement>();

        pigletHearingSense =
            GetComponentInParent<HearingSense>();

        // 1. Desactivar oído durante la alerta.
        if (pigletHearingSense != null)
        {
            pigletHearingSense.enabled = false;
        }

        Vector3 alertPosition =
            transform.parent != null
                ? transform.parent.position
                : transform.position;

        // 2. Sonido lógico de IA.
        // Este es el que puede escuchar el Jabalí Alfa
        // si se encuentra dentro del radio correspondiente.
        if (alertSoundPrefab != null)
        {
            GeneratedSound soundInstance =
                Instantiate(
                    alertSoundPrefab,
                    alertPosition,
                    Quaternion.identity
                );

            // Evitar que el propio jabato
            // escuche su alerta.
            soundInstance.soundGenerator =
                pigletHearingSense;
        }

        // 3. Sonido real para el jugador.
        if (alertAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(
                alertAudioClip,
                alertPosition,
                alertVolume
            );
        }

        // 4. Ajustar velocidad de persecución.
        if (kinematicMovement != null)
        {
            previousSpeed = kinematicMovement.speed;
            kinematicMovement.speed = alertSpeed;
            kinematicMovement.targetTransform = null;
        }

        if (movementController != null)
        {
            movementController.currentTargetTransform = null;
        }

        // 5. Buscar al jugador.
        if (target == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        isHoldingDistance = false;

        // 6. Iniciar persecución mediante A*.
        if (target != null &&
            pathAgent != null &&
            pathAgent.isActiveAndEnabled)
        {
            pathAgent.SetDestination(target);
        }
    }

    public override void OnUpdate()
    {
        if (target == null)
            return;

        Transform rootTransform =
            transform.parent != null
                ? transform.parent
                : transform;

        Vector3 toPlayer =
            target.position -
            rootTransform.position;

        toPlayer.y = 0f;

        float dist = toPlayer.magnitude;

        // Mantener distancia de seguridad.
        if (dist <= keepDistance)
        {
            if (!isHoldingDistance)
            {
                isHoldingDistance = true;

                if (pathAgent != null)
                {
                    pathAgent.ClearDestination();
                }

                if (kinematicMovement != null)
                {
                    kinematicMovement.Stop();
                }
            }

            // Mirar hacia el jugador sin acercarse más.
            if (toPlayer.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot =
                    Quaternion.LookRotation(
                        toPlayer.normalized,
                        Vector3.up
                    );

                rootTransform.rotation =
                    Quaternion.RotateTowards(
                        rootTransform.rotation,
                        lookRot,
                        300f * Time.deltaTime
                    );
            }
        }
        else if (dist > keepDistance + 0.4f)
        {
            // El jugador se aleja:
            // reanudar persecución.
            if (isHoldingDistance)
            {
                isHoldingDistance = false;
            }

            if (pathAgent != null &&
                pathAgent.isActiveAndEnabled)
            {
                pathAgent.SetDestination(target);
            }
            else if (movementController != null)
            {
                movementController.SetNewTarget(
                    target.position
                );
            }
        }
    }

    public override void OnExit()
    {
        // Reactivar oído al salir del estado.
        if (pigletHearingSense != null)
        {
            pigletHearingSense.enabled = true;
            pigletHearingSense.ResetHearing();
        }

        // Restaurar velocidad anterior.
        if (kinematicMovement != null)
        {
            kinematicMovement.speed = previousSpeed;
            kinematicMovement.Stop();
        }

        // Detener Pathfinding.
        if (pathAgent != null)
        {
            pathAgent.ClearDestination();
        }

        isHoldingDistance = false;

        Debug.Log(
            "[PigletAlertState] Fin de Alerta. Oído reactivado."
        );
    }
}