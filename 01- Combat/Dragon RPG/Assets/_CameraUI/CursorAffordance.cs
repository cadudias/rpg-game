using UnityEngine;

namespace RPG.CameraUI
{
    // add CursorAffordance script to a camera again it will bring the camera raycaster with it and it's required
    [RequireComponent(typeof(CameraRaycaster))]
    public class CursorAffordance : MonoBehaviour
    {

        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D attackCursor = null;
        //[SerializeField] Texture2D unknowCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        [SerializeField] const int walkableLayer = 8;
        [SerializeField] const int enemyLayer = 9;
        [SerializeField] const int unkownLayer = 2;

        public CameraRaycaster cameraRaycaster;

        void Start()
        {
            cameraRaycaster = GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyLayerChangedObservers += OnLayerChanged; // register 
        }

        void OnLayerChanged(int newLayer) // only called when layer changes
        {
            switch (newLayer)
            {
                case walkableLayer:
                    Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                    break;
                case enemyLayer:
                    Cursor.SetCursor(attackCursor, cursorHotspot, CursorMode.Auto);
                    break;
                default:
                    //Cursor.SetCursor(unknowCursor, cursorHotspot, CursorMode.Auto);
                    return;
            }
        }

        // TODO consider de-registering OnLayerChnaged on leaving all game scenes
    }
}