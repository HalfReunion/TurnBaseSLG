using HalfStateFrame;
using System.Collections.Generic;
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
        Current.RegisterEvent<int, int>("OnTeamCustomChanged", OnTeamCustomToggleClick);
        Current.RegisterEvent<int, int>("OnTeamCustomChanged", SaveToTeamCustom);
        DataInit();
    }

    private void DataInit()
    {
        currentCustomTeam = Current.GetModel<PartyTeamCustomModel>().GetValue[0];
    }

    /// <summary>
    /// 使用nextIdx，改变当前的List
    /// </summary>
    /// <param name="nextIdx"></param>
    /// <param name="lastIdx"></param>
    private void OnTeamCustomToggleClick(int nextIdx, int lastIdx)
    {
        if (nextIdx == -1) return;
        currentCustomTeam = Current.GetModel<PartyTeamCustomModel>().GetValue[nextIdx];
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
        Current.EventTrigger("OnTeamCustomChanged", idx, -1);
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
       
        int nullIdx = currentCustomTeam.IndexOf(null);

        if (nullIdx == -1 && currentCustomTeam.Count >= 4) return;
        if (nullIdx != -1)
        {
            currentCustomTeam[nullIdx] = Current.GetModel<PartyViewModel>().GetValue[idx];
        }
        else
        {
            currentCustomTeam.Add(Current.GetModel<PartyViewModel>().GetValue[idx]);
        }

        //触发更换角色的效果
        Current.EventTrigger("ChangeOneRenderChar", idx);
    }

    public void DelCharInTeam(int idx)
    {
    }
}