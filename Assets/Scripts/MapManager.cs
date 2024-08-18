using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour {

    [SerializeField]
    private GameObject corePrefab;

    [SerializeField]
    private Dictionary<Vector2Int, GameObject> map;

    void Start() {
        map = new Dictionary<Vector2Int, GameObject>();
        GameObject placedCore = Instantiate(corePrefab, new Vector2(-1.5f, 1.5f), Quaternion.identity);
        Placeable placeable = placedCore.GetComponent<Placeable>();
        placeable.startPosition = new Vector2Int(-1, 1);

        Set(placedCore, placeable.startPosition, placeable.GetEndPosition(placeable.startPosition));
    }

    void Update() {}

    public bool CanPlace(Vector2Int start, Vector2Int end) {
        for(int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++) {
            for(int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++) {
                if(map.ContainsKey(new Vector2Int(x, y))) {
                    return false;
                }
            }
        }
        return true; 
    }

    public void Set(GameObject gameObject, Vector2Int start, Vector2Int end) {
        for(int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++) {
            for(int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++) {
                map[new Vector2Int(x, y)] = gameObject;
            }
        } 
    }

    public void Remove(Vector2Int start, Vector2Int end) {
        for(int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++) {
            for(int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++) {
                map.Remove(new Vector2Int(x, y));
            }
        } 
    }

    public GameObject Get(Vector2Int position) {
        if (map.ContainsKey(position) == false) {
            return null;
        }

        return map[position];
    }
}
