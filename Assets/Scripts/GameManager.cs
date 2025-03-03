using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Настройки игры")]
    [SerializeField] private int totalLives = 3;           // Общее количество жизней
    [SerializeField] private Transform ballSpawnPoint;     // Точка спавна мяча
    [SerializeField] private GameObject ballPrefab;        // Префаб мяча
    [SerializeField] private float respawnDelay = 1f;      // Задержка перед спавном нового мяча

    [Header("UI Elements")]
    [SerializeField] private Image[] iconsLife;
    [SerializeField] private TextMeshProUGUI textPoints;

    [Header("Бонусы")]
    [SerializeField] private float paddleSizeMultiplier = 1.5f; // Множитель для расширения платформы
    [SerializeField] private float bonusDuration = 10f;         // Длительность эффекта бонуса

    private int currentLives;
    private int points = 0;
    private GameObject currentBall;

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
        currentLives = totalLives;
        UpdateLifeUI();
        SpawnBall();
    }

    public void SpawnBall()
    {
        if (currentBall != null)
        {
            Debug.LogWarning("Попытка заспавнить мяч, но текущий мяч уже существует!");
            return;
        }

        if (ballPrefab == null)
        {
            Debug.LogError("ballPrefab не назначен в GameManager!");
            return;
        }
        if (ballSpawnPoint == null)
        {
            Debug.LogError("ballSpawnPoint не назначен в GameManager!");
            return;
        }

        currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        Debug.Log("Заспавнен новый мяч: " + currentBall.name);
    }

    // Этот метод вызывается из BallController, когда мяч теряется
    public void OnBallLost()
    {
        currentBall = null;
        currentLives--;
        UpdateLifeUI();
        Debug.Log("Жизней осталось: " + currentLives);

        if (currentLives > 0)
        {
            Invoke(nameof(SpawnBall), respawnDelay);
        }
        else
        {
            Debug.Log("Game Over!");
            // Вместо перезапуска активной сцены загружаем сцену GameOver
            SceneManager.LoadScene("Scenes/GameOver");
        }
    }

    // Метод для увеличения очков
    public void IncreasePoints(int amount)
    {
        points += amount;

    }

    // Метод для обновления UI жизней
    private void UpdateLifeUI()
    {
        for (int i = 0; i < iconsLife.Length; i++)
        {
            if (i < currentLives)
                iconsLife[i].gameObject.SetActive(true);
            else
                iconsLife[i].gameObject.SetActive(false);
        }
    }

    // Метод для обновления UI очков


    // Метод для применения бонусов
    public void ApplyBonus(BonusType bonusType, float multiplier, float duration)
    {
        switch (bonusType)
        {
            case BonusType.ExpandPaddle:
                StartCoroutine(ExpandPaddle());
                break;
            case BonusType.ShrinkPaddle:
                StartCoroutine(ShrinkPaddle());
                break;
            case BonusType.ExtraLife:
                GainExtraLife();
                break;
            case BonusType.SlowBall:
                StartCoroutine(SlowBall());
                break;
            case BonusType.FastBall:
                StartCoroutine(FastBall());
                break;
            default:
                Debug.LogWarning("Неизвестный тип бонуса.");
                break;
        }
    }

    // Реализация бонуса "Расширение платформы"
    private IEnumerator ExpandPaddle()
    {
        PlayerController.Instance.ExpandPaddle(paddleSizeMultiplier);
        yield return new WaitForSeconds(bonusDuration);
        PlayerController.Instance.ResetPaddleSize();
    }

    // Реализация бонуса "Сжатие платформы"
    private IEnumerator ShrinkPaddle()
    {
        PlayerController.Instance.ShrinkPaddle();
        yield return new WaitForSeconds(bonusDuration);
        PlayerController.Instance.ResetPaddleSize();
    }

    // Реализация бонуса "Дополнительная жизнь"
    private void GainExtraLife()
    {
        currentLives++;
        UpdateLifeUI();
        Debug.Log("Получена дополнительная жизнь! Жизней: " + currentLives);
    }

    // Реализация бонуса "Замедление мяча"
    private IEnumerator SlowBall()
    {
        BallController.Instance.SetBallSpeedMultiplier(0.5f);
        yield return new WaitForSeconds(bonusDuration);
        BallController.Instance.ResetBallSpeed();
    }

    // Реализация бонуса "Ускорение мяча"
    private IEnumerator FastBall()
    {
        BallController.Instance.SetBallSpeedMultiplier(1.5f);
        yield return new WaitForSeconds(bonusDuration);
        BallController.Instance.ResetBallSpeed();
    }
}
