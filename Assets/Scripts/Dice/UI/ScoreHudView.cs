using Dice.Roll;
using TMPro;
using UnityEngine;

namespace Dice.UI
{
    public class ScoreHudView : MonoBehaviour
    {
        [SerializeField] private RollEventsChannel _rollEventsChannel;
        [SerializeField] private TextMeshProUGUI _resultValueText;
        [SerializeField] private TextMeshProUGUI _totalScoreValueText;

        private readonly string _onDragResultText = "?";

        private void OnEnable()
        {
            _rollEventsChannel.OnSessionDataUpdated += SetTexts;
            _rollEventsChannel.OnRollStarted += SetResultText;
        }

        private void OnDisable()
        {
            _rollEventsChannel.OnSessionDataUpdated -= SetTexts;
            _rollEventsChannel.OnRollStarted -= SetResultText;
        }

        private void SetTexts(int result, int totalScore)
        {
            _resultValueText.text = result.ToString();
            _totalScoreValueText.text = totalScore.ToString();
        }

        private void SetResultText()
        {
            _resultValueText.text = _onDragResultText;
        }
    }
}