using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public const string SCN_LAUNCHER = "SCN_LAUNCHER";
    public const string SCN_EXPLORING_VIEW = "SCN_EXPLORING_VIEW";
    public const string SCN_INSPIRATIONAL_LEARNING = "SCN_INSPIRATIONAL_LEARNING";
    public const string SCN_MAIN_MENU = "SCN_MAIN_MENU";
    public const string SCN_PLACES = "SCN_PLACES";
    public const string SCN_QUIZ = "SCN_QUIZ";
    public const string SCN_QUIZ_LEARNING = "SCN_QUIZ_LEARNING";
    public const string SCN_PUZZLE_GAME = "SCN_PUZZLE_GAME";
        
    private PlayerController _playerController;
    private Stack<string> _sceneStack = new Stack<string>();
    private string _currentScene = SCN_LAUNCHER;
    
    private void Start()
    {
        _playerController = Utilities.FindPlayer();
    }

    // Update is called once per frame
    private void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform != RuntimePlatform.Android) return;
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Go back to previous scene
            GoBack();
        }
    }

    public void GoToPlacesSceneWithType(string type)
    {
        _playerController.Settings.LookedType = type;
        _playerController.Settings.SaveSettings();
        GoToScene(SCN_PLACES);
    }
    
    public void GoToScene(string sceneName)
    {
        // case: BTN_FOLLOW then BTN_CHOOSE
        // case: BTN_CHOOSE then BTN_FOLLOW
        if (_sceneStack.Count != 0 && _sceneStack.Peek().Equals(sceneName))
        {
            GoBack();
        }

        if (sceneName.Equals(SCN_INSPIRATIONAL_LEARNING))
        {
            Stack<string> newStack = new Stack<string>();
            newStack.Push(SCN_LAUNCHER);
            newStack.Push(SCN_MAIN_MENU);
            _currentScene = SCN_PLACES;
            _sceneStack = newStack;
        }
        
        _sceneStack.Push(_currentScene);
        _currentScene = sceneName;
        Debug.Log("Moving to scene " + _currentScene);
        SceneManager.LoadScene(_currentScene); 
    }

    public void GoBack()
    {
        _currentScene = _sceneStack.Pop();
        if (_currentScene.Equals(SCN_LAUNCHER))
        {
            ExitGame();
        }
        Debug.Log("Moving back to scene " + _currentScene);
        SceneManager.LoadScene(_currentScene); 
    }

    //case: Lose the game or regular goBack
    public void GoBackFromGame()
    {
        while (!_sceneStack.Peek().Equals(SCN_EXPLORING_VIEW))
        {
            _sceneStack.Pop();
        }

        GoBack();
    }

    public void ExitGame()
    {
        _playerController.SaveGame();
        _playerController.Settings.SaveSettings();
        Application.Quit();
    }
}
