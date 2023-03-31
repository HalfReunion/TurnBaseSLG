using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private Text txt;
    [SerializeField] private Button btn;
    [SerializeField] private GameObject frame;
    private BaseAction baseAction;

    //添加按钮的功能点击
    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        txt.text = baseAction.GetActionName().ToUpper();

        btn.onClick.AddListener(() =>
        {
            if (!baseAction.HasTaken)
            {
                //修改装载的行为
                //点击button会装载对应的action
                UnitActionSystem.Instance.SetUnitAction(baseAction);
                if (baseAction is MoveAction)
                {
                    GridSystemVisual.Instance.UpdateCombineGridVisual();
                }
                else
                {
                    GridSystemVisual.Instance.UpdateGridVisual();
                }
                return;
            }
            Debug.Log("已行动过");
        });
        return;
    }

    /// <summary>
    /// UI状态更新
    /// </summary>
    public void UpdateSelectedVisual()
    {
        frame.SetActive(UnitActionSystem.Instance.SelectedAction == this.baseAction && !this.baseAction.HasTaken);
    }
}