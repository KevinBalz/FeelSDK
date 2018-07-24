using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feel_Glove : MonoBehaviour
{
    public bool subscribeForFingerUpdates = true;
    public string deviceName;
    public Feel device { get; private set; }

    void Awake()
    {
        device = new Feel();
        device.Connect(deviceName);
    }

    void Start()
    {
        if (subscribeForFingerUpdates)
        {
            device.SubscribeForFingerUpdates();
        }
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
