using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public class Feel
{
    [DllImport("libfeelc")]
    static extern IntPtr FEEL_CreateNewWithDevice(IntPtr device);
    [DllImport("libfeelc")]
    static extern IntPtr FEEL_CreateWithSerialDevice();
    [DllImport("libfeelc")]
    static extern IntPtr FEEL_CreateWithSimulatorDevice();
    [DllImport("libfeelc")]
    static extern void FEEL_Destroy(IntPtr feel);
    [DllImport("libfeelc")]
    static extern void FEEL_Connect(IntPtr feel, string deviceName);
    [DllImport("libfeelc")]
    static extern void FEEL_Disconnect(IntPtr feel);
    [DllImport("libfeelc")]
    static extern unsafe void FEEL_GetAvailableDevices(IntPtr feel, out FeelStringArrayHandle handle, out char** devices, out int deviceCount);
    [DllImport("libfeelc")]
    static extern void FEEL_ReleaseFeelStringArrayHandle(IntPtr handle);
    [DllImport("libfeelc")]
    static extern void FEEL_StartNormalization(IntPtr feel);
    [DllImport("libfeelc")]
    static extern void FEEL_BeginSession(IntPtr feel);
    [DllImport("libfeelc")]
    static extern void FEEL_EndSession(IntPtr feel);
    [DllImport("libfeelc")]
    static extern void FEEL_SetFingerAngle(IntPtr feel, int finger, float angle, int force);
    [DllImport("libfeelc")]
    static extern void FEEL_ReleaseFinger(IntPtr feel, int finger);
    [DllImport("libfeelc")]
    static extern float FEEL_GetFingerAngle(IntPtr feel, int finger);
    [DllImport("libfeelc")]
    static extern int FEEL_GetStatus(IntPtr feel);
    [DllImport("libfeelc")]
    static extern void FEEL_ParseMessages(IntPtr feel);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DebugLogCallback(string log);
    [DllImport("libfeelc")]
    static extern void FEEL_SetDebugLogCallback(IntPtr feel, DebugLogCallback callback);
    
    readonly IntPtr feelPtr;
    DebugLogCallback _debugLogCallback;
    static DebugLogCallback _currentDebugLogCallback;

    public Feel()
    {
        feelPtr = FEEL_CreateWithSimulatorDevice();
        _debugLogCallback = (s) =>{ Debug.Log("FEEL: " + s); };
        _currentDebugLogCallback = _debugLogCallback;
        FEEL_SetDebugLogCallback(feelPtr, CallDebugLogCallback);
    }

    public void Destroy()
    {
        FEEL_Destroy(feelPtr);
        if (_currentDebugLogCallback == _debugLogCallback)
        {
            _currentDebugLogCallback = null;
        }
    }
    
    public void Connect(string deviceName)
    {
        FEEL_Connect(feelPtr, deviceName);
    }

    public void Disconnect()
    {
        FEEL_Disconnect(feelPtr);
    }

    public string[] GetAvailableDevices()
    {
        string[] devices;
        unsafe
        {
            char** deviceStrs;
            int deviceCount;
            FeelStringArrayHandle handle;
            FEEL_GetAvailableDevices(feelPtr, out handle, out deviceStrs, out deviceCount);
            devices = new string[deviceCount];
            Debug.Log(deviceCount);
            for (int i = 0; i < deviceCount; i++)
            {
                devices[i] = new string(deviceStrs[i]);
                Debug.Log(*deviceStrs[i]);
                Debug.Log(new string(deviceStrs[i]));
                Debug.Log(devices[i]);
            }
            handle.Dispose();
        }

        return devices;
    }

    public void StartNormalization()
    {
        FEEL_StartNormalization(feelPtr);
    }

    public void BeginSession()
    {
        FEEL_BeginSession(feelPtr);
    }

    public void EndSession()
    {
        FEEL_EndSession(feelPtr);
    }

    public void SetFingerAngle(FeelFinger finger, float angle, int force)
    {
        FEEL_SetFingerAngle(feelPtr, (int) finger, angle, force);
    }

    public float GetFingerAngle(FeelFinger finger)
    {
        return FEEL_GetFingerAngle(feelPtr, (int) finger);
    }

    public void SetDebugLogCallback(DebugLogCallback callback)
    {
        _debugLogCallback = callback;
    }

    // Static function which calls callback, to have a static function Pointer you can pass
    // to the native world
    static void CallDebugLogCallback(string s)
    {
       _currentDebugLogCallback.Invoke(s);
    }

    public int GetStatus() //TODO: replace int with enum
    {
        return FEEL_GetStatus(feelPtr);
    }

    public void ParseMessages()
    {
        _currentDebugLogCallback = _debugLogCallback;
        FEEL_ParseMessages(feelPtr);
    }

    class FeelStringArrayHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public FeelStringArrayHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            FEEL_ReleaseFeelStringArrayHandle(handle);
            return true;
        }
    }
}
