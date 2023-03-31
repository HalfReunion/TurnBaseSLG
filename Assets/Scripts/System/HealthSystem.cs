using System;
using UnityEngine;

public struct DamageMessage
{
    public float Damage;
    public float MaxHealth;
}

namespace CoreSystem
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth = 100;

        [SerializeField] private float def = 10;
        [SerializeField] private float currentDef = 10;

        public event EventHandler OnDead;

        public event EventHandler<DamageMessage> OnDamaged;

        public void Damage(int damageAmount)
        {
            int damageC = (int)(damageAmount * (currentDef / (currentDef + 10)));
            currentHealth -= damageC;
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            if (OnDamaged != null)
            {
                DamageMessage damage = new DamageMessage();
                damage.Damage = damageC;
                damage.MaxHealth = maxHealth;
                OnDamaged(this, damage);
            }
            if (currentHealth == 0)
            {
                die();
            }
        }

        public void ReduceDef(float i)
        {
            currentDef = currentDef * (1 - i / 100);
            Debug.Log($"·ÀÓùÁ¦:{currentDef}¼õÉÙ:{i}");
        }

        public void RestoreDef()
        {
            currentDef = def;
        }

        private void die()
        {
            OnDead?.Invoke(this, EventArgs.Empty);
        }
    }
}