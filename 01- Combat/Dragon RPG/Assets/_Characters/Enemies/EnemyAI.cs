using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] private GameObject patrolPath;
        [SerializeField] private float waypointTolerance = 2f;

        private Character character;

        private int nextWaypointIndex;
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
                StartCoroutine(Patrol());
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
                //weaponSystem.AttackTarget(character.gameObject);
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrolling;

            while (true)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);

                CycleWaypointWhenClose(nextWaypointPos);
                yield return new WaitForSeconds(1.5f);
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        {
            // when enemy get within 2 meters we decide to increment the index of the waypoint
            // so the enemy will start patrolling on it's designated route
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;   
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
