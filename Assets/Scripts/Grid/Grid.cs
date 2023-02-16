using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct GridPos:IEquatable<GridPos>
{
    public int x;
    public int y; 

    public GridPos(int x, int y) {
        this.x = x;
        this.y = y;
    }
     

    public override string ToString() {
        return $"X:{this.x}  Y:{this.y}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x,y);
    }

    public bool Equals(GridPos obj)
    {
        return obj is GridPos pos && x == pos.x && y == pos.y;
    }

    public static bool operator ==(GridPos left, GridPos right) 
    { 
        return left.x == right.x && left.y == right.y;
    }
    public static bool operator !=(GridPos left, GridPos right)
    {
        return !(left==right);
    }
    public static GridPos operator +(GridPos left, GridPos right) {
        return new GridPos(left.x + right.x, left.y + right.y);
    }
}

public class Grid {
    private GridSystem _system;
    private GridPos _pos;
    private List<Unit> unitList;

    public GridPos Pos=> _pos;

    public Grid(GridSystem _system, GridPos _pos) { 
        this._system = _system;
        this._pos = _pos;
        unitList = new List<Unit>();
    }

    public override string ToString() {
        
        StringBuilder sb = new StringBuilder();
        foreach (Unit unit in unitList) {
            sb.Append(unit+"\n");
        }

        return _pos.ToString() + "\n" + sb.ToString();
         
    }

    public void AddUnit(Unit unit) { 
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit) {
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList() { 
        return unitList; 
    }

    public Unit GetUnitFirst() {
        if (unitList.Count > 0) {
            return unitList.FirstOrDefault();
        }
        return null;
    }
    
}

 
 
