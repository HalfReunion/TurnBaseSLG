using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private const string TEAMSCENE = "TeamCustom";

    private static SceneLoader instance;
    public static SceneLoader Instance => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async UniTask ChangeScene(string name, Action onStart, Action<AsyncOperation> onComplete)
    {
        AsyncOperation asy = SceneManager.LoadSceneAsync(name);
        asy.allowSceneActivation = false;

        onComplete(asy);
    }

    public void ChangedToTeamCustom()
    {
        ChangeScene(TEAMSCENE, () =>
        {
        },
        (AsyncOperation asy) =>
        {
            asy.allowSceneActivation = true;
        }
        );
    }
}