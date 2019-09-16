using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiplayerController : MonoBehaviour
{
    public Text nickText;
    public Text scoreText;
    public Text positionText;
    
    public GameObject entryPrefab;
    public Transform tableContent;
    public GameObject inputField;
    
    private PlayerController _playerController;
    private SceneController _sceneController;

    private List<PlayerInfo> _playerInfos = new List<PlayerInfo>();
    private List<GameObject> _shownPlayers = new List<GameObject>();


    private void Start()
    {
        _playerController = Utilities.FindPlayerController();
        _sceneController = Utilities.FindSceneController();

        if (_playerController.Nick == null || _playerController.Nick.Equals(""))
        {
            inputField.SetActive(true);
        }
        else
        {
            InvokeRepeating(nameof(GetPlayers), 0.0f, 0.5f);
        }
    }
    
    public void SetNick(InputField userInput)
    {
        _playerController.Nick = userInput.text;
        _playerController.SaveGame();
        inputField.SetActive(false);
        InvokeRepeating(nameof(GetPlayers), 0.0f, 0.5f);
    }

    public void GoBack()
    {
        _sceneController.GoBack();
    }
    
    private void SetTopText()
    {
        nickText.text = "Hello " + _playerController.Nick + "!";
        scoreText.text = "Your score: " + _playerController.Score;

        int position = _playerInfos.FindIndex(p => p.username.Equals(_playerController.Nick))+1;
        positionText.text = "You are " + position + " comparing to other players";
    }
    
    private void GetPlayers()
    {
        string uri = Settings.IP + "/api/players";
        StartCoroutine(GetRequest(uri));
    }
    
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                string response = "{\"playerInfos\":" + webRequest.downloadHandler.text + "}";
                _playerInfos = JsonUtility.FromJson<PlayersWrapper>(response).playerInfos;
            }

            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        ClearTable();
        
        _playerInfos.Sort((p1,p2)=>p2.points.CompareTo(p1.points));

        foreach (var player in _playerInfos)
        {
            GameObject newPlayer = Instantiate(entryPrefab, tableContent);
            newPlayer.transform.Find("Name").GetComponent<Text>().text = player.username;
            newPlayer.transform.Find("Score").GetComponent<Text>().text = player.points.ToString();
            if (player.username.Equals(_playerController.Nick))
            {
                newPlayer.transform.GetComponent<Image>().enabled = true;
            }
            
            _shownPlayers.Add(newPlayer);
        }
        SetTopText();
    }
    
    private void ClearTable()
    {
        foreach (var place in _shownPlayers)
        {
            Destroy(place);
        }
    }
}
