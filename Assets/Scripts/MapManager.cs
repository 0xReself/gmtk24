using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour {

    [SerializeField]
    private GameObject corePrefab;

    [SerializeField]
    private GameObject sinkPrefab;

    [SerializeField]
    private Dictionary<Vector2Int, GameObject> map;

    void Start() {
        map = new Dictionary<Vector2Int, GameObject>();
        //Set Core
        GameObject placedCore = Instantiate(corePrefab, new Vector2(-1.5f, 1.5f), Quaternion.identity);
        Placeable placeableCore = placedCore.GetComponent<Placeable>();
        placeableCore.startPosition = new Vector2Int(-1, 1);

        Set(placedCore, placeableCore.startPosition, placeableCore.GetEndPosition(placeableCore.startPosition));

        //Set Core
        GameObject placedSink = Instantiate(sinkPrefab, new Vector2(30.5f, 2.5f), Quaternion.identity);
        Placeable placeableSink = placedSink.GetComponent<Placeable>();
        placeableSink.startPosition = new Vector2Int(30, 2);

        Set(placedSink, placeableSink.startPosition, placeableSink.GetEndPosition(placeableSink.startPosition));
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
