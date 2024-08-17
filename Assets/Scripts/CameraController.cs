using UnityEngine;
using UnityEngine.Animations;

public class CameraController : MonoBehaviour {
    private Vector3 lastPos = Vector3.zero;

    [SerializeField]
    private GameObject gridCursor;

    [SerializeField]
    private float gridCursorSpeed = 5;

    [SerializeField]
    private float zoom;
    [SerializeField]
    private float zoomMultiplier = 1;
    [SerializeField]
    private float maxZoom = 8;
    [SerializeField]
    private float minZoom = 2;

    [SerializeField]
    private float velocity = 0;

    [SerializeField]
    private float smoothTime = 0.1f;

    [SerializeField]
    private GameObject backgroundGrid = null;

    void Start() {
        if (this.GetComponent<Camera>() == null) {
            Debug.LogError("CameraController not on Gameobject with Camera");
        }
        zoom = this.GetComponent<Camera>().orthographicSize;
    }

    void Update() {
        HandleMouseInput();
        HandleGridCursor();
        HandleBackgroundOpacity();
    }

    private void HandleBackgroundOpacity() {
        float opacity = 1 - (zoom - minZoom) / (maxZoom - minZoom);
        backgroundGrid.GetComponent<SpriteRenderer>().material.SetFloat("_Opacity", opacity);
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
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        this.GetComponent<Camera>().orthographicSize =  Mathf.SmoothDamp(this.GetComponent<Camera>().orthographicSize, zoom, ref velocity, smoothTime);

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
