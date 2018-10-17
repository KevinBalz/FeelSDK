using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FeelUnity))]
public class HandRig : MonoBehaviour
{
    public List<FingerRig> fingers;
    FeelUnity feel;

    IEnumerator Start()
    {
        feel = GetComponent<FeelUnity>();
        feel.device.StartNormalization();
        yield return new WaitWhile(() => feel.device.GetStatus() == FeelStatus.Normalization);
        feel.device.BeginSession();
        yield return null;
        yield return null;
        foreach (var finger in fingers)
        {
            finger.feel = feel;
            finger.Activate();
        }
    }
}
