public abstract class CombineAction : BaseAction
{
    public virtual new bool IsValidActionGridPos(GridPos gridPos)
    {
        return false;
    }
}