using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using ResourcesObjects;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ActionGameController : MonoBehaviour
{
    public GameObject[] hazards;
    public Vector3 hazardSpawnValues;
    public int hazardsPerWave;
    
    public GameObject[] neutrals;
    public Vector3 neutralSpawnValues;
    public int neutralsPerWave;
    
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public Text restartText;
    public Text gameOverText;

    private bool _gameOver = false;

    private PlayerController _playerController;
    private SceneController _sceneController;
    private Place _rewardPlace;
    private void Start()
    {
        _playerController = Utilities.FindPlayerController();
        _sceneController = Utilities.FindSceneController();
        _rewardPlace = _playerController.Places
            .Find(p => p.id == _playerController.CurrentPlayedPlaceId);
        
        restartText.text = "";
        gameOverText.text = "";
        StartCoroutine(SpawnWaves());
    }

    private void Update()
    {
        if (_gameOver && Input.GetButton("Fire1"))
        {
            StartCoroutine(ReloadScene());
        }
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        
        int wavesToBeat;
        switch (_rewardPlace.gameDifficulty)
        {
            case "Easy":
                wavesToBeat = 2;
                break;
            case "Medium":
                wavesToBeat = 4;
                break;
            case "Hard":
                wavesToBeat = 6;
                break;
            default:
                throw new NotImplementedException();
        }
        int currentWave = 1;
        
        Quaternion spawnRotation = Quaternion.identity;
        
        while (currentWave <= wavesToBeat)
        {
            int hazardToSpawn = hazardsPerWave;
            int neutralToSpawn = neutralsPerWave;
            while (hazardToSpawn > 0)
            {
                if (Random.Range(0, hazardToSpawn + neutralToSpawn) <= hazardToSpawn)
                {
                    hazardToSpawn--;
                    
                    GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                    Vector3 spawnPosition = new Vector3(Random.Range(-hazardSpawnValues.x, hazardSpawnValues.x), hazardSpawnValues.y, hazardSpawnValues.z);
                    Instantiate(hazard, spawnPosition, spawnRotation);
                }
                else
                {
                    neutralToSpawn--;
                    
                    GameObject neutral = neutrals[Random.Range(0, neutrals.Length)];
                    Vector3 spawnPosition = new Vector3(Random.Range(-neutralSpawnValues.x, neutralSpawnValues.x), neutralSpawnValues.y, neutralSpawnValues.z);
                    Instantiate(neutral, spawnPosition, spawnRotation);
                }
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);

            if (_gameOver)
            {
                restartText.text = "Tap to Restart";
                break;
            }
            currentWave++;
        }

        if (!_gameOver)
        {
            gameOverText.text = "Congratulations!";
            yield return new WaitForSeconds(waveWait);
            EndGame();
        }
    }
    
    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        _gameOver = true;
    }
    
    private void EndGame()
    {
        _playerController.AddPoints(_rewardPlace.scoreValue);
        _playerController.DiscoveredPlaces.Add(_rewardPlace.id);
        _sceneController.GoToScene(SceneController.SCN_INSPIRATIONAL_LEARNING);
    }
}