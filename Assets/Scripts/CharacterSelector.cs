using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class CharacterSelector : MonoBehaviour
{
    public CharacterSelector[] chars;
    public DataManager.Character character;
    public Renderer floorRenderer;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.red;
    public float normalScale = 1f;
    public float selectedScale = 1.2f;
    public float transitionSpeed = 5f;

    private Vector3 targetScale;

    void Start()
    {
        for(int i = 0; i < chars.Length; i++)
        {
            chars[i].OnDeselect();
        }
    }

    private void OnMouseUpAsButton()
    {
        DataManager.Instance.CurrentCharacter = character;
        OnSelect();
        for(int i = 0; i < chars.Length; i++)
        {
            if (chars[i] != this)
            {
                chars[i].OnDeselect();
            }
        }
    }

    public void OnDeselect()
    {
        // 메인 게임 씬이면 색상/스케일 변경 막기
        if (SceneManager.GetActiveScene().name == "MainGameScene")
            return;

        floorRenderer.material.color = normalColor;
        targetScale = Vector3.one * normalScale;
    }

    public void OnSelect()
    {
        // 메인 게임 씬이면 색상/스케일 변경 막기
        if (SceneManager.GetActiveScene().name == "MainGameScene")
            return;

        floorRenderer.material.color = selectedColor;
        targetScale = Vector3.one * selectedScale;
    }
}
