using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PickupInteractor : MonoBehaviour
{
    [Header("Overlap Settings")]
    [SerializeField] Transform center;               // 기준점(보통 플레이어/카메라)
    [SerializeField] float radius = 2.5f;            // 줍기 반경
    [SerializeField] LayerMask interactMask = ~0;    // 인터랙터 레이어 (플레이어 레이어 제외 권장)
    [SerializeField] bool requireLineOfSight = false;// 가시선 필요 시 true
    [SerializeField] LayerMask losMask = ~0;         // 가시선 체크용(벽/지형 등)

    [Header("Targeting Bias (optional)")]
    [SerializeField] Camera cam;                     // 정중앙 우선 선택을 위한 카메라(없으면 거리만)
    [SerializeField, Range(0f, 2f)] float viewBias = 0.25f; // 시선 각도 가중치 (0=거리만)

    [Header("UI (Screen Overlay)")]
    [SerializeField] GameObject promptRoot;          // "E: 줍기" 패널
    [SerializeField] TextMeshProUGUI promptText;

    [Header("Refs")]
    [SerializeField] WeaponHandler weaponHandler;    // 선택

    // runtime
    IInteractable current;

    [Header("Debug")]
    [SerializeField] bool drawGizmos = true;
    [SerializeField] Color sphereColor = new Color(1f, 1f, 0f, 0.25f);
    [SerializeField] Color currentLineColor = Color.green;

    public WeaponHandler WeaponHandler => weaponHandler;

    void Awake()
    {
        if (!center) center = transform;
        if (!cam) cam = Camera.main;
        if (!weaponHandler) weaponHandler = GetComponentInChildren<WeaponHandler>();
        ShowPrompt(false, null);
    }

    void Update()
    {
        current = null;
        if (!center) return;

        Vector3 cpos = center.position;

        // 반경 내 모든 후보 수집
        Collider[] hits = Physics.OverlapSphere(cpos, radius, interactMask, QueryTriggerInteraction.Collide);

        float bestScore = float.MaxValue;
        foreach (var h in hits)
        {
            var interactable = h.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            // 가시선 필요하면 라인캐스트로 차단 체크
            if (requireLineOfSight && cam)
            {
                Vector3 to = interactable.Transform.position;
                if (Physics.Linecast(cam.transform.position, to, out var blockHit, losMask, QueryTriggerInteraction.Ignore))
                {
                    // 라인캐스트가 뭔가를 맞췄고, 그게 interactable 자체가 아니면 차단된 것으로 간주
                    if (blockHit.collider.GetComponentInParent<IInteractable>() != interactable)
                        continue;
                }
            }

            // 스코어: 거리 + (시선 각도 * 가중치)
            Vector3 toTarget = interactable.Transform.position - cpos;
            float dist2 = toTarget.sqrMagnitude;

            float score = dist2;
            if (cam && viewBias > 0f && toTarget.sqrMagnitude > 0.0001f)
            {
                float angle = Vector3.Angle(cam.transform.forward, toTarget.normalized); // 0~180
                score += angle * viewBias;
            }

            if (score < bestScore)
            {
                bestScore = score;
                current = interactable;
            }
        }

        if (current != null) ShowPrompt(true, current.Prompt);
        else                 ShowPrompt(false, null);
    }

    // Send Messages 방식: 액션 이름이 "Interact"여야 함
    public void OnInteract(InputValue v)
    {
        if (!v.isPressed) return;
        current?.Interact(this);
    }

    void ShowPrompt(bool show, string text)
    {
        if (promptRoot) promptRoot.SetActive(show);
        if (show && promptText) promptText.text = string.IsNullOrEmpty(text) ? "E: Interact" : text;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        var at = center ? center.position : transform.position;

        // 반경 시각화
        Gizmos.color = sphereColor;
        Gizmos.DrawWireSphere(at, radius);

        // 현재 타겟 표시
        if (current != null)
        {
            Gizmos.color = currentLineColor;
            Gizmos.DrawLine(at, current.Transform.position);
        }
    }
}
