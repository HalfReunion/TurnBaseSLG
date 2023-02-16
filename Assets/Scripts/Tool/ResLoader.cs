using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public interface IResLoader
{
    public T LoadAsset<T>(string abName, string resName) where T : UnityEngine.Object;
}


public class ABResLoader : Singleton<ABResLoader>, IResLoader
{
    //主包
    private AssetBundle mainPack = null;
    //主包依赖获取配置文件
    private AssetBundleManifest manifest = null;

    //选择存储 AB包的容器
    //AB包不能够重复加载 否则会报错
    //字典知识 用来存储 AB包对象
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();


    /// <summary>
    /// 获取AB包加载路径
    /// </summary>
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 主包名 根据平台不同 报名不同
    /// </summary>
    private string MainPackName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "Windows";
#endif
        }
    }

    /// <summary>
    /// 加载主包 和 配置文件
    /// 因为加载所有包是 都得判断 通过它才能得到依赖信息
    /// 所以写一个方法
    /// </summary>
    private void LoadMainPack()
    {
        if (mainPack == null)
        {
            mainPack = AssetBundle.LoadFromFile(PathUrl + MainPackName);
            manifest = mainPack.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    /// <summary>
    /// 加载指定包的依赖包
    /// </summary>
    /// <param name="abName"></param>
    private void LoadDependencies(string abName)
    {
        //加载主包
        LoadMainPack();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                if (ab != null)
                {
                    Debug.Log("LoadDependencies：添加");
                    abDic.Add(strs[i], ab);
                }
            }
        }
    }


    public T LoadAsset<T>(string abName, string resName) where T : UnityEngine.Object
    {
        LoadDependencies(abName);
        if (!abDic.ContainsKey(abName))
        {
            AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
            if (ab != null)
            { 
                abDic.Add(abName, ab);
            }
        }
        T obj = abDic[abName].LoadAsset<T>(resName);
        return obj;
    }

    public GameObject GetGameObject(string abName, string resName) {
        return null;
    }
}

