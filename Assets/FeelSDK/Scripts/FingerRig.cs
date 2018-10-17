using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FingerRig : MonoBehaviour
{
    public FeelUnity feel { get; set; }
    public FeelFinger finger;
    public List<JointData> joints;
    public Transform effectorTransform;
    public Transform targetTransform;
    public Transform targetJoint;
    public Vector2 fingerStretchedPosition;
    public FeelTrigger collisionTrigger;
    
    bool initialized;
    float lastAngle;

    public void Activate()
    {
        lastAngle = feel.device.GetFingerAngle(finger);
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        var fingerAngle = feel.device.GetFingerAngle(finger);
        var angleDifference = fingerAngle - lastAngle;
        var angle = fingerAngle / 180 * (joints[0].angleConstraint.y - joints[0].angleConstraint.x) + joints[0].angleConstraint.x;
        joints[0].transform.localEulerAngles = new Vector3(0, 0, angle);

        if (Input.GetButtonUp("Jump"))
        {
            targetJoint.localEulerAngles = Vector3.zero;
            targetTransform.localPosition = new Vector3(fingerStretchedPosition.x, fingerStretchedPosition.y, 0);
        }
        else
        {
            var targetAngle = targetJoint.localEulerAngles;
            targetAngle.z += angleDifference / 180 * 70;
            targetJoint.localEulerAngles = targetAngle;
        }

        var constraint = joints[0].angleConstraint;
        joints[0].angleConstraint = Vector2.zero;
        DoCCD(targetTransform.position, 10);
        joints[0].angleConstraint = constraint;

        var releaseFinger = true;
        if (collisionTrigger.colliders.Count > 0)
        {
            var other = collisionTrigger.colliders[0];
            Vector3 direction;
            float distance;
            var overlapped = Physics.ComputePenetration(
                    collisionTrigger.self, collisionTrigger.transform.position, collisionTrigger.transform.rotation,
                    other, other.transform.position, other.transform.rotation,
                    out direction, out distance);
            if (overlapped)
            {
                Vector3 targetPos = targetTransform.position + direction * distance;
                Quaternion[] endRotations;
                DoCCD(targetPos, 10, out endRotations);
                float newFingerAngle = (endRotations[0].eulerAngles.z - joints[0].angleConstraint.x) / (joints[0].angleConstraint.y - joints[0].angleConstraint.x) * 180;
                newFingerAngle = (fingerAngle + newFingerAngle) / 2;
                feel.device.SetFingerAngle(finger, newFingerAngle, 50);
                releaseFinger = false;
            }
        }

        if (releaseFinger)
        {
            feel.device.ReleaseFinger(finger);
        }

        lastAngle = fingerAngle;
    }

    static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (point - pivot) + pivot;
    }

    static Vector3 UnRotatePosition(Transform child, Transform parent)
    {
        return UnRotatePosition(child.position, parent);
    }

    static Vector3 UnRotatePosition(Vector3 position, Transform parent)
    {
        return RotateAroundPivot(position, parent.position, Quaternion.Inverse(parent.rotation));
    }

    void DoCCD(Vector3 target, int times = 1)
    {
        var lastJoint = joints.Last();
        Plane plane = new Plane(joints[0].transform.position, joints[1].transform.position, effectorTransform.position);
        Vector3 targetPosition = plane.ClosestPointOnPlane(target);
        for (int iteration = 0; iteration < times; iteration++)
        {
            foreach (var joint in joints.AsEnumerable().Reverse())
            {
                Vector2 position = UnRotatePosition(joint.transform, transform);
                Vector2 toEnd = (Vector2) UnRotatePosition(effectorTransform, transform) - position;
                Vector2 toTarget = (Vector2) UnRotatePosition(targetPosition, transform) - position;
                var rot = Quaternion.FromToRotation(toEnd, toTarget);

                joint.transform.localRotation *= rot;

                var euler = RoundAngle(joint.transform.localEulerAngles.z);
                if (euler < joint.angleConstraint.x || euler > joint.angleConstraint.y)
                {
                    joint.transform.localRotation *= Quaternion.Inverse(rot);
                }
            }
        }      
    }

    void DoCCD(Vector2 target, int times, out Quaternion[] endRotations)
    {
        var savedRotations = joints.Select(j => j.transform.rotation).ToArray();
        DoCCD(target, times);
        endRotations = joints.Select(j => j.transform.rotation).ToArray();
        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].transform.rotation = savedRotations[i];
        }
    }

    float RoundAngle(float angle)
    {
        angle = angle % 360;
        if (angle < -180)
        {
            angle += 360;
        }
        else if (angle > 180)
        {
            angle -= 360;
        }

        return angle;
    }
}
