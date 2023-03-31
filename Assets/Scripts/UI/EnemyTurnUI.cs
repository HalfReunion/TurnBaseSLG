using UnityEngine;
using UnityEngine.UI;

public class EnemyTurnUI : MonoBehaviour
{
    private Text txt;

    private void Awake()
    {
        txt = GetComponent<Text>();
    }

    public void Show(string txt)
    {
        this.txt.text = txt;
    }

    public void Hide()
    { }
}