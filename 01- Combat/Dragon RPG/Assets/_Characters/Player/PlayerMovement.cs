using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
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

        [SerializeField] const int walkableLayer = 8;
        [SerializeField] const int enemyLayer = 9;
        [SerializeField] const int unkownLayer = 2;

        AICharacterControl aic = null;
        GameObject walkTarget = null;

        private void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            aic = GetComponent<AICharacterControl>();

            walkTarget = new GameObject("walkTarget");

            cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
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

        void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
        {
            switch (layerHit)
            {
                case enemyLayer:
                    GameObject enemy = raycastHit.collider.gameObject;
                    aic.SetTarget(enemy.transform);
                    break;
                default:
                    break;
            }
        }

        //private void ProcessDirectMovement()
        //{
        //    // read inputs
        //    float h = Input.GetAxis("Horizontal");
        //    float v = Input.GetAxis("Vertical");

        //    // calculate camera relative direction to move:
        //    Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        //    Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

        //    thirdPersonCharacter.Move(movement, false, false);
        //}

        private Vector3 ShortDestination(Vector3 destination, float shortening)
        {
            Vector3 reductionVector = (destination - transform.position).normalized * shortening;
            return destination - reductionVector;
        }
    }
}
