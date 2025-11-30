using UnityEngine;

public class CharacterIdentity : MonoBehaviour
{
    public int associatedPlatformIndex;

    void OnMouseDown()
    {
        if (CharacterSelectionManager.Instance == null)
            return;

        if (CharacterSelectionManager.Instance.platforms == null)
            return;

        CharacterSelectionManager.Instance.SelectPlatform(associatedPlatformIndex);
    }
}
