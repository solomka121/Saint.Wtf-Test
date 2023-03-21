using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Storage : MonoBehaviour
{
    public ResourceType storageType;
    [SerializeField] private bool _canTake = false;
    [SerializeField] private bool _canGive = true;
    [SerializeField] private float _delayBetweenMovingItems = 0.1f;
    private Coroutine _movingItemsCoroutine;

    [SerializeField] private Transform _storagePoint;
    [SerializeField] private Collider _trigger;

    [Header("Sizes")] [SerializeField] private List<Resource> _resourcesInStorage;

    [SerializeField] private int _width;
    [SerializeField] private int _lenght;

    [SerializeField] private int _maxCount;
    [SerializeField] private float _offset;

    private Vector3 centerOffset;
    private Vector3 size;

    private void OnValidate()
    {
        CalculateCenter();
    }

    private void Start()
    {
        CalculateCenter();
    }

    private void CalculateCenter()
    {
        size = new Vector3(_width * _offset, Mathf.CeilToInt(_maxCount / (_width * _lenght)), _lenght * _offset);
        centerOffset = - new Vector3(_width * _offset / 2f , 0, _lenght * _offset / 2f); // Minus Half Box Size
        centerOffset += new Vector3(_offset / 2 , 0 , _offset / 2); // Add Half Offset
    }

    public Vector3 GetLocalPositionForNewItem()
    {
        Vector3 point;

        point.x = _resourcesInStorage.Count % _width * _offset;
        point.z = _resourcesInStorage.Count / _width % _lenght * _offset;

        point.y = Mathf.FloorToInt(_resourcesInStorage.Count / (_width * _lenght)) * _offset;

        return point + centerOffset;
    }

    private IEnumerator TakeResourcesFromPLayer(Inventory inventory)
    {
        while (inventory.HasResource(storageType) && HasSpace())
        {
            yield return new WaitForSeconds(_delayBetweenMovingItems);
            inventory.GetResource(storageType, out Resource resource);
            TakeResource(resource);
        }
    }

    public void TakeResource(Resource resource)
    {
        StartCoroutine(TakingResource(resource, GetLocalPositionForNewItem()));
    }

    private IEnumerator TakingResource(Resource resource, Vector3 targetLocalPosition)
    {
        _resourcesInStorage.Add(resource);
        resource.transform.parent = _storagePoint;

        Vector3 startPosition = resource.transform.localPosition;
        Quaternion startRotation = resource.transform.localRotation;

        for (float progress = 0; progress <= 1; progress += Time.deltaTime * (1 / resource.takeTime))
        {
            resource.transform.localPosition = Vector3.Lerp(startPosition, targetLocalPosition, progress);
            resource.transform.localRotation = Quaternion.Lerp(startRotation, Quaternion.identity, progress);
            yield return null;
        }

        resource.transform.localPosition = targetLocalPosition;
        resource.transform.localRotation = Quaternion.identity;
    }

    private void PlaceResource(Resource resource, Vector3 point)
    {
        
    }

    public bool HasSpace()
    {
        return _resourcesInStorage.Count < _maxCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Inventory>(out Inventory inventory))
        {
            if (_movingItemsCoroutine == null)
            {
                _movingItemsCoroutine = StartCoroutine(TakeResourcesFromPLayer(inventory));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Inventory>(out Inventory inventory))
        {
            if (_movingItemsCoroutine != null)
            {
                StopCoroutine(_movingItemsCoroutine);
                _movingItemsCoroutine = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, size.y / 2, 0), size);

        Gizmos.color = Color.red;
        Vector3 startCountPoint = transform.position + centerOffset;
        Gizmos.DrawWireCube(startCountPoint, new Vector3(_offset, _offset, _offset));
    }
}