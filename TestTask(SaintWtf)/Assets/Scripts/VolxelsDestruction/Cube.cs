using DG.Tweening;
using UnityEngine;
using TMPro;

public class Cube : MonoBehaviour
{
    private bool _detouched;

    public int Id { get; set; }

    [ContextMenu("Detouch cube")]
    public void Detouch()
    {
        if (_detouched)
            return;

        _detouched = true;
        GetComponentInParent<Entity>().DetouchCube(this);
    }

    public void Destroy()
    {
        Detouch();

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        transform.DOScale(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }
}