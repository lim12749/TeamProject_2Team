using UnityEngine;

[DisallowMultipleComponent]
public class CharacterIdentity : MonoBehaviour
{
    public int associatedPlatformIndex;  // 이 캐릭터와 연관된 발판의 인덱스

    void OnMouseDown()
    {
        // 캐릭터 선택 시, CharacterSelectionManager에 발판 인덱스를 전달
        CharacterSelectionManager.Instance.SelectPlatform(associatedPlatformIndex);
    }
}
