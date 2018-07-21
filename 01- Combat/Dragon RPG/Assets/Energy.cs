using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float pointsPerHit = 10f;

        float currentEnergyPoints;
        CameraRaycaster cameraRaycaster;

        // Use this for initialization
        void Start()
        {
            //energyBar = GetComponent<RawImage>();
            currentEnergyPoints = maxEnergyPoints;
            RegisterForMouseClick();
        }

        private void RegisterForMouseClick()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyRightClickObservers += OnMouseClick;
        }

        void OnMouseClick()
        {
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints - pointsPerHit, 0f, maxEnergyPoints);

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
