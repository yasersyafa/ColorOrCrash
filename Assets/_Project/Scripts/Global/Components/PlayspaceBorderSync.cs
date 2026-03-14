using UnityEngine;

namespace ColorOrCrash.Global.Components
{
    public class PlayspaceBorderSync : MonoBehaviour
    {
        [Header("Playspace Reference")]
        [SerializeField] private RectTransform playspaceRect; // UI frame yang stretch
        [SerializeField] private Camera uiCamera;             // Camera yang dipakai Canvas

        [Header("Border Colliders")]
        [SerializeField] private BoxCollider2D topBorder;
        [SerializeField] private BoxCollider2D bottomBorder;
        [SerializeField] private BoxCollider2D leftBorder;
        [SerializeField] private BoxCollider2D rightBorder;

        [Header("Settings")]
        [SerializeField] private float borderThickness = 1f;
        [SerializeField, Range(0f, 0.5f)] private float paddingTopPercent    = 2f;
        [SerializeField, Range(0f, 0.5f)] private float paddingBottomPercent = 2f;
        [SerializeField, Range(0f, 0.5f)] private float paddingLeftPercent   = 2f;
        [SerializeField, Range(0f, 0.5f)] private float paddingRightPercent  = 2f;

        private void Start()
        {
            SyncBorders();
        }

        // Kalau mau safe, sync juga tiap frame (misal saat resize window di editor)
        #if UNITY_EDITOR
        private void Update()
        {
            SyncBorders();
        }
        #endif

        private void SyncBorders()
        {
            if (playspaceRect == null || uiCamera == null) return;

            // Ambil 4 corner RectTransform dalam world space
            Vector3[] corners = new Vector3[4];
            playspaceRect.GetWorldCorners(corners);
            // corners[0] = bottom-left
            // corners[1] = top-left
            // corners[2] = top-right
            // corners[3] = bottom-right

            Vector3 bottomLeft  = corners[0];
            Vector3 topLeft     = corners[1];
            Vector3 topRight    = corners[2];
            Vector3 bottomRight = corners[3];

            float width  = Vector3.Distance(bottomLeft, bottomRight);
            float height = Vector3.Distance(bottomLeft, topLeft);

            // Padding dalam world unit, proporsional terhadap ukuran rect
            float padTop    = height * paddingTopPercent;
            float padBottom = height * paddingBottomPercent;
            float padLeft   = width  * paddingLeftPercent;
            float padRight  = width  * paddingRightPercent;

            Vector2 center = (bottomLeft + topRight) * 0.5f;

            // TOP
            if (topBorder != null)
            {
                topBorder.transform.position = new Vector3(center.x, topLeft.y - padTop, 0);
                topBorder.size = new Vector2(width - padLeft - padRight, borderThickness);
            }

            // BOTTOM
            if (bottomBorder != null)
            {
                bottomBorder.transform.position = new Vector3(center.x, bottomLeft.y + padBottom, 0);
                bottomBorder.size = new Vector2(width - padLeft - padRight, borderThickness);
            }

            // LEFT
            if (leftBorder != null)
            {
                leftBorder.transform.position = new Vector3(bottomLeft.x + padLeft, center.y, 0);
                leftBorder.size = new Vector2(borderThickness, height - padTop - padBottom);
            }

            // RIGHT
            if (rightBorder != null)
            {
                rightBorder.transform.position = new Vector3(bottomRight.x - padRight, center.y, 0);
                rightBorder.size = new Vector2(borderThickness, height - padTop - padBottom);
            }
        }
    }
}