using UnityEngine;
public interface IInteractable
{
    string Prompt { get; }
    Transform Transform { get; }
    void Interact(PickupInteractor interactor);
}
