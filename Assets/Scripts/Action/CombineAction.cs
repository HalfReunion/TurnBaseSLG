using System.Collections.Generic;

public abstract class CombineAction:BaseAction{
    public new virtual bool IsValidActionGridPos(GridPos gridPos) {
        return false;
    }
}
