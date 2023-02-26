using HalfStateFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PartyViewModel;

public class TeamCustomSystem : SystemBase
{   
    protected override void OnInit()
    {
        Debug.Log("��ʼ��TeamCustomSystem");
        PartyViewModel viewModel = new PartyViewModel(); 
        Current.RegisterModel(viewModel);
    }

    /// <summary>
    /// ����
    /// </summary>
    public List<CharacterTData> PartyViewData() { 
        return Current.GetModel<PartyViewModel>().GetValue;
    }


}
