using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectRoot<T> : MonoBehaviour where T : MonoBehaviour{
    readonly Dictionary<Type, object> _componentsCache = new();

    public event Action onStructureChanged;

    public T1 GetPart<T1>() where T1 : T{
        var componentsList = GetParts<T1>();
        if (componentsList.Count == 0){
            return null;
        }
        if (componentsList.Count > 1){
            Debug.LogWarning(
                $"Multiple components of type {typeof(T1)} found on {gameObject.name}. Returning the first one.");
        }
        return componentsList[0];
    }

    public virtual IReadOnlyList<T1> GetParts<T1>() where T1 : T{
        var type = typeof(T1);
        if (_componentsCache.TryGetValue(type, out var cachedObject)){
            return (List<T1>)cachedObject;
        }

        var fetchedComponents = GetComponentsInChildren<T1>(true);
        var newList = new List<T1>(fetchedComponents);

        _componentsCache[type] = newList;

        return newList;
    }

    public void OnStructureChanged(){
        _componentsCache.Clear();
        onStructureChanged?.Invoke();
    }

    public T1 AddPart<T1>() where T1 : T{
        var part = gameObject.AddComponent<T1>();
        OnPartAdded(part);
        OnStructureChanged();
        return part;
    }

    public void RemovePart(T part){
        if (part == null){
            Debug.LogWarning($"Attempted to remove a null part from {gameObject.name}. Operation aborted.");
            return;
        }
        Destroy(part);
        OnStructureChanged();
    }

    protected virtual void OnPartAdded(T part){ }
}

// public interface IObjectRoot<T> where T : class{
//     public T1 GetPart<T1>() where T1 : T;
//     public IReadOnlyList<T1> GetParts<T1>() where T1 : T;
// }