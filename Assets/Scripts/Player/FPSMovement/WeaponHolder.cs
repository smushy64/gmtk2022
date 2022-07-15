using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Docien.FPSMovement;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject pivot;
    [SerializeField] private HitscanWeapon weapon;

    private PlayerInput input;
    private InputAction fireAction;

    private bool isFiring = false;

    private void Awake()
    {
        input = GetComponentInParent<PlayerInput>();
        fireAction = input.actions["Fire"];
        fireAction.performed += OnStartFire;
        fireAction.canceled += OnStopFire;
    }

    private void OnStartFire(InputAction.CallbackContext ctx) => isFiring = true;
    private void OnStopFire(InputAction.CallbackContext ctx) => isFiring = false;

    private void Update()
    {
        if (isFiring && weapon.CanFire)
        {
            weapon.Fire(cam.transform.position, cam.transform.forward);
        }
    }
}
