using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("Параметры блока")]
    [SerializeField] private int maxHealth = 1; // Максимальное здоровье блока


    [Header("Визуальные элементы")]
    [SerializeField] private Sprite[] damageSprites; // Спрайты для состояний блока
    [SerializeField] private Color[] damageColors;     // Цвета для состояний

    [Header("Настройки бонусов")]
    [SerializeField] private GameObject[] bonusPrefabs;
    [SerializeField] private float bonusDropChance = 0.3f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            TakeHit();
        }
    }

    private void TakeHit()
    {
        currentHealth--;
        if (currentHealth > 0)
        {
            UpdateVisuals();
            Debug.Log($"Блок {gameObject.name} получил урон. Осталось жизней: {currentHealth}");
        }
        else
        {
            DestroyBlock();
        }
    }

    private void UpdateVisuals()
    {
        // Вычисляем индекс состояния (0 – без повреждений)
        int damageIndex = maxHealth - currentHealth;
        damageIndex = Mathf.Clamp(damageIndex, 0, damageSprites.Length - 1);

        if (damageSprites != null && damageSprites.Length > 0)
            spriteRenderer.sprite = damageSprites[damageIndex];

        if (damageColors != null && damageColors.Length > damageIndex)
            spriteRenderer.color = damageColors[damageIndex];
    }

    private void DestroyBlock()
    {
        // Рандомный выбор бонуса
        if (bonusPrefabs.Length > 0 && Random.value <= bonusDropChance)
        {
            int bonusIndex = Random.Range(0, bonusPrefabs.Length);
            Instantiate(bonusPrefabs[bonusIndex], transform.position, Quaternion.identity);
        }
        // Увеличение счёта и уничтожение объекта
        GameManager.Instance.IncreasePoints(10);
        Destroy(gameObject);
    }
}

