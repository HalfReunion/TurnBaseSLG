using HalfStateFrame;
using System.Collections.Generic;
using UnityEngine;
using static PartyViewModel;

public class TeamCustomSystem : SystemBase
{
    /// <summary>
    /// ��ǰѡ��Ķ���
    /// </summary>
    private List<CharacterInfoData> currentCustomTeam;

    protected override void OnInit()
    {
        Debug.Log("��ʼ��TeamCustomSystem");

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
    /// ʹ��nextIdx���ı䵱ǰ��List
    /// </summary>
    /// <param name="nextIdx"></param>
    /// <param name="lastIdx"></param>
    private void OnTeamCustomToggleClick(int nextIdx, int lastIdx)
    {
        if (nextIdx == -1) return;
        currentCustomTeam = Current.GetModel<PartyTeamCustomModel>().GetValue[nextIdx];
    }

    //д���ļ���ʹ��lastIdx
    private void SaveToTeamCustom(int nextIdx, int lastIdx)
    {
        if (lastIdx == -1) return;
        PartyTeamCustomModel partyModel = Current.GetModel<PartyTeamCustomModel>();
    }

    /// <summary>
    /// ����Զ������
    /// </summary>
    /// <param name="idx"></param>
    public void TriggerToggleEvent(int idx)
    {
        Current.EventTrigger("OnTeamCustomChanged", idx, -1);
    }

    /// <summary>
    /// ����
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
        //3�������1��ɶ��û�� 2���У�������Ҫ��չ��λ 3���У��ܶ��λ��null
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

        //����������ɫ��Ч��
        Current.EventTrigger("ChangeOneRenderChar", idx);
    }

    public void DelCharInTeam(int idx)
    {
    }
}