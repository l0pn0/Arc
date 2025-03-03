using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BonusType
{
    ExpandPaddle,
    ShrinkPaddle,
    ExtraLife,
    SlowBall,
    FastBall
}

public class BonusController : MonoBehaviour
{
    [Header("Общие настройки бонуса")]
    [SerializeField] private float fallSpeed = 3f; // Скорость падения бонуса

    // Параметры для бонусов, которые требуют дополнительных значений:
    [SerializeField] private float paddleScaleMultiplier = 1.5f; // Множитель для расширения/сжатия платформы
    [SerializeField] private float bonusDuration = 10f;          // Длительность действия бонуса

    // Внутреннее поле для хранения выбранного типа бонуса
    private BonusType bonusType;

    private void Start()
    {
        // Случайным образом выбираем тип бонуса из перечисления
        BonusType[] bonusTypes = (BonusType[])System.Enum.GetValues(typeof(BonusType));
        bonusType = bonusTypes[Random.Range(0, bonusTypes.Length)];
        Debug.Log("Случайно выбран бонус: " + bonusType);
    }

    private void Update()
    {
        // Двигаем бонус вниз
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        // Удаляем бонус, если он за пределами экрана (например, ниже y = -6)
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем столкновение с платформой (игроком)
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyBonus(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void ApplyBonus(GameObject player)
    {
        // Здесь можно вызвать метод из GameManager для применения бонуса
        // Например:
        GameManager.Instance.ApplyBonus(bonusType, paddleScaleMultiplier, bonusDuration);

        // Или напрямую изменить параметры игрока, если логика бонусов реализована тут
        Debug.Log("Применяем бонус: " + bonusType);
    }
}