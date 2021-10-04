using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class HighscoreService {
    private string _appId;
    private string _appSecret;
    private string _apiUrl;

    public HighscoreService(string appId, string appSecret, string apiUrl) {
        _appId = appId;
        _appSecret = appSecret;
        _apiUrl = apiUrl;
    }

    public IEnumerator SubmitScore(string playerId, string playerName, long score, Action onSuccess,
        Action<UnityWebRequest.Result> onFailure, ScoreOrder order = ScoreOrder.DESC) {
        var checksum = CalculateChecksum(playerId, score);
        using (UnityWebRequest request =
            UnityWebRequest.Post(
                $"{_apiUrl}?appId={_appId}&playerId={playerId}&playerName={playerName}&score={score}&checksum={checksum}&order={order}",
                "")) {
            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return null;
            if (request.responseCode == 200) {
                onSuccess();
            }
            else {
                onFailure(request.result);
            }
        }
    }

    private string CalculateChecksum(string playerId, long score) {
        var bytes = System.Text.Encoding.UTF8.GetBytes(playerId + score + _appSecret);
        var checksum = new MD5CryptoServiceProvider().ComputeHash(bytes);
        return BitConverter.ToString(checksum).Replace("-", "");
    }

    public IEnumerator GetHighscores(string playerId, Action<HighscoreResults> onSuccess,
        Action<UnityWebRequest.Result> onFailure, ScoreOrder scoreOrder = ScoreOrder.DESC) {
        using (UnityWebRequest request = UnityWebRequest.Get(
            $"{_apiUrl}?appId={_appId}&playerId={playerId}&order={Enum.GetName(typeof(ScoreOrder), scoreOrder)}")
        ) {
            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return null;
            if (request.responseCode == 200) {
                byte[] result = request.downloadHandler.data;
                string resultJson = System.Text.Encoding.Default.GetString(result);
                HighscoreResults results = JsonUtility.FromJson<HighscoreResults>(resultJson);
                onSuccess(results);
            }
            else {
                onFailure(request.result);
            }
        }
    }
}

[Serializable]
public class HighscoreResults {
    public HighscoreResultItemDto currentPlayerScore;
    public List<HighscoreResultItemDto> topScores;
}

[Serializable]
public struct HighscoreResultItemDto {
    public long score;
    public string playerName;
    public string playerId;
    public long placement;
    public string scoreTime;
}

public enum ScoreOrder {
    ASC,
    DESC
}