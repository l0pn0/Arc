using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    // ����� ����������� ����
    public void RestartGame()
    {
        // �������� �������� ������� �����. ���������, ��� ��� ����� ������� ����� (��������, "Game")
        SceneManager.LoadScene("SampleScene");
    }

    // ����� ��� ������ �� ���� ��� �������� � ������� ����
    public void ExitGame()
    {
        // ���� � ��� ���� ��������� ����� �������� ����, ��������� �. ��������:
        // SceneManager.LoadScene("MainMenu");

        // ���� ����� ��������� ���� (�������� � ����-������, � ��������� ������ �� ���������)
        Application.Quit();
    }
}
