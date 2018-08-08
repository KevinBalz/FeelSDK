using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FeelAngleTester : MonoBehaviour
{
    public FeelUnity feel;
    public float midAngle = 60;
    public float releaseTolerance = 5;

    Dictionary<FeelFinger, List<float>> lastPositions;
    Dictionary<FeelFinger, bool> wasBelow;

    IEnumerator Start()
    {
        feel.device.StartNormalization();
        yield return new WaitWhile(() => feel.device.GetStatus() == FeelStatus.Normalization);
        feel.device.BeginSession();
        yield return null;
        lastPositions = new Dictionary<FeelFinger, List<float>>();
        wasBelow = new Dictionary<FeelFinger, bool>();
        foreach (FeelFinger finger in Enum.GetValues(typeof(FeelFinger)))
        {
            lastPositions[finger] = Enumerable.Repeat(feel.device.GetFingerAngle(finger), 10).ToList();
            wasBelow[finger] = false;
        }
    }

    void Update()
    {
        if (feel.device.GetStatus() != FeelStatus.Active) return;

        foreach (FeelFinger finger in Enum.GetValues(typeof(FeelFinger)))
        {
            float angle = feel.device.GetFingerAngle(finger);
            float velocity = (angle - lastPositions[finger][0]) * 10 * Time.deltaTime;

            if (wasBelow[finger] && angle > midAngle - releaseTolerance && angle + releaseTolerance < midAngle && velocity > 0)
            {
                feel.device.ReleaseFinger(finger);
            }
            else if (angle < midAngle - 10 && velocity < -1)
            {
                feel.device.SetFingerAngle(finger, 180, 99);
                wasBelow[finger] = true;
            }
            else if (angle < midAngle && Mathf.Abs(velocity) > 0.5f)
            {
                feel.device.SetFingerAngle(finger, midAngle + (midAngle - angle), 40);
                wasBelow[finger] = true;
            }
            else
            {
                feel.device.ReleaseFinger(finger);
                wasBelow[finger] = angle < midAngle;
            }

            lastPositions[finger].RemoveAt(0);
            lastPositions[finger].Add(angle);
        }
    }
}
