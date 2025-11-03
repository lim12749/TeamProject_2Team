using UnityEngine;
using UnityEngine.TextCore.Text;
using static DataManager;

public class SelectCharacter : MonoBehaviour
{
    public character character; 

    private void OnMouseUpAsButton()
    {
        DataManager.Instance.CurrentCharacter = character;
    }
}
