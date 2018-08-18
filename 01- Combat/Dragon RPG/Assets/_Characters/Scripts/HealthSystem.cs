using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 300f;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] float deathVanishSeconds = 1f;

        float currentHealthPoints;

        const string DEATH_TRIGGER = "Death";
        Animator animator;
        AudioSource audioSource;
        Character characterMovement;

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<Character>();

            currentHealthPoints = maxHealthPoints;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
            if (healthBar)
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);

            var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip); // if other sounds are playing then can continue

            bool characterDies = (currentHealthPoints - damage <= 0);
            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
        }

        IEnumerator KillCharacter()
        {
            StopAllCoroutines();
            characterMovement.Kill(); // tell character movement that the dying is happening
            animator.SetTrigger(DEATH_TRIGGER);

            var playerComponent = GetComponent<PlayerMovement>();
            if (playerComponent && playerComponent.isActiveAndEnabled) // relying on lazy evaluation
            {
                audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
                audioSource.Play(); // override any existing sound
                yield return new WaitForSecondsRealtime(2f); // TODO use audio clip later
                SceneManager.LoadScene(0); // reload scene
            }
            else
            {
                Destroy(gameObject, deathVanishSeconds);
            }
        }

        public void Heal(float pointsToHeal)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + pointsToHeal, 0f, maxHealthPoints);
        }
    }
}