using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public interface IValue{
    UnityEvent<ValueChange> OnValueChanged{ get; }
    float GetValue();
    Vector2 GetMinMax();
    void SetValue(float newValue);
}

public struct ValueChange{
    public float newValue;
    public float previousValue;
    public float diff => newValue - previousValue;

    public ValueChange(float value, float previousValueTmp){
        newValue = value;
        previousValue = previousValueTmp;
    }
}

[Serializable]
public class Value : IValue{
    public bool log;

    public string name;
    [SerializeField] float value;
    [SerializeField] Vector2 minMax = new(0, 100);

    [FoldoutGroup("Events")] public UnityEvent<ValueChange> onValueChanged = new();

    public UnityEvent<ValueChange> OnValueChanged => onValueChanged;

    public float GetValue(){
        return value;
    }

    public Vector2 GetMinMax(){
        return minMax;
    }

    public void SetValue(float newValue){
        if (log){
            Debug.Log($"Setting {name} value to {newValue}");
        }
        var previousValue = value;
        value = Mathf.Clamp(newValue, minMax.x, minMax.y);
        if (value - previousValue == 0){
            return;
        }
        onValueChanged.Invoke(new ValueChange(value, previousValue));
    }

    public void SetMinMax(Vector2 minMax){
        this.minMax = minMax;
        SetValue(value);
    }
}

public static class ValueExtentions{
    public static void ModifyValue(this Value value, float mod){
        if (value.log){
            Debug.Log($"Modifing {value.name} value by " + mod);
        }
        value.SetValue(value.GetValue() + mod);
    }

    public static void MultiplyValue(this Value value, float mod){
        if (value.log){
            Debug.Log($"Multipling {value.name} value by " + mod);
        }
        value.SetValue(value.GetValue() * mod);
    }

    public static bool HasValue(this Value value, int requiredValue){
        return value.GetValue() >= requiredValue;
    }
}