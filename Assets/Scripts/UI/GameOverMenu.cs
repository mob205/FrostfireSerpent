using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private int _mainLevel = 1;
    public void RestartLevel()
    {
        SceneManager.LoadScene(_mainLevel);
    }
}
