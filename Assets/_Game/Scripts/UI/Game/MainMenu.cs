using UnityEngine;
using TMPro;
public class MainMenu : UICanvas
{
    [SerializeField] private TextMeshProUGUI tmpLevel;
    public override void Setup()
    {
        base.Setup();
    }
    public override void Open()
    {
        base.Open();
        SetUIMainMenu();
    }
    private void SetUIMainMenu()
    {
        tmpLevel.text = $"{"Level " + (DataManager.Ins.playerData.levelCurrent + 1)}"; ;
    }
    public void ButtonPlay()
    {
        UIManager.Ins.CloseAll();
        UIManager.Ins.OpenUI<UIGamePlay>();
        LevelManager.Ins.OnLoadLevel(DataManager.Ins.playerData.levelCurrent);
    }
}
