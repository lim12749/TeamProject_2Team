using UnityEngine;

public class SceneChange : MonoBehaviour
{
    public void OnMouseUpAsButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelectScene");
    }
}
