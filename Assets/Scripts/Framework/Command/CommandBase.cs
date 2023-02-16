

namespace HalfStateFrame { 
//可持有  
public interface IHasModule { 
    public void SetModule(IModule module);
}

public interface ICommand : IHasModule
{
    public void Execute();
}

public abstract class CommandBase : ICommand
{
    IModule module;

    public void Execute()
    {
         
    } 
    public void SetModule(IModule module)
    {
        this.module = module;
    }
}
}
