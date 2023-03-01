using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SerializeTool
{
    public static List<T> DeserializeToList<T>(TextAsset textAsset)
    {
        string txt = textAsset.text;

        var data = JsonConvert.DeserializeObject<List<T>>(txt);

        return data;
    }

    public static List<T> DeserializeToList<T>(string txt)
    {  
        var data = JsonConvert.DeserializeObject<List<T>>(txt);

        return data;
    }

    public static T Deserialize<T>(string txt)
    { 
        var data = JsonConvert.DeserializeObject<T>(txt);

        return data;
    }
}
