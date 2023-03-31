using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealthUI : MonoBehaviour
{
    private GameObject emWhite;

    [SerializeField]
    private Image healthBar;

    private RectTransform rect;

    [SerializeField]
    private StatusPanelUI parentUI;

    private void Awake()
    {
        healthBar = transform.Find("Bar").GetComponent<Image>();
        emWhite = transform.Find("EmWhite").gameObject;
        parentUI = GetComponentInParent<StatusPanelUI>();
        rect = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        parentUI.Unit.HealthSystem.OnDamaged += OnHealthBarChanged;
        parentUI.Unit.HealthSystem.OnDead += OnDead;
    }

    private void OnHealthBarChanged(object sender, DamageMessage damageMessage)
    {
        float damageNor = damageMessage.Damage / damageMessage.MaxHealth;
        //float endValue = healthBar.fillAmount - damageNor;
        //healthBar.DOFillAmount(endValue, 1f);
        healthBar.fillAmount -= damageNor;
        damageAnima(damageNor, healthBar.fillAmount);
    }

    private void OnDead(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void damageAnima(float damageNor, float healthNor)
    {
        GameObject emW = GameObject.Instantiate(emWhite, transform);
        EmWhiteAnim emWS = emW.GetComponent<EmWhiteAnim>();
        RectTransform ret = emW.GetComponent<RectTransform>();
        //¸ß¿í
        ret.sizeDelta = new Vector2(getEmBarWidth(damageNor), ret.rect.height);

        ret.anchoredPosition = new Vector2(getEmBarWidth(healthNor), 0);
        emWS.Init(ret.anchoredPosition);
        emW.SetActive(true);
    }

    private float getEmBarWidth(float damageNor)
    {
        float width = rect.rect.width;
        return width * damageNor;
    }

    private void OnDestroy()
    {
        parentUI.Unit.HealthSystem.OnDamaged -= OnHealthBarChanged;
        parentUI.Unit.HealthSystem.OnDead -= OnDead;
    }
}