using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour , ICollectable
{
    [SerializeField] private Collider _collider;
    
    [field:SerializeField] public ResourceType type { get; private set; }
    [field: SerializeField] public float size { get; private set; } = 0.4f;
    [field: SerializeField] public float takeTime { get; private set; } = 1;

    public bool canBeTaken { get; set; } = true;
    
    

    public void SetInteractive(bool state)
    {
        _collider.enabled = state;
    }
}
