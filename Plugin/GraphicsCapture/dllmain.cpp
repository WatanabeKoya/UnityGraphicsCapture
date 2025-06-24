﻿
#include "pch.h"
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#include "Capture.h"
#include "Unity/IUnityInterface.h"
#include "Unity/IUnityGraphicsD3D11.h"


namespace winrt
{
	using namespace Windows::Graphics::Capture;
}

using namespace Microsoft::WRL;

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:

	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

namespace
{
	winrt::com_ptr<ID3D11Device> g_unityDevice = nullptr;
	ComPtr<ID3D11DeviceContext> g_unityContext = nullptr;
	winrt::impl::com_ref<winrt::Windows::Graphics::DirectX::Direct3D11::IDirect3DDevice> g_device = nullptr;

	auto CreateCaptureItemForWindow(HWND hwnd)
	{
		auto interop_factory = winrt::get_activation_factory<winrt::GraphicsCaptureItem, IGraphicsCaptureItemInterop>();
		winrt::GraphicsCaptureItem item = { nullptr };
		winrt::check_hresult(interop_factory->CreateForWindow(hwnd, winrt::guid_of<ABI::Windows::Graphics::Capture::IGraphicsCaptureItem>(), winrt::put_abi(item)));
		return item;
	}

	auto CreateCaptureItemForMonitor(HMONITOR hmon)
	{
		auto interop_factory = winrt::get_activation_factory<winrt::GraphicsCaptureItem, IGraphicsCaptureItemInterop>();
		winrt::GraphicsCaptureItem item = { nullptr };
		winrt::check_hresult(interop_factory->CreateForMonitor(hmon, winrt::guid_of<ABI::Windows::Graphics::Capture::IGraphicsCaptureItem>(), winrt::put_abi(item)));
		return item;
	}

	auto CreateCapture(winrt::GraphicsCaptureItem item)
	{
		auto pixelFormat = winrt::Windows::Graphics::DirectX::DirectXPixelFormat::B8G8R8A8UIntNormalized;

		auto capture = new Capture(g_device, item, pixelFormat, g_unityDevice, g_unityContext);

		return capture;
	}
}


extern "C"
{

	UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
	{
		auto unityDevice = unityInterfaces->Get<IUnityGraphicsD3D11>()->GetDevice();
		g_unityDevice.attach(unityDevice);

		auto dxgiDevice = g_unityDevice.as<IDXGIDevice>();
		g_device = CreateDirect3DDevice(dxgiDevice.get());
		unityDevice->GetImmediateContext(&g_unityContext);
	}

	UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API UnityPluginUnload()
	{
	}

	UNITY_INTERFACE_EXPORT bool UNITY_INTERFACE_API IsSupported()
	{
		return winrt::Windows::Graphics::Capture::GraphicsCaptureSession::IsSupported();
	}

	UNITY_INTERFACE_EXPORT void* UNITY_INTERFACE_API CreateCaptureFromWindow(HWND hWnd)
	{
		auto isCaptureSupported = winrt::GraphicsCaptureSession::IsSupported();
		if (!isCaptureSupported)
		{
			MessageBoxW(nullptr,
				L"Screen capture is not supported on this device!",
				L"",
				MB_OK | MB_ICONERROR);
			return nullptr;
		}

		try
		{
			auto item = CreateCaptureItemForWindow(hWnd);
			return CreateCapture(item);
		}
		catch (winrt::hresult_error)
		{
			return nullptr;
		}
		return nullptr;
	}

	UNITY_INTERFACE_EXPORT void* UNITY_INTERFACE_API CreateCaptureFromMonitor(HMONITOR hMon)
	{
		auto isCaptureSupported = winrt::GraphicsCaptureSession::IsSupported();
		if (!isCaptureSupported)
		{
			MessageBoxW(nullptr,
				L"Screen capture is not supported on this device!",
				L"",
				MB_OK | MB_ICONERROR);
			return nullptr;
		}

		try
		{
			auto item = CreateCaptureItemForMonitor(hMon);
			return CreateCapture(item);
		}
		catch (winrt::hresult_error)
		{
			return nullptr;
		}
		return nullptr;
	}

	UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API StartCapture(void* capturePtr)
	{
		auto capture = reinterpret_cast<Capture*>(capturePtr);
		capture->StartCapture();
	}

	UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API CloseCapture(void* capturePtr)
	{
		auto capture = reinterpret_cast<Capture*>(capturePtr);
		capture->Close();
	}

	UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API GetWidth(void* capturePtr)
	{
		auto capture = reinterpret_cast<Capture*>(capturePtr);
		return capture->GetWidth();
	}

	UNITY_INTERFACE_EXPORT int UNITY_INTERFACE_API GetHeight(void* capturePtr)
	{
		auto capture = reinterpret_cast<Capture*>(capturePtr);
		return capture->GetHeight();
	}

	UNITY_INTERFACE_EXPORT void UNITY_INTERFACE_API Render(void* capturePtr, void* renderTexturePtr)
	{
		auto capture = reinterpret_cast<Capture*>(capturePtr);
		auto renderTexture = reinterpret_cast<ID3D11Texture2D*>(renderTexturePtr);
		capture->Render(renderTexture);
	}
}