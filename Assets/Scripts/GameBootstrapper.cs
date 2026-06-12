using Dice;
using Dice.Faces;
using Dice.Roll;
using UnityEngine;

/// <summary>
/// Scene composition root responsible for wiring gameplay systems together.
///
/// A dedicated DI framework was intentionally not used. For a project of this
/// size, manual dependency injection through a single bootstrapper keeps the
/// architecture simple, explicit and easy to debug.
///
/// In a larger project, solutions such as Zenject could be introduced
/// For this task, that would be unnecessary overengineering.
/// </summary>
public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private DiceRoot _diceRoot;
    [SerializeField] private AutoRollController _autoRollController;
    [SerializeField] private DragRollInput _dragRollInput;
    [SerializeField] private TableBounds _tableBounds;
    [SerializeField] private RollSession _rollSession;


    private void Awake()
    {
        _dragRollInput.Initialize(Camera.main, _diceRoot.DragTarget, _tableBounds);
        _rollSession.Initialize(_diceRoot.FaceProvider);
        _autoRollController.Initialize(_diceRoot.DragTarget);
    }
}