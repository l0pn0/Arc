using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Параметры движения платформы")]
    [SerializeField] private float moveSpeed = 10f; // Скорость движения платформы
    private float minX; // Левая граница платформы
    private float maxX; // Правая граница платформы
    private float fixedY; // Зафиксированная позиция платформы по оси Y

    [Header("Параметры отражения мяча")]
    [SerializeField] private float maxBounceAngle = 75f; // Максимальное отклонение угла (в градусах) от вертикали

    [Header("Параметры изменения размера платформы")]
    [SerializeField] private float expandedMultiplier = 1.5f; // Множитель для расширения
    [SerializeField] private float shrunkMultiplier = 0.75f;  // Множитель для сжатия
    private Vector3 originalScale; // Исходный размер платформы

    private void Awake()
    {
        // Реализация паттерна Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Зафиксируем позицию по Y
        fixedY = transform.position.y;
        // Сохраним исходный масштаб
        originalScale = transform.localScale;

        // Определяем границы экрана для движения платформы
        float halfPlatformWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        minX = -screenHalfWidth + halfPlatformWidth;
        maxX = screenHalfWidth - halfPlatformWidth;
    }

    private void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// Управление движением платформы вслед за указателем мыши по оси X.
    /// </summary>
    private void HandleMovement()
    {
        // Получаем позицию мыши в экранных координатах и преобразуем в мировые
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Ограничиваем по X в пределах границ
        float targetX = Mathf.Clamp(worldPosition.x, minX, maxX);

        // Плавное перемещение платформы
        float smoothX = Mathf.MoveTowards(transform.position.x, targetX, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothX, fixedY, transform.position.z);
    }

    /// <summary>
    /// Логика столкновения платформы с мячом, позволяющая изменять угол отскока.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если платформа сталкивается с объектом, имеющим тег "Ball"
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Получаем Rigidbody мяча
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                // Используем первую контактную точку столкновения
                ContactPoint2D contact = collision.contacts[0];
                // Вычисляем смещение точки контакта относительно центра платформы по оси X
                float offset = contact.point.x - transform.position.x;
                // Определяем половину ширины платформы
                float halfWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
                // Нормализуем смещение в диапазоне от -1 до 1
                float normalizedOffset = Mathf.Clamp(offset / halfWidth, -1f, 1f);

                // При ударе в центр (normalizedOffset == 0) угол равен 90°, при ударе по краям – отклоняется на maxBounceAngle градусов
                float bounceAngle = 90f - normalizedOffset * maxBounceAngle; // чем больше смещение влево, тем угол больше (влево)
                float rad = bounceAngle * Mathf.Deg2Rad;
                // Вычисляем новое направление: по оси X – косинус, по оси Y – синус
                Vector2 newDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

                // Сохраняем текущую скорость мяча
                float speed = ballRb.velocity.magnitude;
                // Присваиваем мячу новое направление с сохранением модуля скорости
                ballRb.velocity = newDirection * speed;
            }
        }
    }

    /// <summary>
    /// Расширение платформы (принимает множитель, если требуется переопределить значение).
    /// </summary>
    /// <param name="multiplier">Множитель для расширения.</param>
    public void ExpandPaddle(float multiplier)
    {
        transform.localScale = originalScale * multiplier;
        Debug.Log("Платформа расширена.");
    }
    
    /// <summary>
    /// Сжатие платформы.
    /// </summary>
    public void ShrinkPaddle()
    {
        transform.localScale = originalScale * shrunkMultiplier;
        Debug.Log("Платформа сжата.");
    }

    /// <summary>
    /// Сброс размера платформы к исходному.
    /// </summary>
    public void ResetPaddleSize()
    {
        transform.localScale = originalScale;
        Debug.Log("Платформа вернула исходный размер.");
    }
}


