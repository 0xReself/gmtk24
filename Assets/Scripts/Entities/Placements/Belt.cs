using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Belt : Placeable {

    [SerializeField]
    private MapManager mapManager = null;

    [SerializeField]
    private List<Sprite> topLayers;

    [SerializeField]
    private bool openEnd = false;

    void Start() {
        mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
    }

    private (Vector2Int, Vector2Int) GetCheckOffsets() {
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
        if (animate) {
            HandleSpawnAnimation();
        }

        (Vector2Int start, Vector2Int end) = GetCheckOffsets();
        bool startTile = true;
        GameObject startObject = mapManager.Get(startPosition + start);
        if (startObject != null) {
            if (openEnd) {
                startTile = false;
            }
            if(startObject.GetComponent<Belt>() != null && startObject.GetComponent<Belt>().rotation == rotation) {
                startTile = false;
            }  
        }
        
        bool endTile = true;
        GameObject endObject = mapManager.Get(startPosition + end);
        if (endObject != null) {
            if (openEnd) {
                endTile = false;
            }
            if(endObject.GetComponent<Belt>() != null && endObject.GetComponent<Belt>().rotation == rotation) {
                endTile = false;
            }  
        }

        if (!startTile && !endTile) {
            topLayer.GetComponent<SpriteRenderer>().sprite = topLayers[2];
        } else if (startTile && endTile) {
            topLayer.GetComponent<SpriteRenderer>().sprite = topLayers[0];
        } else if (startTile) {
            topLayer.GetComponent<SpriteRenderer>().sprite = topLayers[1];
        } else {
            topLayer.GetComponent<SpriteRenderer>().sprite = topLayers[3];
        }
    }
}
