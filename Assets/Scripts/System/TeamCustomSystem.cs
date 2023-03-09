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
    private Dictionary<int, List<CharacterInfoData>> allCustomTeams;

    private int currentTeamID;

    protected override void OnInit()
    {
        Debug.Log("初始化TeamCustomSystem");
        currentTeamID = 0;
        allCustomTeams = new Dictionary<int, List<CharacterInfoData>>();

        PartyViewModel viewModel = new PartyViewModel();
        PartyTeamCustomModel partyModel = new PartyTeamCustomModel();
        UI3DCharacterData renderModel = new UI3DCharacterData();
        Current.RegisterModel(viewModel);
        Current.RegisterModel(partyModel);
        Current.RegisterModel(renderModel);
        Current.RegisterEvent("SaveToSetting", SaveToTeamCustom);
        DataInit();
    }

    private void DataInit()
    {
        if (!allCustomTeams.ContainsKey(0))
        {
            List<CharacterInfoData> list = GetCustomCharacterInfoDatasByTeamID(0);
            allCustomTeams.Add(0, list);
        }
    }

    
    public void SaveToTeamCustom()
    {
        Current.GetModel<PartyTeamCustomModel>().SaveToFile(allCustomTeams);
    }

    /// <summary>
    /// 点击自定义队伍
    /// </summary>
    /// <param name="idx"></param>
    public void TriggerToggleEvent(int idx)
    {
        currentTeamID = idx;
        if (!allCustomTeams.ContainsKey(idx))
        {
            List<CharacterInfoData> temp = GetCustomCharacterInfoDatasByTeamID(idx);
            allCustomTeams.Add(idx, temp);
        }

        Current.EventTrigger("ChangeOneRenderChar", idx, RenderChangeType.All);
        for (int i = 0; i < allCustomTeams[idx].Count; i++)
        {
            if (allCustomTeams[idx][i] == null) continue;

            Current.EventTrigger("CharWithCharMsgChanged",
                allCustomTeams[idx][i]
                , i);
        }
    }

    public List<CharacterInfoData> GetCurrentTeam(int idx)
    {
        return allCustomTeams[idx];
    }

    private List<CharacterInfoData> GetCustomCharacterInfoDatasByTeamID(int idx)
    {
        List<int> customTeamIDs = Current.GetModel<PartyTeamCustomModel>().GetValue[idx];
        List<CharacterInfoData> customList = new List<CharacterInfoData>();
        for (int i = 0; i < customTeamIDs.Count; i++)
        {
            customList.Add(Current.GetModel<PartyViewModel>().GetCharByID(customTeamIDs[i]));
        }
        return customList;
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
        return allCustomTeams;
    }

    public List<RenderTexture> GetRenderTextures()
    {
        return Current.GetModel<UI3DCharacterData>().GetValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idx"> 参数是人物id</param>
    public void AddCharInTeam(int idx)
    {
        //3种情况：1：啥都没有 2：有，但是需要扩展槽位 3：有，很多空位是null
        if (allCustomTeams == null)
        {
            Debug.Log("CustomTeam Is Null");
            return;
        }

        Debug.Log($"Put character in {idx}");
        List<CharacterInfoData> customTeam = allCustomTeams[currentTeamID];

        Debug.Log($"AddCharacter:{idx}");

        int slotIdx = customTeam.IndexOf(null);

        if (slotIdx == -1 && customTeam.Count >= 4) return;
        CharacterInfoData charInfo = Current.GetModel<PartyViewModel>().GetValue[idx];

        bool existsChar = customTeam.Count(x => { if (x != null) { return x.Name.Equals(charInfo.Name); } return false; }) > 0;
        if (existsChar) return;

        if (slotIdx != -1)
        {
            customTeam[slotIdx] = charInfo;
        }
        else
        {
            customTeam.Add(charInfo);
            slotIdx = customTeam.Count - 1;
        }

        //触发更换角色的效果
        Current.EventTrigger("ChangeOneRenderChar", idx, RenderChangeType.Add);
        Current.EventTrigger("CharWithCharMsgChanged",
            charInfo
            , slotIdx);
    }

    public void DelCharInTeam(int idx)
    {
        allCustomTeams[currentTeamID][idx] = null;
        Current.EventTrigger<CharacterInfoData, int>("CharWithCharMsgChanged",
            null
            , idx);
        Current.EventTrigger("ChangeOneRenderChar", idx, RenderChangeType.Del);
    }

  
}