using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100f;

        float currentEnergyPoints;

        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
        }

        public bool IsEnergyAvailable(float amount)
        {
            return amount < currentEnergyPoints;
        }

        public void ConsumeEnergy(float amount)
        {
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints - amount, 0f, maxEnergyPoints);
            UpdateEnergyBar();   
        }

        private void UpdateEnergyBar()
        {
            float energyPointsAsPercentage = currentEnergyPoints / maxEnergyPoints;

            float rawImageXValue = -(energyPointsAsPercentage / 2f) - 0.5f;
            energyBar.uvRect = new Rect(rawImageXValue, 0f, 0.5f, 1f);
        }
    }
}
