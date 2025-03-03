using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    // Метод перезапуска игры
    public void RestartGame()
    {
        // Загрузка основной игровой сцены. Убедитесь, что имя сцены указано верно (например, "Game")
        SceneManager.LoadScene("SampleScene");
    }

    // Метод для выхода из игры или возврата в главное меню
    public void ExitGame()
    {
        // Если у вас есть отдельная сцена главного меню, загрузите её. Например:
        // SceneManager.LoadScene("MainMenu");

        // Либо можно завершить игру (работает в билд-версии, в редакторе ничего не произойдёт)
        Application.Quit();
    }
}
