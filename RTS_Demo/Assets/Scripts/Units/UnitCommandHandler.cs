using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandHandler : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private void Start()
    {
        this.mainCamera = Camera.main;
    }

    private void Update()
    {

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                MoveUnits(hit.point);
            }
        }
    }

    private void MoveUnits(Vector3 point)
    {
        foreach(Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            unit.GetUnitMover().MovementCommand(point);
        }
    }
}
