using UnityEngine;

public class GamePlay : MonoBehaviour
{
    private void OnMouseDown()
    {
        
    }
    void Update()
    {
        // Dành cho c? chu?t và ch?m (touch)
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            RaycastAtPosition(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastAtPosition(Input.GetTouch(0).position);
        }
#endif
    }

    void RaycastAtPosition(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Box"))
            {
                Debug.Log("Ch?m vào object có tag là Box: " + hit.collider.name);
            }
        }
    }
}
