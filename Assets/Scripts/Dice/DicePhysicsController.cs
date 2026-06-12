using System.Collections;
using Dice.Interfaces;
using Dice.Roll;
using UnityEngine;

namespace Dice
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class DicePhysicsController : MonoBehaviour, IDiceDragTarget
    {
        [SerializeField] private RollEventsChannel _diceRollEvents;
        [SerializeField] private DiceRollSettings _settings;
        [SerializeField] private Rigidbody _rigidbody;

        private bool _isThrown;
        private bool _isReturningToTable;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _dragVelocity;
        private Vector3 _dragAngularVelocity;
        private Vector3 _lastDragVelocity;
        private Vector3 _dragFollowVelocity;
        private float _stoppedLinearVelocityThresholdSqr;
        private float _stoppedAngularVelocityThresholdSqr;
        private float _stoppedTimer;

        private bool IsOnStartPosition => Vector3.Distance(transform.position, _startPosition) < 0.5f;
        public Vector3 Position => transform.position;
        public bool CanBeDragged => !_isThrown && !_isReturningToTable;

        private void OnValidate()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            _startPosition = transform.position;
            _startRotation = transform.rotation;

            var stoppedLinearVelocityThreshold = _settings.StoppedLinearVelocityThreshold;
            var stoppedAngularVelocityThreshold = _settings.StoppedAngularVelocityThreshold;
            _stoppedLinearVelocityThresholdSqr = stoppedLinearVelocityThreshold * stoppedLinearVelocityThreshold;
            _stoppedAngularVelocityThresholdSqr = stoppedAngularVelocityThreshold * stoppedAngularVelocityThreshold;
        }

        private void FixedUpdate()
        {
            if (!_isThrown)
                return;

            _rigidbody.AddForce(Physics.gravity * _settings.GravityMultiplier, ForceMode.Acceleration);
            CheckIfDiceStop();
            CheckIfDiceFellOffTable();
        }

        public void BeginDrag()
        {
            if (_isThrown)
                return;

            _diceRollEvents?.RaiseRollStarted();
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _dragAngularVelocity = Vector3.zero;
            _lastDragVelocity = Vector3.zero;
            _dragVelocity = Vector3.zero;
            _dragFollowVelocity = Vector3.zero;
        }

        public void DragTo(Vector3 worldPosition)
        {
            var targetPosition = new Vector3(worldPosition.x, _startPosition.y + _settings.DragHeight, worldPosition.z);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref _dragFollowVelocity,
                _settings.DragFollowSmoothTime,
                _settings.MaxDragFollowSpeed);

            _dragVelocity = _dragFollowVelocity;

            UpdateDragRotation(_dragVelocity);
        }

        public bool TryThrow()
        {
            _isThrown = true;
            var hasEnoughLinearVelocity = _dragVelocity.magnitude >= _settings.MinimumThrowVelocity;
            if (!hasEnoughLinearVelocity)
            {
                ReturnToTable();
                return false;
            }

            _rigidbody.isKinematic = false;
            _rigidbody.velocity = _dragVelocity * _settings.ThrowVelocityMultiplier;
            _rigidbody.angularVelocity = _dragAngularVelocity * Mathf.Deg2Rad;

            return true;
        }

        private void ReturnToTable()
        {
            StartCoroutine(ReturnToTableCoroutine());
        }

        private IEnumerator ReturnToTableCoroutine()
        {
            _isThrown = false;
            _isReturningToTable = true;
            _rigidbody.isKinematic = true;

            var velocity = Vector3.zero;

            while (!IsOnStartPosition)
            {
                var smoothTime = _settings.ReturnToTableSmoothTime;
                transform.position = Vector3.SmoothDamp(transform.position, _startPosition, ref velocity, smoothTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, _startRotation, Time.deltaTime / smoothTime);
                yield return null;
            }

            _rigidbody.isKinematic = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _isReturningToTable = false;
        }

        private void UpdateDragRotation(Vector3 dragVelocity)
        {
            var twistVelocity = Vector3.Cross(_lastDragVelocity, dragVelocity).y;
            var inputAngularImpulse = new Vector3(dragVelocity.z, twistVelocity, -dragVelocity.x) * _settings.DragRotationImpulse;

            _dragAngularVelocity += inputAngularImpulse * Time.deltaTime;
            _dragAngularVelocity = Vector3.ClampMagnitude(_dragAngularVelocity, _settings.MaxDragAngularSpeed);
            _dragAngularVelocity = Vector3.Lerp(_dragAngularVelocity, Vector3.zero, Time.deltaTime * _settings.DragRotationInertiaDamping);

            if (_dragAngularVelocity.sqrMagnitude >= 0.01f)
            {
                transform.Rotate(_dragAngularVelocity * Time.deltaTime, Space.World);
            }

            _lastDragVelocity = dragVelocity;
        }

        private void CheckIfDiceStop()
        {
            var isLinearVelocityLow = _rigidbody.velocity.sqrMagnitude < _stoppedLinearVelocityThresholdSqr;
            var isAngularVelocityLow = _rigidbody.angularVelocity.sqrMagnitude < _stoppedAngularVelocityThresholdSqr;

            if (isLinearVelocityLow && isAngularVelocityLow)
            {
                _stoppedTimer += Time.fixedDeltaTime;
            }
            else
            {
                _stoppedTimer = 0f;
            }

            if (_stoppedTimer >= _settings.StoppedDuration)
            {
                _isThrown = false;
                _stoppedTimer = 0f;
                _diceRollEvents.RaiseDiceStopped();
            }
        }

        public void TryAutoRoll(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            if (_isThrown || _isReturningToTable)
                return;

            StopAllCoroutines();
            StartCoroutine(AutoRollRoutine(linearVelocity, angularVelocity));
        }

        private IEnumerator AutoRollRoutine(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            if (!IsOnStartPosition)
                yield return ReturnToTableCoroutine();

            _diceRollEvents?.RaiseRollStarted();
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = linearVelocity;
            _rigidbody.angularVelocity = angularVelocity;
            _isThrown = true;
            _stoppedTimer = 0f;
        }
        


        private void CheckIfDiceFellOffTable()
        {
            if (transform.position.y > _settings.FallThreshold)
                return;

            ReturnToTable();
        }
    }
}