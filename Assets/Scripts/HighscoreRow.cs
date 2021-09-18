using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreRow : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _placement;
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private Color _highlightColor;

    private Image _background;
    private Color _defaultColor;
    
    private void Awake() {
        _background = GetComponent<Image>();
        _defaultColor = _background.color;
    }

    public void SetValues(long placement, string playerName, long score, bool isCurrentPlayer = false) {
        _placement.text = $"{placement}.";
        _playerName.text = playerName;
        _score.text = score.ToString();
        _background.color = isCurrentPlayer ? _highlightColor : _defaultColor;
    }
    
}
