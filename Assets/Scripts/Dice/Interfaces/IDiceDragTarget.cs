using UnityEngine;

namespace Dice.Interfaces
{
    public interface IDiceDragTarget
    {
        Vector3 Position { get; }
        bool CanBeDragged { get; }
        void BeginDrag();
        void DragTo(Vector3 worldPosition);
        bool TryThrow();
        void TryAutoRoll(Vector3 linearVelocity, Vector3 angularVelocity);
    }
}