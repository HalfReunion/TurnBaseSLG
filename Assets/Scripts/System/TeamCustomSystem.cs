using HalfStateFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void PartyViewData() { 
        
    }


}
