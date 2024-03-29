using TMPro;
using UnityEngine.UI;

public class TeamSelection : UIBase
{
    private Image charIcon;
    private TMP_Text charName;
    private int idx;

    private void Awake()
    {
        charIcon = transform.Find("CharIcon").GetComponent<Image>();
        charName = transform.Find("CharName").GetComponent<TMP_Text>();
    }

    public void SetData(CharacterInfoData ch)
    {
        charIcon.sprite = ch.Icon;
        charName.text = ch.Name;
    }

    public void SetIdx(int idx)
    {
        this.idx = idx;
    }
}