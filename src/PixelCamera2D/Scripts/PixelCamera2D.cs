﻿using UnityEngine;

namespace RyanNielson.PixelCamera2D
{
    [ExecuteInEditMode]
    public class PixelCamera2D : MonoBehaviour
    {
        [SerializeField]
        private int baseWidth = 400;

        [SerializeField]
        private int baseHeight = 240;

        [SerializeField]
        private PixelCamera2DBehaviour behaviour;

        [SerializeField]
        private MeshRenderer quad;

        public int BaseWidth { get { return baseWidth; } }
        public int BaseHeight { get { return baseHeight; } }

        private Camera pixelCamera;
        private Camera pixelCameraRenderer;

        private int previousWidth = 0;
        private int previousHeight = 0;
        private PixelCamera2DBehaviour previousBehaviour;

        void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            pixelCamera = GetComponent<Camera>();
            pixelCameraRenderer = GetPixelCameraRenderer(pixelCamera);
        }

        private void Update()
        {
            if (Screen.width != previousWidth || Screen.height != previousHeight || previousBehaviour != behaviour)
            {
                UpdatePreviousValues();

                UpdateCamera();
            }
        }

        public void SetRenderTexture(RenderTexture renderTexture)
        {
            pixelCamera.targetTexture = renderTexture;
            quad.sharedMaterial.mainTexture = renderTexture;
        }

        private void UpdateCamera()
        {
            if (behaviour == PixelCamera2DBehaviour.BestPixelPerfectFit)
            {
                BestFitBehaviour();
            }
            else if (behaviour == PixelCamera2DBehaviour.ScaleToFit)
            {
                ScaleBehaviour();
            }
        }

        private void BestFitBehaviour()
        {
            int nearestWidth = Screen.width / baseWidth * baseWidth;
            int nearestHeight = Screen.height / baseHeight * baseHeight;

            int xScaleFactor = nearestWidth / baseWidth;
            int yScaleFactor = nearestHeight / baseHeight;

            int scaleFactor = yScaleFactor < xScaleFactor ? yScaleFactor : xScaleFactor;

            float heightRatio = (baseHeight * (float)scaleFactor) / Screen.height;

            quad.transform.localScale = new Vector3(baseWidth / (float)baseHeight * heightRatio, 1f * heightRatio, 1f);

            // Offset the camera rect in odd screen sizes to prevent subpixel issues.
            pixelCameraRenderer.rect = new Rect(GetCameraRectOffset(Screen.width), GetCameraRectOffset(Screen.height), pixelCameraRenderer.rect.width, pixelCameraRenderer.rect.height);
        }

        private void ScaleBehaviour()
        {
            float targetAspectRatio = baseWidth / (float)baseHeight;
            float windowAspectRatio = Screen.width / (float)Screen.height;
            float scaleHeight = windowAspectRatio / targetAspectRatio;

            scaleHeight = scaleHeight * (1.15505f / windowAspectRatio);

            if (scaleHeight < 1f)
            {
                quad.transform.localScale = new Vector3(targetAspectRatio * scaleHeight, scaleHeight, 1f);
            }
            else
            {
                quad.transform.localScale = new Vector3(targetAspectRatio, 1f, 1f);
            }
        }

        private void UpdatePreviousValues()
        {
            previousWidth = Screen.width;
            previousHeight = Screen.height;
            previousBehaviour = behaviour;
        }

        private Camera GetPixelCameraRenderer(Camera cameraToIgnore)
        {
            foreach (Camera possiblePixelCameraRenderer in GetComponentsInChildren<Camera>())
            {
                if (possiblePixelCameraRenderer != cameraToIgnore)
                {
                    return possiblePixelCameraRenderer;
                }
            }

            return null;
        }

        private float GetCameraRectOffset(int size)
        {
            return size % 2 == 0 ? 0 : 1f / size;
        }
    }
}