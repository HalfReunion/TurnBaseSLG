using HalfStateFrame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PartyViewModel;

public class TeamCustomSystem : SystemBase
{
    /// <summary>
    /// 当前选择的队伍
    /// </summary>
    private List<CharacterInfoData> currentCustomTeam;

    protected override void OnInit()
    {
        Debug.Log("初始化TeamCustomSystem");

        PartyViewModel viewModel = new PartyViewModel();
        PartyTeamCustomModel partyModel = new PartyTeamCustomModel();
        UI3DCharacterData renderModel = new UI3DCharacterData();
        Current.RegisterModel(viewModel);
        Current.RegisterModel(partyModel);
        Current.RegisterModel(renderModel);
        //Current.RegisterEvent<int, int>("OnTeamCustomChanged", OnTeamCustomToggleClick);
        //Current.RegisterEvent<int, int>("OnTeamCustomChanged", SaveToTeamCustom);
        DataInit();
    }

    private void DataInit()
    {
        currentCustomTeam = Current.GetModel<PartyTeamCustomModel>().GetValue[0];
    }

    //写入文件，使用lastIdx
    private void SaveToTeamCustom(int nextIdx, int lastIdx)
    {
        if (lastIdx == -1) return;
        PartyTeamCustomModel partyModel = Current.GetModel<PartyTeamCustomModel>();
    }

    /// <summary>
    /// 点击自定义队伍
    /// </summary>
    /// <param name="idx"></param>
    public void TriggerToggleEvent(int idx)
    {
        currentCustomTeam = Current.GetModel<PartyTeamCustomModel>().GetValue[idx];
        Current.EventTrigger("ChangeOneRenderChar", idx, RenderChangeType.All);
        for (int i = 0; i < currentCustomTeam.Count; i++)
        {
            Current.EventTrigger("CharWithCharMsgChanged",
                currentCustomTeam[i]
                , i);
        }
    }

    /// <summary>
    /// 返回
    /// </summary>
    public List<CharacterInfoData> GetPartyViewData()
    {
        return Current.GetModel<PartyViewModel>().GetValue;
    }

    public Dictionary<int, List<CharacterInfoData>> GetCustomTeamData()
    {
        return Current.GetModel<PartyTeamCustomModel>().GetValue;
    }

    public List<RenderTexture> GetRenderTextures()
    {
        return Current.GetModel<UI3DCharacterData>().GetValue;
    }

    public void AddCharInTeam(int idx)
    {
        //3种情况：1：啥都没有 2：有，但是需要扩展槽位 3：有，很多空位是null
        if (currentCustomTeam == null)
        {
            Debug.Log("CustomTeam Is Null");
            return;
        }
        Debug.Log($"AddCharacter:{idx}");

        int slotIdx = currentCustomTeam.IndexOf(null);

        if (slotIdx == -1 && currentCustomTeam.Count >= 4) return;
        CharacterInfoData charInfo = Current.GetModel<PartyViewModel>().GetValue[idx];

        bool existsChar = currentCustomTeam.Count(x => { if (x != null) { return x.Name.Equals(charInfo.Name); } return false; }) > 0;
        if (existsChar) return;

        if (slotIdx != -1)
        {
            currentCustomTeam[slotIdx] = charInfo;
        }
        else
        {
            currentCustomTeam.Add(charInfo);
            slotIdx = currentCustomTeam.Count - 1;
        }

        //触发更换角色的效果
        Current.EventTrigger("ChangeOneRenderChar", idx, RenderChangeType.Add);
        Current.EventTrigger("CharWithCharMsgChanged",
            charInfo
            , slotIdx);
    }

    public void DelCharInTeam(int idx)
    {
        currentCustomTeam[idx] = null;
        Current.EventTrigger<CharacterInfoData, int>("CharWithCharMsgChanged",
            null
            , idx);
        Current.EventTrigger("ChangeOneRenderChar", idx, RenderChangeType.Del);
    }
}