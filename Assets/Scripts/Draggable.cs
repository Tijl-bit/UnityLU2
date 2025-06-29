//using System.Collections;
//using UnityEngine;
//using UnityEngine.Networking;

//public class Draggable : MonoBehaviour
//{
//    private bool isDragging = false;
//    private Vector3 offset;


//    private string objectId = "yourObjectId"; // The object's ID, set this to your actual object ID

//    void OnMouseDown()
//    {
//        isDragging = true;
//        offset = transform.position - GetMouseWorldPosition();
//    }

//    void OnMouseUp()
//    {
//        isDragging = false;

//    }

//    void Update()
//    {
//        if (isDragging)
//        {
//            transform.position = GetMouseWorldPosition() + offset;
//        }
//    }

//    private Vector3 GetMouseWorldPosition()
//    {
//        Vector3 mousePosition = Input.mousePosition;
//        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
//        return Camera.main.ScreenToWorldPoint(mousePosition);
//    }
//}




