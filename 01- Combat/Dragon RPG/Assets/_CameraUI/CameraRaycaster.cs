using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using RPG.Characters; // so we can detect by type, if they have the EnemyAI component on them
using System;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        const int POTENTIALY_WALKABLE_LAYER = 8;

        float maxRaycastDepth = 100f; // Hard coded value

        public delegate void OnMouseOverTerrain(Vector3 destination);
        public event OnMouseOverTerrain onMouseOverPotentiallyWalkable;

        public delegate void OnMouseOverEnemy(EnemyAI enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        // drawn a rectangle of th size of the screen
        Rect screenRectAtStartPlay = new Rect(0, 0, Screen.width, Screen.height); // move inside update to support screen resize

        void Update()
        {
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Stop looking for other objects
            }
            else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // form the ray to check layers

            //order matters, tis is our layer priority
            if (RaycastForEnemy(ray))
            {
                return;
            }
            if (RaycastForPotentiallyWalkable(ray))
            {
                return;
            }
        }

        bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, maxRaycastDepth))
            {
                var gameObjectHit = hitInfo.collider.gameObject;
                var enemyHit = gameObjectHit.GetComponent<EnemyAI>();

                if (enemyHit)
                {
                    Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                    onMouseOverEnemy(enemyHit);
                    return true;
                }
            }

            return false;
        }

        bool RaycastForPotentiallyWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask potentiallyWalkableLayer = 1 << POTENTIALY_WALKABLE_LAYER;
            // do a raycast through the camera that will oly sucssed if we hit a potentially walkable layer, something that we have agged as potentially walkable
            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);

            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverPotentiallyWalkable(hitInfo.point);
                return true;
            }

            return false;
        }
    }
}