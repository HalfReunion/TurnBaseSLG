using HalfStateFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamCustomSystem : SystemBase
{   
    protected override void OnInit()
    {
        Debug.Log("≥ı ºªØTeamCustomSystem");
        PartyViewModel viewModel = new PartyViewModel(); 
        Current.RegisterModel(viewModel);
    }

    /// <summary>
    /// ∑µªÿ
    /// </summary>
    public void PartyViewData() { 
        
    }


}
