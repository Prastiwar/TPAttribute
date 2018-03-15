﻿using System.Collections.Generic;
using UnityEngine;

namespace TP.Utilities
{
    public enum ModifierType
    {
        Flat,
        FlatMultiply,
        Percentage
    }

    [System.Serializable]
    public struct TPModifier
    {
        public ModifierType Type;
        public float Value;
        public int Priority;
        public object Source;

        public TPModifier(ModifierType modifierType, float modifierValue, int modifierPriority, object modifierSource)
        {
            Type = modifierType;
            Value = modifierValue;
            Priority = modifierPriority;
            Source = modifierSource;
        }

        public TPModifier(ModifierType modifierType, float modifierValue, int modifierPriority) : this(modifierType, modifierValue, modifierPriority, null) { }
        public TPModifier(ModifierType modifierType, float modifierValue, object modifierSource) : this(modifierType, modifierValue, 0, modifierSource) { }
        public TPModifier(ModifierType modifierType, float modifierValue) : this(modifierType, modifierValue, 0, null) { }
    }

    [System.Serializable]
    public class TPAttribute
    {
        /// <summary>
        /// It's start, base value, without any modifiers
        /// </summary>
        public float BaseValue { get; set; }
        /// <summary>
        /// It's calculated value, with all modifiers in list
        /// </summary>
        public float Value { get; set; }

        public delegate void OnChangeHandler();
        /// <summary>
        /// It's delegate, called after value has changed
        /// </summary>
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

        readonly List<TPModifier> _modifiers = new List<TPModifier>();

        /// <summary>
        /// Adds struct TPModifier to list of modifiers, sorts modifiers, recalculates Value
        /// </summary>
        /// <param name="modifier"></param>
        public void AddModifier(TPModifier modifier)
        {
            _modifiers.Add(modifier);
            _modifiers.Sort(CompareModifiers);
            Recalculate();
        }

        /// <summary>
        /// Removes struct TPModifier from list of modifiers, sorts modifiers, recalculates Value
        /// </summary>
        /// <param name="modifier"></param>
        public void RemoveModifier(TPModifier modifier)
        {
            _modifiers.Remove(modifier);
            Recalculate();
        }

        /// <summary>
        /// Removes all modifiers from attribute, recalculates Value
        /// </summary>
        public void RemoveAllModifiers()
        {
            _modifiers.Clear();
            Recalculate();
        }

        /// <summary>
        /// Removes all modifiers from attribute by source object, recalculates Value
        /// </summary>
        /// <param name="source">object declared in constructor of TPModifier</param>
        public void RemoveAllModifiers(object source)
        {
            foreach (var modifier in _modifiers)
            {
                if(modifier.Source == source)
                {
                    _modifiers.Remove(modifier);
                }
            }
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
                switch (modifier.Type)
                {
                    case ModifierType.Flat:
                        Value += modifier.Value;
                        break;
                    case ModifierType.FlatMultiply:
                        Value *= modifier.Value;
                        break;
                    case ModifierType.Percentage:
                        Value *= (1 + modifier.Value);
                        break;
                }
            }
            OnChanged();
        }

    }

}
