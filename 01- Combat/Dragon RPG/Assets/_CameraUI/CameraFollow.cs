using UnityEngine;

namespace RPG.CameraUI
{
    public class CameraFollow : MonoBehaviour
    {

        public GameObject player; //Public variable to store a reference to the player game object

        // Use this for initialization
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            //Debug.Log("player", player);
        }

        // BONUS - CONSIDER USING LATEUPDATE
        //LateUpdate is called after all Update functions have been called.This is useful to order script execution.For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.

        void LateUpdate()
        {
            transform.position = player.transform.position;
        }
    }
}
