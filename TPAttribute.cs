﻿using System.Collections.Generic;
using UnityEngine;

namespace TP.Utilities
{
    public enum ModifierType
    {
        Flat,
        Percentage
    }

    public enum ModifierCommand
    {
        Increase,
        Decrease,
        Multiply
    }

    [System.Serializable]
    public class TPModifier
    {
        public ModifierType Type;
        public ModifierCommand Command;
        public float Value;
        public int Priority;

        public TPModifier(ModifierType modifierType, ModifierCommand modifierCommand, float modifierValue, int modifierPriority)
        {
            Type = modifierType;
            Command = modifierCommand;
            Value = modifierValue;
            Priority = modifierPriority;
        }

        public TPModifier(ModifierType modifierType, ModifierCommand modifierCommand, float modifierValue) : this(modifierType, modifierCommand, modifierValue, 0) { }
    }

    [System.Serializable]
    public class TPAttribute
    {
        public float BaseValue { get; set; }
        public float Value { get; set; }
        public float MaxValue { get; set; }

        readonly List<TPModifier> _modifiers = new List<TPModifier>();

        public delegate void OnChangeHandler();

        public OnChangeHandler OnChanged
        {
            get
            {
                if (OnChanged != null)
                    return OnChanged;

                return () => { };
            }
            set { OnChanged = value; }
        }

        public void AddModifier(TPModifier modifier)
        {
            _modifiers.Add(modifier);
            _modifiers.Sort(CompareModifiers);
            Recalculate();
        }

        public void RemoveModifier(TPModifier modifier)
        {
            _modifiers.Remove(modifier);
            Recalculate();
        }

        public void RemoveAllModifiers()
        {
            _modifiers.Clear();
            Recalculate();
        }

        int CompareModifiers(TPModifier modifier1, TPModifier modifier2)
        {
            if (modifier1.Priority > modifier2.Priority)
            {
                return 1;
            }
            else if (modifier1.Priority < modifier2.Priority)
            {
                return -1;
            }
            return 0;
        }

        void Recalculate()
        {
            Value = BaseValue;
            foreach (var modifier in _modifiers)
            {
                var modifyValue = modifier.Type == ModifierType.Flat ? modifier.Value : Mathf.CeilToInt((Value / modifier.Value) * 100);

                switch (modifier.Command)
                {
                    case ModifierCommand.Increase:
                        Value = Value += modifyValue;
                        break;
                    case ModifierCommand.Decrease:
                        Value = Value -= modifyValue;
                        break;
                    case ModifierCommand.Multiply:
                        Value = Value *= modifyValue;
                        break;
                }
            }
            OnChanged();
        }

    }

}
