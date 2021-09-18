using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscorePanel : MonoBehaviour {
    [SerializeField] private GameObject _loadingPlaceholder;
    [SerializeField] private GameObject _loadingErrorPlaceholder;
    
    private HighscoreRow[] _rows;


    private void Awake() {
        _rows = GetComponentsInChildren<HighscoreRow>();
        HideAll();
    }

    public void ShowLoadingMessage() {
        HideAll();
        _loadingPlaceholder.SetActive(true);
    }
    
    public void ShowLoadingErrorMessage() {
        HideAll();
        _loadingErrorPlaceholder.SetActive(true);
    }
    
    public void ShowResults(HighscoreResults results) {
        _loadingPlaceholder.SetActive(false);
        _loadingErrorPlaceholder.SetActive(false);
        foreach (var highscoreRow in _rows) {
            highscoreRow.gameObject.SetActive(false);
        }

        var topScores = results.topScores;
        var currentPlayerId = results.currentPlayerScore.playerId;
        for (int i = 0; i < results.topScores.Count; i++) {
            _rows[i].gameObject.SetActive(true);
            _rows[i].SetValues(topScores[i].placement, topScores[i].playerName, topScores[i].score, topScores[i].playerId == currentPlayerId);
        }

        //show current player score if it's out of top 10
        if (results.currentPlayerScore.placement > results.topScores.Count) {
            var lastRowIndex = results.topScores.Count;
            _rows[lastRowIndex].gameObject.SetActive(true);
            _rows[lastRowIndex].SetValues(
                results.currentPlayerScore.placement, 
                results.currentPlayerScore.playerName,
                results.currentPlayerScore.score, true);
        }
    }

    private void HideAll() {
        foreach (var highscoreRow in _rows) {
            highscoreRow.gameObject.SetActive(false);
        }
        _loadingPlaceholder.SetActive(false);
        _loadingErrorPlaceholder.SetActive(false);
    }
}
