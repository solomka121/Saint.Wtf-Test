using UnityEngine;

public interface ICollectable
{
    ResourceType type { get;}
    float size { get;}
    float takeTime { get;}
    
    bool canBeTaken { get;}
}
