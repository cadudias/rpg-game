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
        [SerializeField] float deathVanishSeconds = .1f;

        [SerializeField] float currentHealthPoints;

        private const string DEATH_TRIGGER = "Death";
        Animator animator;
        AudioSource audioSource;
        Character characterMovement;

        bool isDestroyed = false;

        public float HealthAsPercentage
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
                Debug.Log("HealthAsPercentage: " + HealthAsPercentage);
                healthBar.fillAmount = HealthAsPercentage;
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);

            var clip = damageSounds[Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip); // if other sounds are playing then can continue

            bool characterDies = (currentHealthPoints <= 0);
            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
        }

        IEnumerator KillCharacter()
        {
            characterMovement.Kill(); // tell character movement that the dying is happening
            animator.SetTrigger(DEATH_TRIGGER);

            audioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)];
            audioSource.Play(); // override any existing sound
            yield return new WaitForSecondsRealtime(audioSource.clip.length);
            //yield return new WaitForSecondsRealtime(0);

            var playerComponent = GetComponent<PlayerControl>();
            if (playerComponent && playerComponent.isActiveAndEnabled) // relying on lazy evaluation
            {
                SceneManager.LoadScene(0); // reload scene
            }
            else
            {
                if (!isDestroyed)
                {
                    Debug.Log("destroy char");
                    Destroy(gameObject, deathVanishSeconds);
                    isDestroyed = true;
                }
            }
        }

        public void Heal(float pointsToHeal)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + pointsToHeal, 0f, maxHealthPoints);
        }
    }
}