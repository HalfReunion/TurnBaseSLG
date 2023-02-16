using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


public class Deserialize_Data_UI 
{
    public class Child_Data_UI
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Level { get; set; }
    }
    public string SystemName { get; set; }
    public string ResPath { get; set; }
    public string AbName { get; set; }

    public Child_Data_UI[] ChildUIs { get; set; }
}
