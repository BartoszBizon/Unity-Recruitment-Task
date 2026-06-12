using Dice.Faces;
using Dice.Interfaces;
using UnityEngine;

namespace Dice
{
    public class DiceRoot : MonoBehaviour
    {
        [SerializeField] private DiceFaces _diceFaces;
        [SerializeField] private DicePhysicsController _dicePhysicsController;

        public IDiceFaceProvider FaceProvider => _diceFaces;
        public IDiceDragTarget DragTarget => _dicePhysicsController;
    }
}