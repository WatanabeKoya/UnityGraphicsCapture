using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ruccho.GraphicsCapture
{
    [RequireComponent(typeof(Renderer))]
    public class CaptureTexture : MonoBehaviour
    {
        public ICaptureTarget CurrentTarget => client.CurrentTarget;

        private CaptureClient client = new CaptureClient();
        private Renderer targetRenderer = default;

        public void SetTarget(ICaptureTarget target)
        {
            try
            {
                client.SetTarget(target);
                if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
                if (!targetRenderer || !targetRenderer.material) return;
                targetRenderer.material.mainTexture = client.GetTexture();
            }
            catch (CreateCaptureException)
            {
                Debug.LogWarning("This target cannot be captured!");
            }
        }

        private void Update()
        {
            client?.Render();
        }

        private void OnDestroy()
        {
            client?.Dispose();
        }
    }
}