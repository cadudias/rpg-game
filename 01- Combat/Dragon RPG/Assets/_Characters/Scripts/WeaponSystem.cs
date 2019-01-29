using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        //[Range(.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        //[SerializeField] float criticalHitMultiplier = 1.25f;

        [SerializeField] float baseDamage = 30f;
        [SerializeField] WeaponConfig currentWeaponConfig = null;
        
        GameObject weaponObject;
        GameObject target;
        Animator animator;
        Character character;

        float lastHitTime;
        string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        void Start()
        {
            character = GetComponent<Character>();
            animator = GetComponent<Animator>();

            PutWeaponInHand(currentWeaponConfig);

            SetAttackAnimation();
        }

        void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;

            if (target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                targetIsDead = target.GetComponent<HealthSystem>().HealthAsPercentage <= Mathf.Epsilon;
                
                var distanceToTarget = Vector3.Distance(transform.position, character.transform.position);
                targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().HealthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if (characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                
            }
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        IEnumerator AttackTargetRepeatedly()
        {
            // determine if alive (attacker and defender)
            bool attackerStillAlive = GetComponent<HealthSystem>().HealthAsPercentage >= Mathf.Epsilon;
            bool defenderStillAlive = target.GetComponent<HealthSystem>().HealthAsPercentage >= Mathf.Epsilon;

            while (attackerStillAlive && defenderStillAlive)
            {
                float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits();
                float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier();

                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;
                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }

                yield return  new WaitForSeconds(timeToWait);
            }
        }

        private void AttackTargetOnce()
        {
            if (target != null)
            {
                transform.LookAt(target.transform);
                animator.SetTrigger(ATTACK_TRIGGER);
                SetAttackAnimation();
                StartCoroutine(DamageAfterDelay(currentWeaponConfig.GetDamageDelay()));
            }
        }

        IEnumerator DamageAfterDelay(float damageDelay)
        {
            yield return new WaitForSecondsRealtime(damageDelay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
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
            if (!character.GetAnimatorOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Provide " + gameObject + " player with override controller");
            }
            else
            {
                animator = GetComponent<Animator>();
                var animatorOverrideController = character.GetAnimatorOverrideController();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimationClip();
            }
        }

        GameObject RequestDominantHand()
        {
            var dominatHand = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominatHand.Length;
            Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominantHand added, please remove one");
            return dominatHand[0].gameObject;
        }

        float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
            // if a random number between 0 and 1 is less than or equanl to the criticalHitChance then
            // we have a critical hit
            // if criticalHitChance is 1 it's 100% chance that your number will be in the range
            // if criticalHitChance is 0.1 it's a 10% chance that a number picked randomly between 0 and 1 
            // will be less or equal to 0.1
            // 0.2 <= 1 =  true
            // 0.2 <= 0.1 = false
            // 0.1 <= 0.1 = true
            //bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            //float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage();

            // if it is critical damage, multiple the damage by the criticalHitMultiplier
            //if (isCriticalHit)
            //{
            //    criticalHitParticle.Play();
            //    return damageBeforeCritical * criticalHitMultiplier;
            //}

            //return damageBeforeCritical;
        }

        //void IsLeftClick(EnemyAI enemy)
        //{
        //    if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
        //        AttackTarget();
        //}

        private void AttackTarget()
        {
            if (AttackDelayTimeEnded())
            {
                SetAttackAnimation();
                PlayAnimation();

                //enemy.TakeDamage(CalculateDamage());
                lastHitTime = Time.time;
            }
        }

        void PlayAnimation()
        {
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        bool AttackDelayTimeEnded()
        {
            float timePassedSinceLastHit = Time.time - lastHitTime;
            return timePassedSinceLastHit > GetCurrentWeapon().GetMinTimeBetweenHits();
        }
    }
}
