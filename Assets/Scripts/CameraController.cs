using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 lastPos = Vector3.zero;

    void Start() {
        if (this.GetComponent<Camera>() == null) {
            Debug.LogError("CameraController not on Gameobject with Camera");
        }
    }

    void Update() {
        HandleMouseInput();
    }

    private Vector3 GetMousePositionInWorldSpace() {
        return this.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
    }

    private void HandleMouseInput() {
        if (Input.GetKey(KeyCode.Mouse1)) {
            Vector3 difference = GetMousePositionInWorldSpace() - lastPos;
            lastPos = GetMousePositionInWorldSpace();

            this.transform.position -= difference;
        }

        lastPos = GetMousePositionInWorldSpace();   
    }
}
