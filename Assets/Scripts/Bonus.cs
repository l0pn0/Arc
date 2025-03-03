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
    [Header("����� ��������� ������")]
    [SerializeField] private float fallSpeed = 3f; // �������� ������� ������

    // ��������� ��� �������, ������� ������� �������������� ��������:
    [SerializeField] private float paddleScaleMultiplier = 1.5f; // ��������� ��� ����������/������ ���������
    [SerializeField] private float bonusDuration = 10f;          // ������������ �������� ������

    // ���������� ���� ��� �������� ���������� ���� ������
    private BonusType bonusType;

    private void Start()
    {
        // ��������� ������� �������� ��� ������ �� ������������
        BonusType[] bonusTypes = (BonusType[])System.Enum.GetValues(typeof(BonusType));
        bonusType = bonusTypes[Random.Range(0, bonusTypes.Length)];
        Debug.Log("�������� ������ �����: " + bonusType);
    }

    private void Update()
    {
        // ������� ����� ����
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        // ������� �����, ���� �� �� ��������� ������ (��������, ���� y = -6)
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ��������� ������������ � ���������� (�������)
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyBonus(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void ApplyBonus(GameObject player)
    {
        // ����� ����� ������� ����� �� GameManager ��� ���������� ������
        // ��������:
        GameManager.Instance.ApplyBonus(bonusType, paddleScaleMultiplier, bonusDuration);

        // ��� �������� �������� ��������� ������, ���� ������ ������� ����������� ���
        Debug.Log("��������� �����: " + bonusType);
    }
}