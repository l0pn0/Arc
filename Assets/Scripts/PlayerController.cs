using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("��������� �������� ���������")]
    [SerializeField] private float moveSpeed = 10f; // �������� �������� ���������
    private float minX; // ����� ������� ���������
    private float maxX; // ������ ������� ���������
    private float fixedY; // ��������������� ������� ��������� �� ��� Y

    [Header("��������� ��������� ����")]
    [SerializeField] private float maxBounceAngle = 75f; // ������������ ���������� ���� (� ��������) �� ���������

    [Header("��������� ��������� ������� ���������")]
    [SerializeField] private float expandedMultiplier = 1.5f; // ��������� ��� ����������
    [SerializeField] private float shrunkMultiplier = 0.75f;  // ��������� ��� ������
    private Vector3 originalScale; // �������� ������ ���������

    private void Awake()
    {
        // ���������� �������� Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // ����������� ������� �� Y
        fixedY = transform.position.y;
        // �������� �������� �������
        originalScale = transform.localScale;

        // ���������� ������� ������ ��� �������� ���������
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
    /// ���������� ��������� ��������� ����� �� ���������� ���� �� ��� X.
    /// </summary>
    private void HandleMovement()
    {
        // �������� ������� ���� � �������� ����������� � ����������� � �������
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // ������������ �� X � �������� ������
        float targetX = Mathf.Clamp(worldPosition.x, minX, maxX);

        // ������� ����������� ���������
        float smoothX = Mathf.MoveTowards(transform.position.x, targetX, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothX, fixedY, transform.position.z);
    }

    /// <summary>
    /// ������ ������������ ��������� � �����, ����������� �������� ���� �������.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���� ��������� ������������ � ��������, ������� ��� "Ball"
        if (collision.gameObject.CompareTag("Ball"))
        {
            // �������� Rigidbody ����
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                // ���������� ������ ���������� ����� ������������
                ContactPoint2D contact = collision.contacts[0];
                // ��������� �������� ����� �������� ������������ ������ ��������� �� ��� X
                float offset = contact.point.x - transform.position.x;
                // ���������� �������� ������ ���������
                float halfWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
                // ����������� �������� � ��������� �� -1 �� 1
                float normalizedOffset = Mathf.Clamp(offset / halfWidth, -1f, 1f);

                // ��� ����� � ����� (normalizedOffset == 0) ���� ����� 90�, ��� ����� �� ����� � ����������� �� maxBounceAngle ��������
                float bounceAngle = 90f - normalizedOffset * maxBounceAngle; // ��� ������ �������� �����, ��� ���� ������ (�����)
                float rad = bounceAngle * Mathf.Deg2Rad;
                // ��������� ����� �����������: �� ��� X � �������, �� ��� Y � �����
                Vector2 newDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

                // ��������� ������� �������� ����
                float speed = ballRb.velocity.magnitude;
                // ����������� ���� ����� ����������� � ����������� ������ ��������
                ballRb.velocity = newDirection * speed;
            }
        }
    }

    /// <summary>
    /// ���������� ��������� (��������� ���������, ���� ��������� �������������� ��������).
    /// </summary>
    /// <param name="multiplier">��������� ��� ����������.</param>
    public void ExpandPaddle(float multiplier)
    {
        transform.localScale = originalScale * multiplier;
        Debug.Log("��������� ���������.");
    }
    
    /// <summary>
    /// ������ ���������.
    /// </summary>
    public void ShrinkPaddle()
    {
        transform.localScale = originalScale * shrunkMultiplier;
        Debug.Log("��������� �����.");
    }

    /// <summary>
    /// ����� ������� ��������� � ���������.
    /// </summary>
    public void ResetPaddleSize()
    {
        transform.localScale = originalScale;
        Debug.Log("��������� ������� �������� ������.");
    }
}


