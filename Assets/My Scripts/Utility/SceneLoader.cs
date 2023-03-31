#region
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

public class SceneLoader : MonoBehaviour
{
    [SerializeField] float delayInSeconds = 2f;
    public float DelayInSeconds { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) Invoke(nameof(LoadGameOver), delayInSeconds);
    }

    /// <summary>
    ///     Loads the main menu screen (scene 0).
    /// </summary>
    public void LoadMainMenu() { SceneManager.LoadScene(0); }

    /// <summary>
    ///     Loads a scene based on the scene number.
    /// </summary>
    /// <param name="sceneNumber">The scene number to load.</param>
    public void LoadGame(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    /// <summary>
    ///     Loads the end screen.
    /// </summary>
    /// <param name="delay">The delay before loading the scene.</param>
    public async void LoadGameOver(float delay)
    {
        await Task.Delay(Mathf.RoundToInt(delay * 1000));

        // value "2" is the end screen.
        LoadGame(2);
    }

    /// <summary>
    ///     Reloads the current scene after a delay.
    /// </summary>
    public void ReloadScene() { LoadGame(SceneManager.GetActiveScene().buildIndex); }

    // Load scene 2 (end screen) after a delay.
    public void LoadGameOver() { LoadGameOver(delayInSeconds); }

    /// <summary>
    ///     Loads the quit screen.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game!");
        Application.Quit();
    }
}