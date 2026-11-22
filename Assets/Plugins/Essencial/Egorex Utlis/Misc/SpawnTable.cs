using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
[InlineEditor]
public class SpawnTable : ScriptableObject{
    [SerializeField] List<ObjectWithValue<Object>> gameObjects;
    [SerializeField] bool allowEmpty = false;

    [ShowInInspector]
    [ReadOnly]
    public float TotalWeight
    {
        get
        {
            if (gameObjects == null) return 0f;
            float total = 0f;
            foreach (var item in gameObjects)
            {
                if (item != null)
                {
                    total += Mathf.Max(item.value, 0f);
                }
            }
            return total;
        }
    }

    public GameObject GetGameObject(){
        return GetAsset<GameObject>();
    }
    
    public List<T> GetAssets<T>() where T : Object{
        var assets = new List<T>();
        var visited = new HashSet<SpawnTable>{ this };
        var toProcess = new Queue<SpawnTable>();
        toProcess.Enqueue(this);
        while (toProcess.Count > 0){
            foreach (var item in toProcess.Dequeue().gameObjects){
                if (item.Object is SpawnTable nestedTable){
                    if (visited.Add(nestedTable)){
                        toProcess.Enqueue(nestedTable);
                    }
                }
                else if (item.Object is T castedObj && !assets.Contains(castedObj)){
                    assets.Add(castedObj);
                }
            }
        }
        return assets;
    }

    public T GetAsset<T>() where T : Object{
        if (gameObjects.Count == 0){
            return null;
        }
        var rolledObj = gameObjects.GetWeightedRoll();
        if (rolledObj.Object is T castedObj){
            return castedObj;
        }
        if (rolledObj.Object is SpawnTable nestedTable){
            return nestedTable.GetAsset<T>();
        }
        if (!allowEmpty){
            Debug.LogError($"Object is not {typeof(T).Name} or another SpawnTable in {name}", this);
        }
        return null;
    }

// #if UNITY_EDITOR
    // [ShowInInspector][ReadOnly] List<GameObject> possibleGameObjects = new();
    // [ShowInInspector][ReadOnly] List<SpawnTable> referencedByTables = new();
//     void OnValidate()
//     {
//         possibleGameObjects.Clear();
//         Queue<ObjectWithValue<Object>> toProcess = new(gameObjects);
//         foreach (var objectWithValue in toProcess){
//             if (objectWithValue.Object is not SpawnTable table){
//                 return;
//             }
//             if (!table.referencedByTables.Contains(this)){
//                 table.referencedByTables.Add(this);
//             }
//         }
//         while (toProcess.Count > 0){
//             var item = toProcess.Dequeue();
//             switch (item.Object){
//                 case SpawnTable getGameObject:
//                     foreach (var obj in getGameObject.gameObjects){
//                         toProcess.Enqueue(obj);
//                     }
//                     break;
//                 case GameObject gameObj:
//                     if (!possibleGameObjects.Contains(gameObj)){
//                         possibleGameObjects.Add(gameObj);
//                     }
//                     break;
//             }
//         }
//         
//         foreach (var table in referencedByTables.Copy()){
//             if (table.gameObjects.All(x => x.Object != this)){
//                 referencedByTables.Remove(table);
//             }
//         }
//     }
// #endif
}