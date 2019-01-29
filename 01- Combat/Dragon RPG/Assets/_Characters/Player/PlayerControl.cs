using System.Collections;
using UnityEngine;
using RPG.CameraUI; // for mouse events

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        SpecialAbilities abilities;
        Character character;
        WeaponSystem weaponSystem;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();
        }

        void Update()
        {
            ScanForAbilityKeyDown();
        }

        private void RegisterForMouseEvents()
        {
            var cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceTotarget = (target.transform.position - transform.position).magnitude;
            bool isInRange = distanceTotarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
            return isInRange;
        }

        void OnMouseOverEnemy(EnemyAI enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
            {
                StartCoroutine(MoveAndAttack(enemy));
            }
            else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemy.gameObject))
            {
                abilities.AttemptSpecialAbility(0, enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1) && !IsTargetInRange(enemy.gameObject))
            {
                StartCoroutine(MoveAndPowerAttack(enemy));
            }
        }

        IEnumerator MoveToTarget(EnemyAI target)
        {
            character.SetDestination(target.transform.position);

            // allowing the character to continue to move to it's destination
            while (!IsTargetInRange(target.gameObject))
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttack(EnemyAI target)
        {
            // we can call a coroutine from inside another
            // first move to target
            yield return StartCoroutine(MoveToTarget(target));
            weaponSystem.AttackTarget(target.gameObject);
        }

        IEnumerator MoveAndPowerAttack(EnemyAI target)
        {
            // we can call a coroutine from inside another
            // first move to target
            yield return StartCoroutine(MoveToTarget(target));
            abilities.AttemptSpecialAbility(0, target.gameObject);
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                weaponSystem.StopAttacking();
                character.SetDestination(destination);
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }
    }
}