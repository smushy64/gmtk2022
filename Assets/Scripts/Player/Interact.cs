using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField]
    LayerMask interactLayers;

    UIHud hud;
    PlayerInput input;
    InputAction interactAction;
    bool interact;
    bool lastInteract;

    private void Awake() {
        hud = FindObjectOfType<UIHud>();
        input = GetComponentInParent<PlayerInput>();
        interactAction = input.actions["Interact"];
    }

    private void OnEnable()
    {
        interactAction.started += OnInteract;
        interactAction.canceled += OnInteractEnd;
    }
    private void OnDisable()
    {
        interactAction.started -= OnInteract;
        interactAction.canceled -= OnInteractEnd;
    }

    private void FixedUpdate()
    {
        if(Physics.Raycast(
            transform.position, transform.forward,
            out RaycastHit hitInfo,
            3f, interactLayers,
            QueryTriggerInteraction.Collide
        )) {
            hud.SetPickUpTextEnabled(true);
            if( interact != lastInteract && interact ) {
                hitInfo.transform.GetComponentInParent<IInteractable>().OnInteract();
            }
        } else {
            hud.SetPickUpTextEnabled(false);
        }
        lastInteract = interact;
    }

    void OnInteract( InputAction.CallbackContext ctx ) {
        interact = true;
    }
    void OnInteractEnd( InputAction.CallbackContext ctx ) {
        interact = false;
    }

}
