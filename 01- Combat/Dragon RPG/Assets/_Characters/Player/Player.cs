using UnityEngine;
using UnityEngine.Assertions;

// TODO consider re-wiring
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        public CameraRaycaster cameraRaycaster;

        [SerializeField] float baseDamage = 30f;
        [SerializeField] float maxHealthPoints = 300f;
        [SerializeField] public AbilityConfig[] abilities;
        [SerializeField] PlayerWeapon weaponInUse = null;

        [Range(.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;

        float lastHitTime = 0;
        float currentHealthPoints;

        Energy energy = null;
        Enemy enemy = null;

        /* Animation */
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        Animator animator = null;
        string DEATH_TRIGGER = "Death";
        string ATTACK_TRIGGER = "Attack";

        /* SOUND */
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        AudioSource audioSource = null;

        [SerializeField] ParticleSystem criticalHitParticle = null;

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();

            RegisterForMouseClick();

            SetCurrentMaxHealth();

            PutWeaponInHand();

            SetupRuntimeAnimator();

            AttachInitialAbilities();
        }

        private void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachComponent(gameObject);
            }
        }

        void Update()
        {
            if (healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.Length; keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    AttemptSpecialAbility(keyIndex);
                }
            }
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAttackAnimationClip();
        }

        private void SetCurrentMaxHealth() { currentHealthPoints = maxHealthPoints; }

        private void PutWeaponInHand()
        {
            // get default weapon
            var weaponPrefab = weaponInUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            var weapon = Instantiate(weaponPrefab, dominantHand.transform);
            // set the weapon postion based on weaponGrip
            weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        private GameObject RequestDominantHand()
        {
            var dominatHand = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominatHand.Length;
            Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on Player, please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominantHand added, please remove one");
            return dominatHand[0].gameObject;
        }

        private void RegisterForMouseClick()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
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
                AttemptSpecialAbility(0);
            }
        }

        private void AttemptSpecialAbility(int abilityIndex)
        {
            energy = GetComponent<Energy>();

            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if (energy.IsEnergyAvailable(energyCost))
            {
                energy.ConsumeEnergy(energyCost);

                var abilityParams = new AbilityUseParams(enemy, baseDamage);
                abilities[abilityIndex].Use(abilityParams);
            }
        }

        private void IsLeftClick(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
                AttackTarget();
        }

        private void AttackTarget()
        {
            if (AttackDelayTimeEnded())
            {
                PlayAnimation();

                enemy.TakeDamage(CalculateDamage());
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage()
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
            float damageBeforeCritical = baseDamage + weaponInUse.GetAdditionalDamage();

            // if it is critical damage, multiple the damage by the criticalHitMultiplier
            if (isCriticalHit)
            {
                criticalHitParticle.Play();
                return damageBeforeCritical * criticalHitMultiplier;
            }

            return damageBeforeCritical;
        }

        private void PlayAnimation()
        {
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        private bool AttackDelayTimeEnded()
        {
            float timePassedSinceLastHit = Time.time - lastHitTime;
            return timePassedSinceLastHit > weaponInUse.GetMinTimeBetweenHits();
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceTotarget = (target.transform.position - transform.position).magnitude;

            return distanceTotarget <= weaponInUse.GetMaxAttackRange();
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);

            audioSource.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.Play();

            if (currentHealthPoints <= 0) // player dies
            {
                StartCoroutine(KillPlayer());
            }
        }

        public void Heal(float pointsToHeal)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + pointsToHeal, 0f, maxHealthPoints);
        }

        IEnumerator KillPlayer()
        {
            animator.SetTrigger(DEATH_TRIGGER);

            // reload scene with SceneManager.Reload
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();
            yield return new WaitForSecondsRealtime(2f); // TODO use audio clip later
            SceneManager.LoadScene(0);
        }
    }
}