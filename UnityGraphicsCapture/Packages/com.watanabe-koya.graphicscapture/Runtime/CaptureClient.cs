using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ruccho.GraphicsCapture
{
    public class CaptureClient : IDisposable
    {
        private Capture CurrentCapture { get; set; }

        public ICaptureTarget CurrentTarget { get; private set; }

        public RenderTexture GetTexture() => CurrentCapture?.Texture;
        public void Render() => CurrentCapture?.Render();

        private bool IsDisposed { get; set; } = false;

        public void SetTarget(ICaptureTarget target)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(CaptureClient));
            
            if (CurrentCapture != null)
            {
                CurrentCapture.Dispose();
                CurrentCapture = null;
            }

            if (target != null)
            {
                CurrentCapture = new Capture(target);
                CurrentCapture.Start();
                CurrentTarget = target;
            }
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            
            CurrentCapture?.Dispose();
        }
    }
}
