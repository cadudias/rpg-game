using System;
using RPG.CameraUI;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header("Audio")]
        float audioSourceSpatialBlend = 0.5f;

        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0f, 1.03f, 0f);
        [SerializeField] float colliderRadius = .2f;
        [SerializeField] float colliderHeight = 2.03f;

        [Header("Movement")]
        [SerializeField] float moveSpeedMultiplier = .7f;
        [SerializeField] float moveThreshold = 1f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float animationSpeedMultiplier = 1.5f;

        [Header("Nav Mesh Agent")]
        [SerializeField] float navMeshAgentSpeed = 1f;
        [SerializeField] float navMeshAgentStoppingDistance = 1.3f;
        [SerializeField] [Range(.1f, 1f)] float animatorFowardCap = 1f;

        NavMeshAgent navMeshAgent;
        Animator animator;
        Rigidbody rigidBody;

        bool isAlive = true;
        float turnAmount;
        float forwardAmount;

        void Awake()
        {
            AddRequiredComponents();
        }

        private void AddRequiredComponents()
        {
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;

            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = navMeshAgentSpeed;
            navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
            navMeshAgent.autoBraking = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = true;

            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;
        }

        public float GetAnimSpeedMultiplier()
        {
            return animator.speed;
        }

        void Update()
        {
            var isMovingToStopPosition = navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
            if (isMovingToStopPosition && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public AnimatorOverrideController GetAnimatorOverrideController()
        {
            return animatorOverrideController;
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

        public void Move(Vector3 movement)
        {
            SetFowardAndTurn(movement);

            ApplyExtraTurnRotation();

            UpdateAnimator();
        }

        public void Kill()
        {
            isAlive = false;
        }

        public void SetDestination(Vector3 worldPosition)
        {
            navMeshAgent.destination = worldPosition;
        }

        void SetFowardAndTurn(Vector3 movement)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }

            var localMove = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        void UpdateAnimator()
        {
            animator.SetFloat("Forward", forwardAmount * animatorFowardCap, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animationSpeedMultiplier;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }
    }
}
