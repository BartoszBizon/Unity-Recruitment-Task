using System;
using UnityEngine;

namespace Dice.Roll
{
    [CreateAssetMenu(fileName = "Roll Events Channel", menuName = "Dice/Roll Events Channel")]
    public sealed class RollEventsChannel : ScriptableObject
    {
        public event Action OnDiceStopped;
        public event Action OnRollStarted;
        public event Action OnAutoRollRequested;

        public event Action<int, int> OnSessionDataUpdated;

        public void RaiseDiceStopped()
        {
            OnDiceStopped?.Invoke();
        }

        public void RaiseRollStarted()
        {
            OnRollStarted?.Invoke();
        }

        public void RaiseSessionDataUpdated(int score, int totalScore)
        {
            OnSessionDataUpdated?.Invoke(score, totalScore);
        }

        public void RaiseAutoRollRequest()
        {
            OnAutoRollRequested?.Invoke();
        }
    }
}