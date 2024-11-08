using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image _selector;
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void SetSelector(float y)
    {
        _selector.transform.position = new Vector3(_selector.transform.position.x, y, 0);
    }
}
