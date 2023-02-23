using HalfStateFrame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MainMenuState : StateBase
{
    public override void OnEnter(IModel message = null)
    {
        if (message != null)
        {
            RegisterModel(message);
        }
        RegisterSystem(new TeamCustomSystem());

        RegisterSystem(new TeamMenuUI()).OpenUI<TeamCustom>();
    }

    public override void OnExit(out IModel message)
    {
        message= null;
    }

    public override void OnUpdate(float time)
    {
        
    }
}
 
