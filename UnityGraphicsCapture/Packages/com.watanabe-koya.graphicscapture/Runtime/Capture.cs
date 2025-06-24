using System;
using Ruccho.GraphicsCapture.Native;
using UnityEngine;

namespace Ruccho.GraphicsCapture
{
    public class Capture : IDisposable
    {

        private IntPtr SelfPtr { get; }
        private IntPtr texturePtr = IntPtr.Zero;

        private int prevWidth = -1;
        private int prevHeight = -1;

        public RenderTexture Texture { get; }
        public bool IsCapturing { get; private set; }

        private bool IsDisposed { get; set; }

        public Capture(ICaptureTarget target)
        {
            switch(target.TargetType)
            {
                case CaptureTargetType.Window:
                    SelfPtr = NativeCapture.CreateCaptureFromWindow(target.Handle);
                    break;
                case CaptureTargetType.Monitor:
                    SelfPtr = NativeCapture.CreateCaptureFromMonitor(target.Handle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (SelfPtr == IntPtr.Zero)
            {
                throw new CreateCaptureException("CreateCapture() returned null. Failed to create Capturing instance.");
            }
            Texture = new RenderTexture(NativeCapture.GetWidth(SelfPtr), NativeCapture.GetHeight(SelfPtr), 0, RenderTextureFormat.BGRA32);
            Texture.Create();
        }

        public void Start()
        {
            if(IsDisposed) throw new ObjectDisposedException(nameof(Capture));
            if(IsCapturing) throw new InvalidOperationException();
            NativeCapture.StartCapture(SelfPtr);
            IsCapturing = true;
        }

        public void Render()
        {
            if (!IsCapturing || IsDisposed) return;
            var width = NativeCapture.GetWidth(SelfPtr);
            var height = NativeCapture.GetHeight(SelfPtr);
            if (width != prevWidth || height != prevHeight)
            {
                Texture.Release();
                Texture.width = width;
                Texture.height = height;
                Texture.Create();
                texturePtr = Texture.GetNativeTexturePtr();
                prevWidth = width;
                prevHeight = height;
            }
            NativeCapture.Render(SelfPtr, texturePtr);
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            if (SelfPtr != IntPtr.Zero)
                NativeCapture.CloseCapture(SelfPtr);
            IsCapturing = false;
        }

        #if UNITY_EDITOR

        ~Capture()
        {
            if (IsDisposed || SelfPtr == IntPtr.Zero) return;
            Debug.LogWarning("[GraphicsCapture] A capture object has not been disposed! Make sure capture object to be disposed to stop unexpected capturing.");
        }

        #endif
    }
}