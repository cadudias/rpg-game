using UnityEngine;

namespace RPG.Characters
{
    // Add a UI Socket transform to your enemy
    // Attach this script to the socket
    // Link to a canvas prefab
    public class FaceCamera : MonoBehaviour
    {
        Camera cameraToLookAt;

        // Use this for initialization 
        void Start()
        {
            cameraToLookAt = Camera.main;
        }

        // Update is called once per frame 
        void LateUpdate()
        {
            // get the camera position (cameraToLookAt.transform) and points the cloned game object to it
            // this will point the cloned game oject, in this case the enemy canvas prefab that contains the health bar, to the camera
            transform.LookAt(cameraToLookAt.transform);
        }
    }
}