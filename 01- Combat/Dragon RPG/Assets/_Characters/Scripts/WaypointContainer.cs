using UnityEngine;

namespace RPG.Characters
{
    public class WaypointContainer : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmos()
        {
            Vector3 firstPosition = transform.GetChild(0).position;
            Vector3 previousPosition = firstPosition;

            foreach (Transform waypoint in transform)
            {
                Debug.Log("aaa");
                Gizmos.DrawSphere(waypoint.position, .2f);
                Gizmos.DrawLine(previousPosition, waypoint.position);
                previousPosition = waypoint.position;
            }

            Gizmos.DrawLine(previousPosition, firstPosition);
        }
    }
}