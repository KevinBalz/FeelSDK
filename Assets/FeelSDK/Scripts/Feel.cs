﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class Feel
{
    [DllImport("libfeelc")]
    static extern IntPtr FEEL_CreateNew();
    [DllImport("libfeelc")]
    static extern void FEEL_Destroy(IntPtr feel);
    [DllImport("libfeelc")]
    static extern void FEEL_Connect(IntPtr feel, string deviceName);
    [DllImport("libfeelc")]
    static extern void FEEL_BeginSession(IntPtr feel);
    [DllImport("libfeelc")]
    static extern void FEEL_EndSession(IntPtr feel);
    [DllImport("libfeelc")]
    static extern void FEEL_SubscribeForFingerUpdates(IntPtr feel, bool active);
    [DllImport("libfeelc")]
    static extern void FEEL_SetFingerAngle(IntPtr feel, int finger, float angle);
    [DllImport("libfeelc")]
    static extern float FEEL_GetFingerAngle(IntPtr feel, int finger);
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
        feelPtr = FEEL_CreateNew();
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
    
    public void BeginSession()
    {
        FEEL_BeginSession(feelPtr);
    }

    public void EndSession()
    {
        FEEL_EndSession(feelPtr);
    }

    public void SubscribeForFingerUpdates(bool active = true)
    {
        FEEL_SubscribeForFingerUpdates(feelPtr, active);
    }

    public void SetFingerAngle(FeelFinger finger, float angle)
    {
        FEEL_SetFingerAngle(feelPtr, (int) finger, angle);
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

    public void ParseMessages()
    {
        _currentDebugLogCallback = _debugLogCallback;
        FEEL_ParseMessages(feelPtr);
    }
}
