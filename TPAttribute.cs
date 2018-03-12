using System.Collections.Generic;
using UnityEngine;

namespace TP.Utilities
{
    //[System.Serializable]
    //public class TPFlatModifier : TPModifier
    //{
    //    public TPFlatModifier(ModifierType modifierType, ModifierCommand modifierCommand, float modifierValue) : base(modifierType, modifierCommand, modifierValue)
    //    {
    //    }
    //}
    [System.Serializable]
    public class TPModifier
    {
        public TPModifier(ModifierType modifierType, ModifierCommand modifierCommand, float modifierValue)
        {
            ModifyType = modifierType;
            ModifyCommand = modifierCommand;
            ModifyValue = modifierValue;
        }
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
        public ModifierType ModifyType;
        public ModifierCommand ModifyCommand;
        public float ModifyValue;

        public void Modify(ref float value, ref float tempValue)
        {
            if (ModifyType == ModifierType.Percentage)
            {
                tempValue = Mathf.CeilToInt((value / ModifyValue) * 100);
            }
            switch (ModifyCommand)
            {
                case ModifierCommand.Increase:
                    value = (ModifyType == ModifierType.Flat) ? (value += ModifyValue) : (value += tempValue);
                    break;

                case ModifierCommand.Decrease:
                    value = (ModifyType == ModifierType.Flat) ? (value -= ModifyValue) : (value -= tempValue);
                    break;

                case ModifierCommand.Multiply:
                    value = (ModifyType == ModifierType.Flat) ? (value *= ModifyValue) : (value *= tempValue);
                    break;
            }
        }

        public void UnModify(ref float value, float tempValue = 0)
        {
            switch (ModifyCommand)
            {
                case ModifierCommand.Increase:
                    value = ModifyType == ModifierType.Flat ? value -= ModifyValue : value -= tempValue;
                    break;

                case ModifierCommand.Decrease:
                    value = ModifyType == ModifierType.Flat ? value += ModifyValue : value += tempValue;
                    break;

                case ModifierCommand.Multiply:
                    value = ModifyType == ModifierType.Flat ? value /= ModifyValue : value /= tempValue;
                    break;
            }
        }
    }






    public class TPAttribute
    {
        public delegate void OnChangeHandler();

        readonly List<TPModifier> _modifiers = new List<TPModifier>();
        float _value;
        float _maxValue;
        float _baseValue;

        float _tempValue;

        public void AddModifier(TPModifier modifier)
        {
            _modifiers.Add(modifier);
            modifier.Modify(ref _value, ref _tempValue);
            TPModifier mod = new TPModifier(TPModifier.ModifierType.Flat, TPModifier.ModifierCommand.Multiply, 1);
            AddModifier(mod);
            AddModifier(mod);
        }

        public void RemoveModifier(TPModifier modifier)
        {
            _modifiers.Remove(modifier);
            modifier.UnModify(ref _value, _tempValue);
        }

        public void RemoveAllModifiers()
        {
            foreach (var modifier in _modifiers)
            {
                modifier.UnModify(ref _value, _tempValue);
            }
            _modifiers.Clear();
        }

        public OnChangeHandler OnChange
        {
            get
            {
                if (OnChange != null)
                    return OnChange;

                return () => { };
            }
            set { OnChange = value; }
        }

        public float BaseValue
        {
            get
            {
                return _baseValue;
            }
            set
            {
                _baseValue = value;
                OnChange();
            }
        }

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnChange();
            }
        }

        public float MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
            }
        }
        

    }

}
