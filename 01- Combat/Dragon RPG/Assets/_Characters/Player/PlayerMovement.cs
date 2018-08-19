using UnityEngine;
using UnityEngine.Assertions;
using System;

using RPG.CameraUI; // for mouse events

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        CameraRaycaster cameraRaycaster;
        SpecialAbilities abilities;
        EnemyAI enemy;
        Character character;
        WeaponSystem weaponSystem;

        [SerializeField] ParticleSystem criticalHitParticle = null;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();

            abilities = GetComponent<SpecialAbilities>();

            RegisterForMouseEvents();
        }

        void Update()
        {
            ScanForAbilityKeyDown();
        }

        private void RegisterForMouseEvents()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void OnMouseOverEnemy(EnemyAI enemyToSet)
        {
            enemy = enemyToSet;

            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0);
            }
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
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

        bool IsTargetInRange(GameObject target)
        {
            float distanceTotarget = (target.transform.position - transform.position).magnitude;
            bool isInRange = distanceTotarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
            return isInRange;
        }
    }
}