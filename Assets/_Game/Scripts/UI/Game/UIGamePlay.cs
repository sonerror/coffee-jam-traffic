using UnityEngine;
using UnityEngine.UI;
public class UIGamePlay : UICanvas
{

    public override void Setup()
    {
        base.Setup();
    }
    public override void Open()
    {
        base.Open();
    }
    public void ButtonHome()
    {
        UIManager.Ins.CloseUI<UIGamePlay>();
        UIManager.Ins.OpenUI<MainMenu>();
    }
}
