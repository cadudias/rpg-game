using UnityEngine;
using UnityEngine.Assertions;
using System;

using RPG.CameraUI; // for mouse events

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        public CameraRaycaster cameraRaycaster;

        [SerializeField] float baseDamage = 30f; 
        [SerializeField] PlayerWeapon currentWeaponConfig = null;

        GameObject weaponObject;

        [Range(.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;

        float lastHitTime = 0;

        SpecialAbilities abilities;
        Enemy enemy;
        Character character;

        /* Animation */
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        Animator animator = null;
        string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        [SerializeField] ParticleSystem criticalHitParticle = null;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();

            abilities = GetComponent<SpecialAbilities>();

            RegisterForMouseEvents();

            PutWeaponInHand(currentWeaponConfig);

            SetAttackAnimation();
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

        void OnMouseOverEnemy(Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget();
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

        public void PutWeaponInHand(PlayerWeapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            // get default weapon
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject); // emprty hands
            // set the weapon postion based on weaponGrip
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimationClip();
        }

        GameObject RequestDominantHand()
        {
            var dominatHand = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominatHand.Length;
            Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominantHand added, please remove one");
            return dominatHand[0].gameObject;
        }

        void IsLeftClick(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
                AttackTarget();
        }

        void AttackTarget()
        {
            if (AttackDelayTimeEnded())
            {
                SetAttackAnimation();
                PlayAnimation();

                enemy.TakeDamage(CalculateDamage());
                lastHitTime = Time.time;
            }
        }

        float CalculateDamage()
        {
            // if a random number between 0 and 1 is less than or equanl to the criticalHitChance then
            // we have a critical hit
            // if criticalHitChance is 1 it's 100% chance that your number will be in the range
            // if criticalHitChance is 0.1 it's a 10% chance that a number picked randomly between 0 and 1 
            // will be less or equal to 0.1
            // 0.2 <= 1 =  true
            // 0.2 <= 0.1 = false
            // 0.1 <= 0.1 = true
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage();

            // if it is critical damage, multiple the damage by the criticalHitMultiplier
            if (isCriticalHit)
            {
                criticalHitParticle.Play();
                return damageBeforeCritical * criticalHitMultiplier;
            }

            return damageBeforeCritical;
        }

        void PlayAnimation()
        {
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        bool AttackDelayTimeEnded()
        {
            float timePassedSinceLastHit = Time.time - lastHitTime;
            return timePassedSinceLastHit > currentWeaponConfig.GetMinTimeBetweenHits();
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceTotarget = (target.transform.position - transform.position).magnitude;

            return distanceTotarget <= currentWeaponConfig.GetMaxAttackRange();
        }

        void TakeDamage(float damage)
        {
            throw new NotImplementedException();
        }
    }
}