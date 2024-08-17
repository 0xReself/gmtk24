using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt : Placeable {

    [SerializeField]
    private MapManager mapManager = null;

    [SerializeField]
    private List<Texture2D> topLayers;

    void Start() {
        mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }

    private (Vector2Int, Vector2Int) CheckOffsets() {
        switch(rotation) {
            case 1:
                return (new Vector2Int(0, 1), new Vector2Int(0, -1));
            case 2:
                return (new Vector2Int(1, 0), new Vector2Int(-1, 0));
            case 3:
                return (new Vector2Int(0, -1), new Vector2Int(0, 1));
            default:
                return (new Vector2Int(-1, 0), new Vector2Int(1, 0));
        }
    }

    void Update() {

    }
}
