using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance;

    [Header("Параметры шарика")]
    [SerializeField] private float initialSpeed = 10f;
    [SerializeField] private float launchDelay = 0f;

    [Header("Настройки отражения от платформы")]
    [SerializeField] private float maxBounceAngle = 60f; // Максимальное отклонение от вертикали (в градусах)

    private Rigidbody2D rb;

    [Header("Параметры потери шарика")]
    [SerializeField] private float fallThreshold = -6f; // Нижняя граница по оси Y, ниже которой мяч считается потерянным
    private bool hasTriggeredLoss = false;

    private float speedMultiplier = 1f; // Множитель скорости мяча

    private void Awake()
    {
        // Реализация Singleton'а
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        hasTriggeredLoss = false;
        Debug.Log("Новый мяч инициализирован: " + gameObject.name);

        if (launchDelay > 0f)
            Invoke(nameof(LaunchBall), launchDelay);
        else
            LaunchBall();
    }

    private void Update()
    {
        // Если мяч падает ниже порога, уведомляем GameManager и уничтожаем его
        if (!hasTriggeredLoss && transform.position.y < fallThreshold)
        {
            hasTriggeredLoss = true;

            // Предполагаем, что GameManager реализован как Singleton
            GameManager.Instance.OnBallLost();
            Destroy(gameObject);
        }
    }

    private void LaunchBall()
    {
        // Выбираем случайный угол в диапазоне [45°, 135°] (направлен вверх)
        float randomAngle = Random.Range(45f, 135f);
        float radAngle = randomAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle)).normalized;

        rb.velocity = direction * initialSpeed * speedMultiplier;
        Debug.Log("Мяч запущен под углом: " + randomAngle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если мяч столкнулся с платформой (предполагается, что платформе присвоен тег "Player")
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform platform = collision.transform;
            SpriteRenderer sr = platform.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                float platformWidth = sr.bounds.size.x;
                // Вычисляем смещение удара от центра платформы
                float hitOffset = transform.position.x - platform.position.x;
                float normalizedOffset = hitOffset / (platformWidth / 2);

                // При ударе по центру мяч летит вертикально вверх (90°), а по краям угол смещается
                float bounceAngle = 90f + normalizedOffset * maxBounceAngle;
                float radBounceAngle = bounceAngle * Mathf.Deg2Rad;

                Vector2 newDirection = new Vector2(Mathf.Cos(radBounceAngle), Mathf.Sin(radBounceAngle)).normalized;
                float currentSpeed = initialSpeed * speedMultiplier;
                rb.velocity = newDirection * currentSpeed;
            }
        }
        // Остальные столкновения (со стенами, блоками) обрабатываются стандартной физикой Unity.
    }

    // Метод для установки множителя скорости
    public void SetBallSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
        rb.velocity = rb.velocity.normalized * initialSpeed * speedMultiplier;
        Debug.Log("Множитель скорости мяча установлен на: " + multiplier);
    }

    // Метод для сброса множителя скорости
    public void ResetBallSpeed()
    {
        speedMultiplier = 1f;
        rb.velocity = rb.velocity.normalized * initialSpeed * speedMultiplier;
        Debug.Log("Множитель скорости мяча сброшен.");
    }
}



