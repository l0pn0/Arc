using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("��������� �����")]
    [SerializeField] private int maxHealth = 1; // ������������ �������� �����


    [Header("���������� ��������")]
    [SerializeField] private Sprite[] damageSprites; // ������� ��� ��������� �����
    [SerializeField] private Color[] damageColors;     // ����� ��� ���������

    [Header("��������� �������")]
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
            Debug.Log($"���� {gameObject.name} ������� ����. �������� ������: {currentHealth}");
        }
        else
        {
            DestroyBlock();
        }
    }

    private void UpdateVisuals()
    {
        // ��������� ������ ��������� (0 � ��� �����������)
        int damageIndex = maxHealth - currentHealth;
        damageIndex = Mathf.Clamp(damageIndex, 0, damageSprites.Length - 1);

        if (damageSprites != null && damageSprites.Length > 0)
            spriteRenderer.sprite = damageSprites[damageIndex];

        if (damageColors != null && damageColors.Length > damageIndex)
            spriteRenderer.color = damageColors[damageIndex];
    }

    private void DestroyBlock()
    {
        // ��������� ����� ������
        if (bonusPrefabs.Length > 0 && Random.value <= bonusDropChance)
        {
            int bonusIndex = Random.Range(0, bonusPrefabs.Length);
            Instantiate(bonusPrefabs[bonusIndex], transform.position, Quaternion.identity);
        }
        // ���������� ����� � ����������� �������
        GameManager.Instance.IncreasePoints(10);
        Destroy(gameObject);
    }
}

