using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent((typeof(Character)))]
    [RequireComponent((typeof(WeaponSystem)))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float chaseRadius = 6f;
        private Character character;

        float currentHealthPoints;
        float currentWeaponRange;
        private float distanceToPlayer;

        enum State
        { 
            idle, 
            patrolling, 
            attacking, 
            chasing
        }

        private State state = State.idle;

        PlayerMovement player;

        void Start()
        {
            player = FindObjectOfType<PlayerMovement>();
            character = GetComponent<Character>();
        }

        void Update()
        {
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            bool playerIsOutOfRangeAndEnemyIsNotPatrolling = distanceToPlayer > chaseRadius && state != State.patrolling;
            if (playerIsOutOfRangeAndEnemyIsNotPatrolling)
            {
                StopAllCoroutines();
                state = State.patrolling;
            }

            bool playerIsInRangeAndEnemyIsNotChasing = distanceToPlayer <= chaseRadius && state != State.chasing;
            if (playerIsInRangeAndEnemyIsNotChasing)
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }

            bool playerIsInWeaponRangeAndEnenyIsNotAttacking = distanceToPlayer <= currentWeaponRange && state != State.attacking;
            if (playerIsInWeaponRangeAndEnenyIsNotAttacking)
            {
                StopAllCoroutines();
                state = State.attacking;
            }
        }

        IEnumerator ChasePlayer()
        {
            state = State.chasing;
            //stop chasing when player gets inside the weapon radius
            while (distanceToPlayer >= currentWeaponRange)
            {
                // every frame set the destination of the enemy to the player
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}
