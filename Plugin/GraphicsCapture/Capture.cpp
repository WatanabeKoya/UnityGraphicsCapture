#include "pch.h"
#include "Capture.h"
#include "Unity/IUnityInterface.h"
#include "Unity/IUnityGraphicsD3D11.h"

using namespace Microsoft::WRL;

namespace winrt
{
	using namespace Windows::Foundation;
	using namespace Windows::Graphics;
	using namespace Windows::Graphics::Capture;
	using namespace Windows::Graphics::DirectX;
	using namespace Windows::Graphics::DirectX::Direct3D11;
}

Capture::Capture(winrt::IDirect3DDevice const& device, winrt::GraphicsCaptureItem const& item, winrt::DirectXPixelFormat pixelFormat, winrt::com_ptr<ID3D11Device> unityDevice, ComPtr<ID3D11DeviceContext> unityContext)
{
	m_item = item;
	m_device = device;
	m_pixelFormat = pixelFormat;

	m_framePool = winrt::Direct3D11CaptureFramePool::Create(m_device, m_pixelFormat, 1, m_item.Size());
	m_session = m_framePool.CreateCaptureSession(m_item);
	m_lastSize = m_item.Size();
	m_framePool.FrameArrived({ this, &Capture::OnFrameArrived });

	m_context = unityContext;

	m_bufferDevice = unityDevice;
	CreateBufferTexture();

	WINRT_ASSERT(m_session != nullptr);
}

void Capture::OnFrameArrived(winrt::Direct3D11CaptureFramePool const& sender, winrt::IInspectable const&)
{
	std::unique_lock<std::mutex> lock(m_render_mutex, std::try_to_lock);
	if (!lock.owns_lock()) return;

	auto frame = m_framePool.TryGetNextFrame();
	if (!frame) return;

	auto surface = frame.Surface();

	auto contentSize = frame.ContentSize();

	if ((contentSize.Width != m_lastSize.Width) || (contentSize.Height != m_lastSize.Height))
	{
		m_lastSize = contentSize;
		m_framePool.Recreate(m_device, m_pixelFormat, 1, m_lastSize);
		CreateBufferTexture();
	}

	auto interop = surface.as<IDirect3DDxgiInterfaceAccess>();
	ComPtr<ID3D11Texture2D> srcTexture;
	interop->GetInterface(IID_PPV_ARGS(&srcTexture));

	m_context->CopyResource(m_bufferTexture, srcTexture.Get());
}

void Capture::CreateBufferTexture()
{
	if (m_bufferTexture) m_bufferTexture->Release();
	D3D11_TEXTURE2D_DESC desc = {};
	desc.Width = m_lastSize.Width;
	desc.Height = m_lastSize.Height;
	desc.MipLevels = 1;
	desc.ArraySize = 1;
	desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
	desc.SampleDesc.Count = 1;
	desc.Usage = D3D11_USAGE_DEFAULT;

	m_bufferDevice->CreateTexture2D(&desc, nullptr, &m_bufferTexture);
}

void Capture::StartCapture()
{
	m_session.StartCapture();
}

void Capture::Close()
{
	auto expected = false;
	m_session.Close();
	m_framePool.Close();

	m_framePool = nullptr;
	m_session = nullptr;
	m_item = nullptr;
}

int Capture::GetWidth()
{
	return m_lastSize.Width;
}

int Capture::GetHeight()
{
	return m_lastSize.Height;
}

void Capture::Render(ID3D11Texture2D* unityTexture)
{
	if (!m_framePool || !unityTexture) return;
	std::lock_guard<std::mutex> lock(m_render_mutex);
	m_context->CopyResource(unityTexture, m_bufferTexture);
	return;
}
