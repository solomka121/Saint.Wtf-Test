using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform _collectPoint;
    [SerializeField] private int _height;
    [SerializeField] private int _maxCount;
    [SerializeField] private float _offset;
    [SerializeField] private List<Resource> _resourcesInBag;

    public Vector3 GetLocalPositionForLastResource()
    {
        Vector3 offset;

        offset.x = 0;
        offset.y = _resourcesInBag.Count % _height * _offset;
        offset.z = -Mathf.FloorToInt(_resourcesInBag.Count / _height) * _offset;

        Debug.DrawLine(offset + _collectPoint.position, Vector3.zero);
        return offset;
    }

    public void TakeResource(Resource resource)
    {
        if(HasSpace() == false)
            return;
        
        StartCoroutine(TakingResource(resource, GetLocalPositionForLastResource()));
    }

    private IEnumerator TakingResource(Resource resource, Vector3 targetLocalPosition)
    {
        resource.SetInteractive(false);
        _resourcesInBag.Add(resource);

        Vector3 startPosition = resource.transform.position;
        Quaternion startRotation = resource.transform.rotation;

        for (float progress = 0; progress <= 1; progress += Time.deltaTime * (1 / resource.takeTime))
        {
            resource.transform.position = Vector3.Lerp(startPosition,
                _collectPoint.position + (Quaternion.Euler(new Vector3(0, _collectPoint.rotation.eulerAngles.y, 0)) *
                                          targetLocalPosition), progress);
            resource.transform.rotation = Quaternion.Lerp(startRotation, _collectPoint.rotation, progress);
            yield return null;
        }

        resource.transform.parent = _collectPoint;
        resource.transform.localPosition = targetLocalPosition;
        resource.transform.localRotation = Quaternion.identity;
    }

    public bool GetResource(ResourceType type , out Resource resource)
    {
        resource = null;

        if (HasResource(type, out Resource resourceInBag , out int index))
        {
            resource = resourceInBag;
            RemoveFromBag(index);
        }

        return false;
    }

    public bool HasResources()
    {
        return _resourcesInBag.Count > 0;
    }
    
    public bool HasSpace()
    {
        return _resourcesInBag.Count < _maxCount;
    }
    
    public bool HasResource(ResourceType type)
    {
        for (int i = _resourcesInBag.Count - 1; i >= 0; i--)
        {
            if (_resourcesInBag[i].type == type)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasResource(ResourceType type , out Resource resource , out int resourceIndex)
    {
        resource = null;
        resourceIndex = 0;
        
        for (int i = _resourcesInBag.Count - 1; i >= 0; i--)
        {
            if (_resourcesInBag[i].type == type)
            {
                resource = _resourcesInBag[i];
                resourceIndex = i;
                return true;
            }
        }

        return false;
    }

    public void RemoveFromBag(int index)
    {
        Resource resource = _resourcesInBag[index];
        _resourcesInBag.RemoveAt(index);
        //TODO events on resources give
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player Entered Trigger : " + other.name);

        if (other.TryGetComponent<Resource>(out Resource resource))
        {
            TakeResource(resource);
        }
    }
}