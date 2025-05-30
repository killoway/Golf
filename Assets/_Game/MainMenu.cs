using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Загрузка сцены по индексу (следующая)
    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); // Лучше по имени, чем по индексу
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed");
        Application.Quit();
    }
}
