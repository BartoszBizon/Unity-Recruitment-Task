using Dice.Interfaces;
using UnityEngine;

public class TableBounds : MonoBehaviour, IPositionBounds
{
    [SerializeField] private BoxCollider _boundsCollider;

    private Bounds Bounds => _boundsCollider.bounds;

    private void OnValidate()
    {
        if (_boundsCollider == null)
            _boundsCollider = GetComponent<BoxCollider>();
    }

    public Vector3 ClampPosition(Vector3 worldPosition)
    {
        Bounds bounds = Bounds;
        worldPosition.x = Mathf.Clamp(worldPosition.x, bounds.min.x, bounds.max.x);
        worldPosition.z = Mathf.Clamp(worldPosition.z, bounds.min.z, bounds.max.z);
        return worldPosition;
    }
}