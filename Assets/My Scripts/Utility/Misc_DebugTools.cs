#region
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

public class Misc_DebugTools : MonoBehaviour
{
    Gun gun;

    void Awake() => gun = FindObjectOfType<Gun>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) ReloadScene();
    }

    public void SetFireMode(Gun.FireMode fireMode) => gun.CurrentFireMode = fireMode;

    public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}