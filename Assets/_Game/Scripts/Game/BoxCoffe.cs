using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
#pragma warning disable CS0436 // Type conflicts with imported type
public class BoxCoffe : MonoBehaviour, IPointerDownHandler
#pragma warning restore CS0436 // Type conflicts with imported type
{
    public Vector3 targetPosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Hit");
    }


}
