using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

/**
 * Example class to show how the HighscoreService can be used
 */
public class TestPanel : MonoBehaviour {

    [SerializeField] private string _apiUrl;
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private TMP_InputField _playerId;
    [SerializeField] private TMP_InputField _appId;
    [SerializeField] private TMP_InputField _appSecret;
    [SerializeField] private TMP_InputField _score;
    [SerializeField] private TextMeshProUGUI _submitResult;
    [SerializeField] private UnityEngine.UI.Toggle _orderToggle;
    
    [SerializeField] private TextAsset _appSecretFile;
    [SerializeField] private HighscorePanel _highscorePanel; 
    
    private bool _initialized;
    
    private void Awake() {
        
        /**
         * the appSecret can be kept in a separate one-line file which is igored by version control system
         * so it's not accidentaly exposed to the public
         */
        if (_appSecretFile != null) {
            _appSecret.text = _appSecretFile.text;
        }
    }

    void Start() {
        InitializeTestFields();
        _initialized = true;
    }
    
    /**
     * here the HighscoreService is created for every call to get values from test fields
     * but in target solution it could be created only once
     */
    private HighscoreService GetService() {
        return new HighscoreService(_appId.text, _appSecret.text, _apiUrl);
    }
    
    /**
     * Used to simulate service errors
     */
    private HighscoreService GetBrokenService() {
        return new HighscoreService(_appId.text, _appSecret.text, "http://localost/dummy-url");
    }
    

    public void SubmitScore() {
        StartCoroutine(
            GetService().SubmitScore(_playerId.text, _playerName.text, long.Parse(_score.text), OnSubmitSuccess, OnSubmitFailure));
    }

    public void GetHighscores() {
        _highscorePanel.ShowLoadingMessage();
        StartCoroutine(GetService().GetHighscores(_playerId.text, OnGetHighscoreSuccess, OnGetHighscoreFailure, GetOrder()));
    }

    public void SimulateSubmitError() {
        StartCoroutine(GetBrokenService().SubmitScore(_playerId.text, _playerName.text, long.Parse(_score.text), OnSubmitSuccess, OnSubmitFailure));
    }
    
    public void SimulateGetHighscoresError() {
        _highscorePanel.ShowLoadingMessage();
        StartCoroutine(GetBrokenService().GetHighscores(_playerId.text, OnGetHighscoreSuccess, OnGetHighscoreFailure));
    }
    
    private void OnSubmitSuccess() {
        Debug.Log($"submit successful");
        _submitResult.text = "Submit succesfull";
        Invoke(nameof(ClearSubmitResult), 1f);
    }
    
    
    private void OnSubmitFailure(UnityWebRequest.Result result) {
        Debug.Log($"submit failed - {result}");
        _submitResult.text = $"Submit failed: {result}";
        Invoke(nameof(ClearSubmitResult), 2f);
    }
    
    private void OnGetHighscoreSuccess(HighscoreResults results) {
        Debug.Log($"get highscores successful");
        _highscorePanel.ShowResults(results);
    }
    
    
    private void OnGetHighscoreFailure(UnityWebRequest.Result result) {
        Debug.Log($"get highscore failed - {result}");
        _highscorePanel.ShowLoadingErrorMessage();
    }

    private void ClearSubmitResult() {
        _submitResult.text = "";
    }

    private ScoreOrder GetOrder() {
        return _orderToggle.isOn ? ScoreOrder.ASC : ScoreOrder.DESC;
    }
    /**
     * Called whenever test fields are updated
     */
    public void SaveChanges() {
        if (!_initialized) {
            return;
        }
        PlayerPrefs.SetString("playerName", _playerName.text);
        PlayerPrefs.SetString("playerId", _playerId.text);
        PlayerPrefs.SetString("appId", _appId.text);
        PlayerPrefs.SetString("appSecret", _appSecret.text);
        PlayerPrefs.SetString("score", _score.text);
        PlayerPrefs.Save();
    }
    
    private void InitializeTestFields() {
        if (!PlayerPrefs.HasKey("playerName")) {
            PlayerPrefs.SetString("playerName", $"player-{GUID.Generate()}");
        }

        if (!PlayerPrefs.HasKey("playerId")) {
            PlayerPrefs.SetString("playerId", $"playerId-{GUID.Generate()}");
        }

        _playerName.text = PlayerPrefs.GetString("playerName");
        _playerId.text = PlayerPrefs.GetString("playerId");

        if (PlayerPrefs.HasKey("appId")) {
            _appId.text = PlayerPrefs.GetString("appId");
        }

        if (PlayerPrefs.HasKey("appSecret")) {
            _appSecret.text = PlayerPrefs.GetString("appSecret");
        }

        if (PlayerPrefs.HasKey("score")) {
            _score.text = PlayerPrefs.GetString("score");
        }
    }

}
