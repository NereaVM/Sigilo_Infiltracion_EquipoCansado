
using UnityEngine;

public class BoarReturnState : State
{
    [SerializeField] private Transform returnTarget;

    private MovementSystemController movementSystemController;
    private EnemyPathAgent pathAgent;
    private bool usingPathfinding;

    [Header("Return Speed")]
    [SerializeField] private float returnSpeed = 1.7f;

    private Rigidbody rb;
    private float originalMaxSpeed;
    private bool speedModified;

    public override void OnEntry()
    {
        Debug.Log("Boar entered Return state.");

        rb = GetComponentInParent<Rigidbody>();

        speedModified = false;

        if (rb != null)
        {
            originalMaxSpeed = rb.maxLinearVelocity;
            rb.maxLinearVelocity = returnSpeed;
            speedModified = true;
        }

        foreach (MovementScript movementScript in movementScripts)
        {
            if (movementScript != null)
            {
                Debug.Log(
                    "RETURN MOVEMENT: " +
                    movementScript.GetType().Name
                );
            }
        }

        movementSystemController =
            GetComponentInParent<MovementSystemController>();

        pathAgent =
            GetComponentInParent<EnemyPathAgent>();

        if (returnTarget == null)
        {
            Debug.LogWarning("Boar Return: No return target assigned.");
            return;
        }

        // Comprobamos si podemos utilizar A*.
        usingPathfinding =
            pathAgent != null &&
            pathAgent.isActiveAndEnabled &&
            pathAgent.gridManager != null &&
            pathAgent.pathfinding != null &&
            movementSystemController != null;

        if (usingPathfinding)
        {
            // Evitamos que el destino final sobrescriba
            // los nodos calculados por A*.
            movementSystemController.currentTargetTransform = null;

            foreach (MovementScript movementScript in movementScripts)
            {
                if (movementScript != null)
                {
                    movementScript.targetTransform = null;
                }
            }

            // Calculamos la ruta hacia el punto de retorno.
            pathAgent.SetDestination(returnTarget.position);


            // Hacemos que el jabalí mire hacia
            // el siguiente nodo de A* durante el retorno.
            foreach (MovementScript movementScript in movementScripts)
            {
                if (movementScript is Steering_Face face)
                {
                    face.targetTransform = pathAgent.PathLookTarget;
                }
            }


            Debug.Log("Boar returning with A* Pathfinding.");
        }
        else
        {
            // Conservamos el movimiento original.
            if (movementSystemController != null)
            {
                movementSystemController.SetNewTarget(returnTarget);
            }

            foreach (MovementScript movementScript in movementScripts)
            {
                if (movementScript != null)
                {
                    movementScript.targetTransform = returnTarget;
                }
            }

            Debug.Log("Boar returning with original movement.");
        }
    }

    public override void OnUpdate()
    {
        // EnemyPathAgent actualiza los nodos.
        // MovementSystemController mueve al jabalí.
    }

    public override void OnExit()
    {

        if (speedModified && rb != null)
        {
            rb.maxLinearVelocity = originalMaxSpeed;
            speedModified = false;
        }

        // Limpiamos la ruta al abandonar Return.
        if (usingPathfinding && pathAgent != null)
        {
            pathAgent.ClearDestination();
        }


        foreach (MovementScript movementScript in movementScripts)
        {
            if (movementScript is Steering_Face face)
            {
                face.targetTransform = null;
            }
        }


        usingPathfinding = false;

        Debug.Log("Boar exited Return state.");
    }
}
