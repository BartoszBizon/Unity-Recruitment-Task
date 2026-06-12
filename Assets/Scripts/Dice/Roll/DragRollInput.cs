using Dice.Interfaces;
using UnityEngine;

namespace Dice.Roll
{
    public sealed class DragRollInput : MonoBehaviour
    {
        [SerializeField] private LayerMask _diceLayer;

        private bool _isDragging;
        private Plane _dragPlane;
        private Vector3 _dragOffset;

        private IDiceDragTarget _diceDragTarget;
        private IPositionBounds _positionBounds;
        private Camera _camera;

        public void Initialize(Camera camera, IDiceDragTarget diceDragTarget, IPositionBounds positionBounds)
        {
            _camera = camera;
            _diceDragTarget = diceDragTarget;
            _positionBounds = positionBounds;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                TryBeginDrag();

            if (_isDragging && Input.GetMouseButton(0))
                UpdateDrag();

            if (_isDragging && Input.GetMouseButtonUp(0))
                EndDrag();
        }

        private void TryBeginDrag()
        {
            if (!_diceDragTarget.CanBeDragged)
                return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, _diceLayer))
                return;

            var dicePosition = _diceDragTarget.Position;
            _isDragging = true;
            _dragPlane = new Plane(Vector3.up, dicePosition);
            _dragOffset = dicePosition - hit.point;
            _diceDragTarget.BeginDrag();
        }

        private void UpdateDrag()
        {
            if (!_diceDragTarget.CanBeDragged)
                return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!_dragPlane.Raycast(ray, out float distance))
                return;

            var planePoint = ray.GetPoint(distance);
            var targetPosition = planePoint + _dragOffset;
            targetPosition = _positionBounds.ClampPosition(targetPosition);

            _diceDragTarget.DragTo(targetPosition);
        }

        private void EndDrag()
        {
            _isDragging = false;

            var wasThrown = _diceDragTarget.TryThrow();

            if (!wasThrown)
            {
                Debug.Log("Throw cancelled: velocity too low.");
            }
        }
    }
}