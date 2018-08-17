using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    [RequireComponent(typeof(Image))]
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyOrb;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 1f;
        [SerializeField] AudioClip outOfEnergySound;

        float currentEnergyPoints;
        AudioSource audioSource;

        public float energyAsPercentage
        {
            get
            {
                return currentEnergyPoints / maxEnergyPoints;
            }
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            currentEnergyPoints = maxEnergyPoints;
            UpdateEnergyBar();
            AttachInitialAbilities();
        }

        void Update()
        {
            bool isNotMaxEnergyPoint = currentEnergyPoints < maxEnergyPoints;
            if (isNotMaxEnergyPoint)
            {
                AddEnergyPoints();
                UpdateEnergyBar();
            }
        }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        public void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            bool hasEnergy = energyCost <= currentEnergyPoints;
            if (hasEnergy)
            {
                ConsumeEnergy(energyCost);
                abilities[abilityIndex].Use(target);
            }
            else
            {
                audioSource.PlayOneShot(outOfEnergySound);
            }
        }

        private void AddEnergyPoints()
        {
            var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        public void ConsumeEnergy(float amount)
        {
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints - amount, 0f, maxEnergyPoints);
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            energyOrb.fillAmount = energyAsPercentage;
        }
    }
}
