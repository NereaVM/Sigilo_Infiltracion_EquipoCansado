
using UnityEngine;

public class BoarPursuitState : State
{
    [SerializeField] private Transform target;

    private EnemyPathAgent pathAgent;
    private MovementSystemController movementController;
    private bool usingPathfinding;

    public override void OnEntry()
    {
        Debug.Log("Boar entered Pursuit state.");

        foreach (MovementScript movementScript in movementScripts)
        {
            if (movementScript != null)
            {
                Debug.Log(
                    "PURSUIT MOVEMENT: " +
                    movementScript.GetType().Name
                );
            }
        }


        // Buscamos los componentes en el objeto padre.
        pathAgent = GetComponentInParent<EnemyPathAgent>();

        movementController =
            GetComponentInParent<MovementSystemController>();

        // Comprobamos si podemos utilizar A*.
        usingPathfinding =
            pathAgent != null &&
            pathAgent.isActiveAndEnabled &&
            movementController != null &&
            target != null;

        if (usingPathfinding)
        {
            // Evitamos que el objetivo directo sobrescriba
            // los puntos calculados por A*.
            movementController.currentTargetTransform = null;

            foreach (MovementScript movementScript in movementScripts)
            {
                if (movementScript != null)
                {
                    movementScript.targetTransform = null;
                }
            }

            // Pedimos al agente que siga al jugador.
            pathAgent.SetDestination(target);


            // Hacemos que Steering_Face mire hacia
            // el siguiente nodo calculado por A*.
            foreach (MovementScript movementScript in movementScripts)
            {
                if (movementScript is Steering_Face face)
                {
                    face.targetTransform = pathAgent.PathLookTarget;
                }
            }


            Debug.Log("Boar pursuing with A* Pathfinding.");
        }
        else
        {
            // Conservamos el comportamiento original
            // cuando no se utiliza Pathfinding.
            foreach (MovementScript movementScript in movementScripts)
            {
                if (movementScript != null)
                {
                    movementScript.targetTransform = target;
                }
            }

            Debug.Log("Boar using original movement.");
        }
    }

    public override void OnUpdate()
    {
        // EnemyPathAgent actualiza el camino.
        // MovementSystemController mueve al enemigo.
    }

    public override void OnExit()
    {
        if (usingPathfinding && pathAgent != null)
        {
            pathAgent.ClearDestination();
        }

        usingPathfinding = false;

        Debug.Log("Boar exited Pursuit state.");
    }
}
