using UnityEngine;

public enum UI_Status
{
    Close,
    Open,
    Hide
}

public enum UI_Level
{
    Main,
    Game,
    Popup
}

public interface IUIBase
{
    public string UIName { get; }

    public UI_Status UIState { get; }

    public UI_Level UILevel { get; }

    public UIBase Show();

    public void Hide();

    public void Close();
}

public abstract class UIBase : MonoBehaviour, IUIBase
{
    public virtual string UIName
    { get { return null; } }

    public UI_Level UILevel { get => m_Level; set => m_Level = value; }
    protected UI_Level m_Level;

    public UI_Status UIState { get => m_Status; set => m_Status = value; }
    protected UI_Status m_Status = UI_Status.Close;

    protected UISystem uiSystem;

    private void Start()
    {
        OnInit();
    }

    public void Init(UISystem uiSystem)
    {
        if (this.uiSystem != null) { return; }
        Debug.Log("Init System");
        this.uiSystem = uiSystem;
    }

    protected virtual void OnInit()
    { }

    public virtual void Close()
    {
        m_Status = UI_Status.Close;
        Destroy(gameObject);
    }

    public virtual void Hide()
    {
        m_Status = UI_Status.Hide;
        gameObject.SetActive(false);
    }

    public virtual UIBase Show()
    {
        m_Status = UI_Status.Open;
        if (gameObject.activeSelf != false)
        {
            gameObject.SetActive(true);
        }
        return this;
    }
}