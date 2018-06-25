using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICharacterControl))]
[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentDestination;

    [SerializeField] const int walkableLayer = 8;
    [SerializeField] const int enemyLayer = 9;
    [SerializeField] const int unkownLayer = 2;

    AICharacterControl aic = null;
    GameObject walkTarget = null;

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
        aic = GetComponent<AICharacterControl>();

        walkTarget = new GameObject("walkTarget");

        cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;
    }

    void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
    {
        switch (layerHit)
        {
            case enemyLayer:
                GameObject enemy = raycastHit.collider.gameObject;
                aic.SetTarget(enemy.transform);
                break;
            case walkableLayer:
                // here we use just the position from transform and not scale and rotation
                walkTarget.transform.position = raycastHit.point;
                aic.SetTarget(walkTarget.transform);
                break;
            default:
                break;
        }
    }

    //private void ProcessDirectMovement()
    //{
    //    // read inputs
    //    float h = Input.GetAxis("Horizontal");
    //    float v = Input.GetAxis("Vertical");

    //    // calculate camera relative direction to move:
    //    Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
    //    Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

    //    thirdPersonCharacter.Move(movement, false, false);
    //}

    private Vector3 ShortDestination(Vector3 destination, float shortening)
    {
        Vector3 reductionVector = (destination - transform.position).normalized * shortening;
        return destination - reductionVector;
    }
}

