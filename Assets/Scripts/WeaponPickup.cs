using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    [Header("Hand Offsets (local)")]
    public Vector3 localPosition;      // 손에 붙었을 때 위치
    public Vector3 localEulerAngles;   // 손에 붙었을 때 회전

    [SerializeField] string prompt = "Pick up";
    public string   Prompt    => prompt;
    public Transform Transform => transform;

    public void Interact(PickupInteractor interactor)
    {
        var handler = interactor ? interactor.WeaponHandler : null;
        if (handler == null || !handler.CanEquip) return;

        handler.EquipWorldWeapon(gameObject, localPosition, localEulerAngles);
    }
}
