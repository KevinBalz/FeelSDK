using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feel_Glove : MonoBehaviour
{
    public string deviceName;
    public Feel device { get; private set; }

    void Awake()
    {
        device = new Feel();
        foreach (var d in device.GetAvailableDevices())
        {
            Debug.Log(d);
        }
        device.Connect(deviceName);
    }

    void FixedUpdate()
    {
        if (device == null) return;
        device.ParseMessages();
    }

    void OnDestroy()
    {
        if (device == null) return;
        device.EndSession();
        device.Disconnect();
        device.Destroy();
        device = null;
    }
}
