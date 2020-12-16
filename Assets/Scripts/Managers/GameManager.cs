using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStatus { Preparing, Game, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameStatus gameStatus = GameStatus.Preparing;
    public GameStatus GameStatus
    {
        get
        {
            return gameStatus;
        }

        set
        {
            gameStatus = value;

            switch (gameStatus)
            {
                case GameStatus.Preparing:
                    break;

                case GameStatus.Game:
                    StartGame();
                    break;

                case GameStatus.GameOver:
                    GameOver();
                    break;
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            this.enabled = false;
    }

    private void Update()
    {
        if(Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            switch (gameStatus)
            {
                case GameStatus.Preparing:
                    GameStatus = GameStatus.Game;
                    break;

                case GameStatus.Game:
                    break;

                case GameStatus.GameOver:
                    SceneManager.LoadScene(0);
                    break;
            }
        }
    }

    private void StartGame()
    {
        UIManager.instance.HideInfoText();
        BaseUnit.ActivateAllUnits();
    }

    private void GameOver()
    {
        UIManager.instance.ShowInfoText("Click to new game!");
    }
}
