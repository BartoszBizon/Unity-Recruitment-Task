using NaughtyAttributes;
using UnityEngine;

namespace Dice.Roll
{
    [CreateAssetMenu(fileName = "Dice Roll Settings", menuName = "Dice/Dice Roll Settings")]
    public sealed class DiceRollSettings : ScriptableObject
    {
        [Header("Drag")]
        [SerializeField] private float _dragHeight = 2f;
        [SerializeField] private float _dragFollowSmoothTime = 0.05f;
        [SerializeField] private float _maxDragFollowSpeed = 500f;

        [Header("Drag Rotation")]
        [SerializeField] private float _dragRotationImpulse = 35f;
        [SerializeField] private float _dragRotationInertiaDamping = 1.25f;
        [SerializeField] private float _maxDragAngularSpeed = 900f;

        [Header("Throw")]
        [SerializeField] private float _throwVelocityMultiplier = 1.5f;
        [SerializeField] private float _minimumThrowVelocity = 2.5f;


        [Header("Physics")]
        [SerializeField] private float _gravityMultiplier = 2.5f;
        [SerializeField] private float _fallThreshold = -5f;

        [Header("Stop Conditions")]
        [SerializeField] private float _stoppedLinearVelocityThreshold = 0.05f;
        [SerializeField] private float _stoppedAngularVelocityThreshold = 0.1f;
        [SerializeField] private float _stoppedDuration = 0.5f;

        [Header("Return To Table")]
        [SerializeField] private float _returnToTableSmoothTime = 0.15f;

        [Header("Auto Roll")]
        [SerializeField, MinMaxSlider(0f, 50f)]
        private Vector2 _autoRollHorizontalSpeedRange = new(3.5f, 6.5f);

        [SerializeField, MinMaxSlider(0f, 20f)]
        private Vector2 _autoRollUpwardVelocityRange = new(3.5f, 5f);

        [SerializeField, MinMaxSlider(0f, 100f)]
        private Vector2 _autoRollAngularSpeedRange = new(8f, 16f);

        public float DragHeight => _dragHeight;
        public float ThrowVelocityMultiplier => _throwVelocityMultiplier;
        public float MinimumThrowVelocity => _minimumThrowVelocity;
        public float DragRotationImpulse => _dragRotationImpulse;
        public float DragRotationInertiaDamping => _dragRotationInertiaDamping;
        public float MaxDragAngularSpeed => _maxDragAngularSpeed;
        public float MaxDragFollowSpeed => _maxDragFollowSpeed;
        public float DragFollowSmoothTime => _dragFollowSmoothTime;
        public float StoppedAngularVelocityThreshold => _stoppedAngularVelocityThreshold;
        public float StoppedLinearVelocityThreshold => _stoppedLinearVelocityThreshold;
        public float StoppedDuration => _stoppedDuration;
        public float GravityMultiplier => _gravityMultiplier;
        public float FallThreshold => _fallThreshold;
        public float ReturnToTableSmoothTime => _returnToTableSmoothTime;
        public Vector2 AutoRollHorizontalSpeedRange => _autoRollHorizontalSpeedRange;
        public Vector2 AutoRollUpwardVelocityRange => _autoRollUpwardVelocityRange;
        public Vector2 AutoRollAngularSpeedRange => _autoRollAngularSpeedRange;
    }
}