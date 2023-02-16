using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using DG.Tweening;
using UnityEngine.UI;
public class EmWhiteAnim : MonoBehaviour
{
    private Image image;

    private Vector2 startPos;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Init(Vector2 startPos) {
        this.startPos = startPos;
    }
    // Start is called before the first frame update
    void Start()
    {
        image.DOFade(0, 1f);
        image.rectTransform.DOAnchorPos(new Vector2(startPos.x, -20f), 1f).OnComplete(
            () =>
            {
                Destroy(gameObject);
            });
    }  
}
