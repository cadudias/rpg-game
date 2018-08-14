using RPG.CameraUI;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float moveSpeedMultiplier = .7f;
        ThirdPersonCharacter character;

        GameObject walkTarget;
        NavMeshAgent agent;
        Animator animator;
        Rigidbody rigidBody;

        private void Start()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            character = GetComponent<ThirdPersonCharacter>();

            walkTarget = new GameObject("walkTarget");

            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updatePosition = true;

            agent.stoppingDistance = stoppingDistance;

            animator = GetComponent<Animator>();

            rigidBody = GetComponent<Rigidbody>();

            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void Update()
        {
            var isMovingToStopPosition = agent.remainingDistance > agent.stoppingDistance;
            if (isMovingToStopPosition)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
            {
                agent.SetDestination(enemy.transform.position);
            }
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            // Notify delegates of highest priority game object under mouse when clicked
            if (Input.GetMouseButton(0))
            {
                agent.SetDestination(destination);
            }
        }

        private Vector3 ShortDestination(Vector3 destination, float shortening)
        {
            Vector3 reductionVector = (destination - transform.position).normalized * shortening;
            return destination - reductionVector;
        }

        // adjust the speed of the root motion, of the central gravity of the character
        void OnAnimatorMove()
        {
            // we implement this function to override the default root motio.
            // This allow us to modify the positional speed before it's applied.
            if (Time.deltaTime > 0)
            {
                // animator.deltaPosition - asking unity, what are you saying the change in position of the animator should be
                // the positon multiplied by the move speed multiplier divided by the time (distance over time is equal speed)
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity
                velocity.y = rigidBody.velocity.y;
                rigidBody.velocity = velocity;
            }
        }
    }
}
