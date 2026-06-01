using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    public void SetMaxHealth(int health)
    {
        if (slider != null)
        {
            slider.maxValue = health;
            slider.value = health;
        }
        else
        {
            Debug.LogError($"[HealthBar Error] Komponen Slider tidak ditemukan di GameObject: {gameObject.name}", this);
        }
    }

    public void SetHealth(int health)
    {
        if (slider != null)
        {
            slider.value = health;
        }
        else
        {
            Debug.LogError($"[HealthBar Error] Komponen Slider tidak ditemukan di GameObject: {gameObject.name}", this);
        }
    }
}