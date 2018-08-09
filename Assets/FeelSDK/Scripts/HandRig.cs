using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HandRig : MonoBehaviour
{
    public List<JointData> joints;
    public Transform targetTransform;

    void Update()
    {
        DoIK();
    }

    void DoIK()
    {
        var lastJoint = joints.Last();
        Vector2 target = targetTransform.position;
        Vector2 end = lastJoint.transform.position + lastJoint.transform.TransformDirection(lastJoint.endPointOffset);
        Debug.Log(end);
        foreach (var joint in joints.AsEnumerable().Reverse())
        {
            Vector2 position = joint.transform.position;
            var toEnd = end - position;
            var toTarget = target - position;
            var endRot = Quaternion.FromToRotation(toEnd, toTarget);
            var rot = joint.transform.rotation * endRot;
            if (rot.eulerAngles.z >= joint.angleConstraint.x && rot.eulerAngles.z <= joint.angleConstraint.y)
            {
                joint.transform.rotation = rot;
            }
            end = lastJoint.transform.position + lastJoint.transform.TransformDirection(lastJoint.endPointOffset);
        }
    }

    class BoneData
    {
        public Vector2 pos;
        public float angle;
        public float cosAngle;
        public float sinAngle;
    }
}
