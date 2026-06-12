using Dice.Faces;
using Dice.Interfaces;
using UnityEngine;

namespace Dice.Roll
{
    public class RollSession : MonoBehaviour
    {
        [SerializeField] private RollEventsChannel _rollEventsChannel;

        private int _lastRollResult;
        private int _totalScore;

        private IDiceFaceProvider _diceFaceProvider;

        public void Initialize(IDiceFaceProvider diceFaceProvider)
        {
            _diceFaceProvider = diceFaceProvider;
        }

        private void OnEnable()
        {
            _rollEventsChannel.OnDiceStopped += GetRollResult;
        }

        private void OnDisable()
        {
            _rollEventsChannel.OnDiceStopped -= GetRollResult;
        }

        private void GetRollResult()
        {
            var topDiceFace = _diceFaceProvider.GetTopDiceFace();
            if (topDiceFace == null)
                return;

            var result = topDiceFace.Value;
            UpdateSessionData(result);
        }

        private void UpdateSessionData(int result)
        {
            _lastRollResult = result;
            _totalScore += result;
            _rollEventsChannel.RaiseSessionDataUpdated(_lastRollResult, _totalScore);
        }
    }
}