using UnityEngine;

namespace Dice.Interfaces
{
    public interface IPositionBounds
    {
        Vector3 ClampPosition(Vector3 position);
    }
}
