using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalfStateFrame;
public class BuffSystem : SystemBase
{
    public List<BuffData> buffDatas;

    //Buff归属字典
    private Dictionary<Unit, List<BaseBuff>> buffUnitList;

    protected override void OnInit()
    {
        buffDatas = new List<BuffData>();
        buffUnitList = new Dictionary<Unit, List<BaseBuff>>(); 
    }

    public void AddBuff<T>(Unit unit, T buff) where T : BaseBuff
    {
        List<BaseBuff> list;
        //找不到角色
        if (!buffUnitList.TryGetValue(unit, out list))
        {
            list = new List<BaseBuff>();
            buffUnitList.Add(unit, list);
        }

        BaseBuff temp = list.Find((t) => { return t.ToString().Equals(buff.ToString()); });
        if (temp != null)
        {
            temp.Refresh(this, EventArgs.Empty);
            return;
        }

        list.Add(buff);
        buff.Execute(unit.HealthSystem, EventArgs.Empty);
        //BroadBuffChangedUI(this, buffUnitList);
    }

}
 
