using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    public float currentHealth;
    public event Action<float> OnHealthPercentageChange;
    public Transform Healthbar;
    private float initialScaleY;

    void Start()
    {
        initialScaleY = Healthbar.localScale.y;
    }

    private void OnEnable()
    {
        maxHealth += Game.Instance.enemyHealthMultiplier;
        currentHealth = maxHealth;
    }

    public void ModifyHealth(float amount)
    {
        currentHealth += amount;
        float currentHealthPercentage = currentHealth / maxHealth;
        Healthbar.transform.localScale = new Vector3(Healthbar.transform.localScale.x, initialScaleY * currentHealthPercentage, Healthbar.transform.localScale.z);
        OnHealthPercentageChange?.Invoke(currentHealthPercentage);
    }
}
