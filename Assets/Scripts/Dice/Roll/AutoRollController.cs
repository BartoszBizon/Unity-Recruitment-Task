using Dice.Interfaces;
using UnityEngine;

namespace Dice.Roll
{
    public class AutoRollController : MonoBehaviour
    {
        [SerializeField] private RollEventsChannel _rollEventsChannel;
        [SerializeField] private DiceRollSettings _rollSettings;

        private IDiceDragTarget _diceDragTarget;

        public void Initialize(IDiceDragTarget diceDragTarget)
        {
            _diceDragTarget = diceDragTarget;
        }

        private void OnEnable()
        {
            _rollEventsChannel.OnAutoRollRequested += HandleAutoRollRequested;
        }

        private void OnDisable()
        {
            _rollEventsChannel.OnAutoRollRequested -= HandleAutoRollRequested;
        }

        private void HandleAutoRollRequested()
        {
            var linearVelocity = GetRandomLinearVelocity();
            var angularVelocity = GetRandomAngularVelocity();
            _diceDragTarget.TryAutoRoll(linearVelocity, angularVelocity);
        }

        private Vector3 GetRandomLinearVelocity()
        {
            var horizontalDirection = Random.insideUnitCircle.normalized;

            if (horizontalDirection.sqrMagnitude <= 0.001f)
                horizontalDirection = Vector2.right;

            var autoRollHorizontalSpeedRange = _rollSettings.AutoRollHorizontalSpeedRange;
            var autoRollUpwardVelocityRange = _rollSettings.AutoRollUpwardVelocityRange;
            var horizontalSpeed = Random.Range(autoRollHorizontalSpeedRange.x, autoRollHorizontalSpeedRange.y);
            var upwardVelocity = Random.Range(autoRollUpwardVelocityRange.x, autoRollUpwardVelocityRange.y);
            return new Vector3(horizontalDirection.x * horizontalSpeed, upwardVelocity, horizontalDirection.y * horizontalSpeed);
        }

        private Vector3 GetRandomAngularVelocity()
        { 
            // Reduce Y-axis influence to favour rolling over spinning in place.
            var angularDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), Random.Range(-1f, 1f));
            var autoRollAngularSpeedRange = _rollSettings.AutoRollAngularSpeedRange;
            var angularSpeed = Random.Range(autoRollAngularSpeedRange.x, autoRollAngularSpeedRange.y);
            return angularDirection.normalized * angularSpeed;
        }
    }
}