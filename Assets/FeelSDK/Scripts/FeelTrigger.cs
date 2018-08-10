using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FeelTrigger : MonoBehaviour
{
    public List<Collider> colliders { get; private set; }
    public SphereCollider self { get; private set; }

    void Awake()
    {
        colliders = new List<Collider>();
        self = GetComponent<SphereCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        colliders.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }
}
