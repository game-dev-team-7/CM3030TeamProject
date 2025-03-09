using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     Manages the main menu functionality including game start and quit options.
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    ///     Starts the game by loading the next scene in the build index.
    ///     Called from the Play button in the main menu.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    ///     Quits the application.
    ///     Called from the Quit button in the main menu.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}