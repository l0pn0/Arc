using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("��������� ����")]
    [SerializeField] private int totalLives = 3;           // ����� ���������� ������
    [SerializeField] private Transform ballSpawnPoint;     // ����� ������ ����
    [SerializeField] private GameObject ballPrefab;        // ������ ����
    [SerializeField] private float respawnDelay = 1f;      // �������� ����� ������� ������ ����

    [Header("UI Elements")]
    [SerializeField] private Image[] iconsLife;
    [SerializeField] private TextMeshProUGUI textPoints;

    [Header("������")]
    [SerializeField] private float paddleSizeMultiplier = 1.5f; // ��������� ��� ���������� ���������
    [SerializeField] private float bonusDuration = 10f;         // ������������ ������� ������

    private int currentLives;
    private int points = 0;
    private GameObject currentBall;

    private void Awake()
    {
        // ���������� Singleton'�
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
            Debug.LogWarning("������� ���������� ���, �� ������� ��� ��� ����������!");
            return;
        }

        if (ballPrefab == null)
        {
            Debug.LogError("ballPrefab �� �������� � GameManager!");
            return;
        }
        if (ballSpawnPoint == null)
        {
            Debug.LogError("ballSpawnPoint �� �������� � GameManager!");
            return;
        }

        currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        Debug.Log("��������� ����� ���: " + currentBall.name);
    }

    // ���� ����� ���������� �� BallController, ����� ��� ��������
    public void OnBallLost()
    {
        currentBall = null;
        currentLives--;
        UpdateLifeUI();
        Debug.Log("������ ��������: " + currentLives);

        if (currentLives > 0)
        {
            Invoke(nameof(SpawnBall), respawnDelay);
        }
        else
        {
            Debug.Log("Game Over!");
            // ������ ����������� �������� ����� ��������� ����� GameOver
            SceneManager.LoadScene("Scenes/GameOver");
        }
    }

    // ����� ��� ���������� �����
    public void IncreasePoints(int amount)
    {
        points += amount;

    }

    // ����� ��� ���������� UI ������
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

    // ����� ��� ���������� UI �����


    // ����� ��� ���������� �������
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
                Debug.LogWarning("����������� ��� ������.");
                break;
        }
    }

    // ���������� ������ "���������� ���������"
    private IEnumerator ExpandPaddle()
    {
        PlayerController.Instance.ExpandPaddle(paddleSizeMultiplier);
        yield return new WaitForSeconds(bonusDuration);
        PlayerController.Instance.ResetPaddleSize();
    }

    // ���������� ������ "������ ���������"
    private IEnumerator ShrinkPaddle()
    {
        PlayerController.Instance.ShrinkPaddle();
        yield return new WaitForSeconds(bonusDuration);
        PlayerController.Instance.ResetPaddleSize();
    }

    // ���������� ������ "�������������� �����"
    private void GainExtraLife()
    {
        currentLives++;
        UpdateLifeUI();
        Debug.Log("�������� �������������� �����! ������: " + currentLives);
    }

    // ���������� ������ "���������� ����"
    private IEnumerator SlowBall()
    {
        BallController.Instance.SetBallSpeedMultiplier(0.5f);
        yield return new WaitForSeconds(bonusDuration);
        BallController.Instance.ResetBallSpeed();
    }

    // ���������� ������ "��������� ����"
    private IEnumerator FastBall()
    {
        BallController.Instance.SetBallSpeedMultiplier(1.5f);
        yield return new WaitForSeconds(bonusDuration);
        BallController.Instance.ResetBallSpeed();
    }
}
