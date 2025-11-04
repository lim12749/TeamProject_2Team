using UnityEngine;

public class CopyAnimator : MonoBehaviour
{
    public Animator parentAnimator; // CharacterMain의 Animator
    private Animator myAnimator;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if (parentAnimator == null || myAnimator == null) return;

        // 부모의 현재 상태 가져오기
        AnimatorStateInfo parentState = parentAnimator.GetCurrentAnimatorStateInfo(0);

        // 자식 애니메이터에 같은 상태 적용
        myAnimator.Play(parentState.fullPathHash, 0, parentState.normalizedTime);
    }
}
