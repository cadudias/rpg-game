using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent((typeof(WeaponSystem)))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float chaseRadius = 6f;

        public bool IsAttacking = false;
        float currentHealthPoints;
        float currentWeaponRange;

        PlayerMovement player;

        void Start()
        {
            player = FindObjectOfType<PlayerMovement>();
        }

        void Update()
        {
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
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
