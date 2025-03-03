using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance;

    [Header("��������� ������")]
    [SerializeField] private float initialSpeed = 10f;
    [SerializeField] private float launchDelay = 0f;

    [Header("��������� ��������� �� ���������")]
    [SerializeField] private float maxBounceAngle = 60f; // ������������ ���������� �� ��������� (� ��������)

    private Rigidbody2D rb;

    [Header("��������� ������ ������")]
    [SerializeField] private float fallThreshold = -6f; // ������ ������� �� ��� Y, ���� ������� ��� ��������� ����������
    private bool hasTriggeredLoss = false;

    private float speedMultiplier = 1f; // ��������� �������� ����

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
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        hasTriggeredLoss = false;
        Debug.Log("����� ��� ���������������: " + gameObject.name);

        if (launchDelay > 0f)
            Invoke(nameof(LaunchBall), launchDelay);
        else
            LaunchBall();
    }

    private void Update()
    {
        // ���� ��� ������ ���� ������, ���������� GameManager � ���������� ���
        if (!hasTriggeredLoss && transform.position.y < fallThreshold)
        {
            hasTriggeredLoss = true;

            // ������������, ��� GameManager ���������� ��� Singleton
            GameManager.Instance.OnBallLost();
            Destroy(gameObject);
        }
    }

    private void LaunchBall()
    {
        // �������� ��������� ���� � ��������� [45�, 135�] (��������� �����)
        float randomAngle = Random.Range(45f, 135f);
        float radAngle = randomAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle)).normalized;

        rb.velocity = direction * initialSpeed * speedMultiplier;
        Debug.Log("��� ������� ��� �����: " + randomAngle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���� ��� ���������� � ���������� (��������������, ��� ��������� �������� ��� "Player")
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform platform = collision.transform;
            SpriteRenderer sr = platform.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                float platformWidth = sr.bounds.size.x;
                // ��������� �������� ����� �� ������ ���������
                float hitOffset = transform.position.x - platform.position.x;
                float normalizedOffset = hitOffset / (platformWidth / 2);

                // ��� ����� �� ������ ��� ����� ����������� ����� (90�), � �� ����� ���� ���������
                float bounceAngle = 90f + normalizedOffset * maxBounceAngle;
                float radBounceAngle = bounceAngle * Mathf.Deg2Rad;

                Vector2 newDirection = new Vector2(Mathf.Cos(radBounceAngle), Mathf.Sin(radBounceAngle)).normalized;
                float currentSpeed = initialSpeed * speedMultiplier;
                rb.velocity = newDirection * currentSpeed;
            }
        }
        // ��������� ������������ (�� �������, �������) �������������� ����������� ������� Unity.
    }

    // ����� ��� ��������� ��������� ��������
    public void SetBallSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
        rb.velocity = rb.velocity.normalized * initialSpeed * speedMultiplier;
        Debug.Log("��������� �������� ���� ���������� ��: " + multiplier);
    }

    // ����� ��� ������ ��������� ��������
    public void ResetBallSpeed()
    {
        speedMultiplier = 1f;
        rb.velocity = rb.velocity.normalized * initialSpeed * speedMultiplier;
        Debug.Log("��������� �������� ���� �������.");
    }
}



