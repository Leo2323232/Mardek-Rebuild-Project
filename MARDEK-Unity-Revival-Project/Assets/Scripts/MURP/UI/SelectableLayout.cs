using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MURP.UI
{
    [RequireComponent(typeof(GridLayoutGroup), typeof(InputReader))]
    public class SelectableLayout : MonoBehaviour
    {
        int index = 0;
        int Index
        {
            get
            {
                return (index + Selectables.Count) % Selectables.Count;
            }
            set
            {
                index = (value + Selectables.Count) % Selectables.Count;
            }
        }

        Selectable[] selectables;
        List<Selectable> Selectables
        {
            get
            {
                List<Selectable> returnList = new List<Selectable>();
                foreach (var s in selectables)
                    if (s.gameObject.activeSelf)
                        returnList.Add(s);
                return returnList;
            }
        }
        Selectable currentlySelected = null;
        
        GridLayoutGroup layout;
        InputReader input;

        private void OnValidate()
        {
            selectables = GetComponentsInChildren<Selectable>();
            layout = GetComponent<GridLayoutGroup>();
            input = GetComponent<InputReader>();
        }

        private void OnEnable()
        {
            UpdateSelectionAtIndex(false);
            input.enabled = true;
        }

        private void OnDisable()
        {
            input.enabled = false;            
        }

        void UpdateSelectionAtIndex(bool playSFX = true)
        {
            if (currentlySelected)
                currentlySelected.Deselect();
            currentlySelected = Selectables[Index];
            currentlySelected.Select(playSFX);
        }

        public void HandleMovementInput(InputAction.CallbackContext ctx)
        {
            if (enabled == false)
                return;
            var value = ctx.ReadValue<Vector2>();
            if (value.x == 0)
                HandleVerticalInput(value.y);
            if (value.y == 0)
                HandleHorizontalInput(value.x);
        }
        void HandleVerticalInput(float value)
        {
            if (layout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
                if (layout.constraintCount == 1)
                    return;
            if (value > 0)
                Index--;
            else
                Index++;
            UpdateSelectionAtIndex();
        }
        void HandleHorizontalInput(float value)
        {
            if (layout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                if (layout.constraintCount == 1)
                    return;
            if (value > 0)
                Index++;
            else
                Index--;
            UpdateSelectionAtIndex();
        }
    }
}