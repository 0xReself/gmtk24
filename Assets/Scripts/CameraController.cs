using UnityEngine;

public class CameraController : MonoBehaviour {
    private Vector3 lastPos = Vector3.zero;

    [SerializeField]
    private GameObject gridCursor;

    [SerializeField]
    private float gridCursorSpeed = 5;

    void Start() {
        if (this.GetComponent<Camera>() == null) {
            Debug.LogError("CameraController not on Gameobject with Camera");
        }
    }

    void Update() {
        HandleMouseInput();
        HandleGridCursor();
    }

    private Vector3  GetMousePositionInWorldSpace() {
        Vector3 worldPosition = this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

        return new Vector3(worldPosition.x, worldPosition.y);
    }

    private void HandleGridCursor() {
        Vector3 currentGridPos = GetCenterGridPosition();
        Vector3 difference = currentGridPos - new Vector3(gridCursor.transform.position.x, gridCursor.transform.position.y);

        gridCursor.transform.position += gridCursorSpeed * Time.deltaTime * difference;
    }

    private void HandleMouseInput() {
        if (Input.GetKey(KeyCode.Mouse1)) {
            Vector3 difference = GetMousePositionInWorldSpace() - lastPos;
            lastPos = GetMousePositionInWorldSpace();

            this.transform.position -= difference;
        }

        lastPos = GetMousePositionInWorldSpace();   
    }

    public Vector3 GetGridPosition() {
        return Vector3Int.FloorToInt(GetMousePositionInWorldSpace());
    }

    public Vector3 GetCenterGridPosition() {
        return Vector3Int.FloorToInt(GetMousePositionInWorldSpace()) + new Vector3(0.5f, 0.5f);
    }
}
