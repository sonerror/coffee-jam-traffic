using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState { GamePlay, MainMenu, Finish, Revive, Setting, Pause, ShowPassedLevel,Tutorial }
public class GameManager : Singleton<GameManager>
{
    public GameState gameState = GameState.MainMenu;
    public bool isShowPopupStarter;
    public void ChangeState(GameState gameState)
    {
        this.gameState = gameState;
    }

    public bool IsState(GameState gameState) => this.gameState == gameState;
    private void Awake()
    {
        Application.targetFrameRate = 1000;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StartCoroutine(I_InitGame());
    }

    IEnumerator I_InitGame()
    {
        yield return new WaitUntil(
            () => (
            Ins != null
            && UIManager.Ins != null
            && DataManager.Ins != null
            && LevelManager.Ins != null
            )
        );
        yield return new WaitForEndOfFrame();   
        isShowPopupStarter = true;
        UIManager.Ins.Oninit();
        DataManager.Ins.LoadData();
    }
}
