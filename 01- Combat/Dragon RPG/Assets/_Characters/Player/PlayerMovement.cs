using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; // TODO consider re-wiring

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICharacterControl))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class PlayerMovement : MonoBehaviour
    {
        CameraRaycaster cameraRaycaster;

        AICharacterControl aic = null;
        GameObject walkTarget = null;

        private void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            aic = GetComponent<AICharacterControl>();

            walkTarget = new GameObject("walkTarget");

            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
            {
                aic.SetTarget(enemy.transform);
            }
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            // Notify delegates of highest priority game object under mouse when clicked
            if (Input.GetMouseButton(0))
            {
                walkTarget.transform.position = destination;
                aic.SetTarget(walkTarget.transform);
            }
        }

        private Vector3 ShortDestination(Vector3 destination, float shortening)
        {
            Vector3 reductionVector = (destination - transform.position).normalized * shortening;
            return destination - reductionVector;
        }
    }
}
