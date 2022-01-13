using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class KeyRebind : MonoBehaviour
{
    //[SerializeField] PlayerInput control = null;
    [SerializeField] InputActionReference actionReference = null;
    [SerializeField, Range(0, 3)] int compositeBindingIndex = 0;
    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    [SerializeField] Text actionNameText = null;
    [SerializeField] Text bindText = null;
    [SerializeField, HideInInspector] Color originalColor;
    Color rebindingColor = Color.red;

    InputBinding Binding
    {
        get
        {
            if (actionReference.action.bindings[0].isComposite == false)
                return actionReference.action.bindings[0];
            return actionReference.action.bindings[1 + compositeBindingIndex];
        }
    }
    string ActionName
    {
        get
        {
            var name = actionReference.action.name;
            var bindingName = Binding.name;
            if (string.IsNullOrEmpty(bindingName) == false)
                return $"{name} ({bindingName})";
            return name;
        }
    }

    private void OnValidate()
    {
        originalColor = bindText.color;
        actionNameText.text = ActionName;
        gameObject.name = ActionName;
        UpdateBindText();
    }

    void UpdateBindText()
    {
        var bindPath = Binding.effectivePath;
        var options = InputControlPath.HumanReadableStringOptions.None;
        bindText.text = InputControlPath.ToHumanReadableString(bindPath, options);
        bindText.color = originalColor;
    }

    public void Rebind()
    {
        bindText.color = rebindingColor;

        actionReference.action.actionMap.Disable();
        rebindingOperation = actionReference.action
                                .PerformInteractiveRebinding()
                                .WithControlsExcluding("Mouse")
                                .OnMatchWaitForAnother(0.1f)
                                .OnComplete(operation => OnEndRebind())
                                .Start();
    }

    private void OnEndRebind()
    {
        actionReference.action.actionMap.Enable();
        rebindingOperation.Dispose();
        UpdateBindText();
        InputReader.RefreshInputReaders();
    }
}