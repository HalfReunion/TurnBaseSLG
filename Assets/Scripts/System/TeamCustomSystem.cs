using HalfStateFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PartyViewModel;

public class TeamCustomSystem : SystemBase
{
    /// <summary>
    /// ��ǰѡ��Ķ���
    /// </summary>
    private Dictionary<int, List<CharacterInfoData>> allCustomTeams;

    private SwitchCustomAndMapAnima<List<GameObject>> switchAnima;
    private MapSelectAnima<Transform> mapSelectAnima;
    
    private Dictionary<string,UISystem> childUISystem = new Dictionary<string, UISystem>();
         
    private int currentTeamID;
    private int stageID;

    protected override void OnInit()
    {
        Debug.Log("��ʼ��TeamCustomSystem");

        
        currentTeamID = 0;
        allCustomTeams = new Dictionary<int, List<CharacterInfoData>>();
        
        PartyViewModel viewModel = new PartyViewModel();
        PartyTeamCustomModel partyModel = new PartyTeamCustomModel();
        UI3DCharacterData renderModel = new UI3DCharacterData();
        Current.RegisterModel(new TeamStageOutPutModel());
        Current.RegisterModel(viewModel);
        Current.RegisterModel(partyModel);
        Current.RegisterModel(renderModel);
        TeamCustomDataInit();
        InitMapSystemData();
       
    }

    //HACKER �����ʼ�� ��ʼ��File���ù���������
    private void TeamCustomDataInit()
    {
        for (int i = 0; i < 3; i++) { 
            List<CharacterInfoData> temp = GetCustomCharacterInfoDatasByTeamID(i);
            allCustomTeams.Add(i, temp);
        }
    }

    public override void RenderInit()
    {
        TriggerToggleEvent(0);
        SoundManager.Instance.PlayBackground("1st PV Animation Theme");
        
        InitSwitchAnima();
        InitMapComfirmAnima();
    }
    public void RegisterChildSystem(string name, UISystem uiSystem)
    {
        childUISystem.Add(name, uiSystem);
    }

    private void InitSwitchAnima()
    {
        List<GameObject> io = new List<GameObject>();
        io.Add(childUISystem["TeamMenuUI"].GetUI<TeamCustomUI>().gameObject);
        io.Add(childUISystem["MapSelectUI"].GetUI<MapSelectUI>().gameObject);
        switchAnima = new SwitchCustomAndMapAnima<List<GameObject>>(io); 
    }

    private void InitMapComfirmAnima() {
        mapSelectAnima = new MapSelectAnima<Transform>(childUISystem["MapSelectUI"].GetUI<MapSelectUI>().transform);
    }

    public void ExecuteSwitchToMap() {
        switchAnima.Execute();
    }

    /// <summary>
    /// ����ѡ��Ķ���ؿ�ID������Ϸ
    /// </summary>
    public void EnterStage() {
        Current.GetModel<TeamStageOutPutModel>().CurrentTeamID = currentTeamID;
        Current.GetModel<TeamStageOutPutModel>().StageID = stageID;
        Current.GetModel<TeamStageOutPutModel>().CharacterInfoDatas = GetCustomCharacterInfoDatasByTeamID(currentTeamID);
        Debug.Log($"{stageID},{currentTeamID}");
        //�л�����
        SceneLoader.Instance.ChangedToLoadingScene();
    }

    #region MapSelectSystem Part
    private void InitMapSystemData() {
        Current.RegisterModel(new MapStageInfoModels());

    }

    public List<StageInfoData> GetAllStageInfoData()
    {
        return Current.GetModel<MapStageInfoModels>().GetValue();
    }

    public void OnClickMapCancel()
    {
        switchAnima.CancelExecute();
    }

    public void OnClickMapConfirm(Action<StageInfoData> data,int stageID)
    {
        this.stageID = stageID;
        mapSelectAnima.Execute();
        data(GetStageInfoByIndex(stageID));
    }

    public StageInfoData GetStageInfoByIndex(int index)
    {
        return GetAllStageInfoData()[index];
    }

    public StageInfoData GetStageInfoByID(int id) {
        return Current.GetModel<MapStageInfoModels>().GetValueById(id);
    }

    public void OnClickStageBack()
    {
        mapSelectAnima.CancelExecute();
    }
    #endregion
     
    #region TeamSystem part
    public void SaveToTeamCustom()
    {
        Current.GetModel<PartyTeamCustomModel>().SaveToFile(allCustomTeams);
    }

    /// <summary>
    /// ����Զ������
    /// </summary>
    /// <param name="idx"></param>
    public void TriggerToggleEvent(int idx)
    {
        currentTeamID = idx;
        
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
            int index = customTeamIDs[i];
            if (index == -1)
            { customList.Add(null); continue; }
            customList.Add(Current.GetModel<PartyViewModel>().GetCharByID(customTeamIDs[i]));
        }
        return customList;
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
        return allCustomTeams;
    }

    public List<RenderTexture> GetRenderTextures()
    {
        return Current.GetModel<UI3DCharacterData>().GetValue;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="idx"> ����������id</param>
    public void AddCharInTeam(int idx)
    {
        //3�������1��ɶ��û�� 2���У�������Ҫ��չ��λ 3���У��ܶ��λ��null
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

        //����������ɫ��Ч��
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
#endregion