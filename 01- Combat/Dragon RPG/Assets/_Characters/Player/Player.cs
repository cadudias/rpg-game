using UnityEngine;
using UnityEngine.Assertions;

// TODO consider re-wiring
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamagable
    {
        [SerializeField] float damagePerShot = 30f;

        [SerializeField] PlayerWeapon weaponInUse = null;

        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        Animator animator;
        string ATTACK_ANIMATION = "Attack";

        public CameraRaycaster cameraRaycaster;

        float lastHitTime = 0;
        float currentHealthPoints;

        [SerializeField] float maxHealthPoints = 300f;

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        void Start()
        {
            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
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

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget(enemy);
            }
        }

        private void AttackTarget(Enemy enemy)
        {
            if (AttackDelayTimeEnded())
            {
                PlayAnimation();

                enemy.TakeDamage(damagePerShot);
                lastHitTime = Time.time;
            }
        }

        private void PlayAnimation()
        {
            animator.SetTrigger(ATTACK_ANIMATION);
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
        }
    }
}