using UnityEngine;

// Add a UI Socket transform to your enemy
// Attach this script to the socket
// Link to a canvas prefab
public class EnemyUI : MonoBehaviour {

    // Works around Unity 5.5's lack of nested prefabs
    [Tooltip("The UI canvas prefab")]

    [SerializeField] GameObject enemyCanvasPrefab = null;
     
    Camera cameraToLookAt;

    // Use this for initialization 
    void Start()
    {
        cameraToLookAt = Camera.main;

        //creates a copy of enemyCanvasPrefab
        Instantiate(enemyCanvasPrefab, transform.position, Quaternion.identity, transform);
    }

    // Update is called once per frame 
    void LateUpdate()
    {
        // get the camera position (cameraToLookAt.transform) and points the cloned game object to it
        // this will point the cloned game oject, in this case the enemy canvas prefab that contains the health bar, to the camera
        transform.LookAt(cameraToLookAt.transform);
    }
}