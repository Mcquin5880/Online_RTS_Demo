using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private RectTransform unitSelectionArea = null;

    private Camera mainCamera;
    private Player player;
    private Vector2 mouseSelectionStartPos;
    private List<Unit> selectedUnits = new List<Unit>();

    private void Start()
    {
        this.mainCamera = Camera.main;
    }

    public List<Unit> GetSelectedUnits()
    {
        return this.selectedUnits;
    }

    private void Update()
    {
        if (player == null)
        {
            this.player = NetworkClient.connection.identity.GetComponent<Player>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }
            selectedUnits.Clear();
        }

        unitSelectionArea.gameObject.SetActive(true);
        mouseSelectionStartPos = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        float width = mousePos.x - mouseSelectionStartPos.x;
        float height = mousePos.y - mouseSelectionStartPos.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        unitSelectionArea.anchoredPosition = mouseSelectionStartPos + new Vector2(width / 2, height / 2);
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

            if (!unit.hasAuthority) { return; }

            selectedUnits.Add(unit);

            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Select();
            }

            return;
        }

        SelectUnitsInMouseSelectionArea();

    }

    private void SelectUnitsInMouseSelectionArea()
    {
        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in player.GetPlayerUnits())
        {
            if (selectedUnits.Contains(unit)) continue;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
            {
                selectedUnits.Add(unit);
                unit.Select();
            }
        }
    }
}
