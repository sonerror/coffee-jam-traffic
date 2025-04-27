namespace UnityEngine.EventSystems
{
#pragma warning disable CS0436 // Type conflicts with imported type
    public interface IPointerDownHandler : IEventSystemHandler
#pragma warning restore CS0436 // Type conflicts with imported type
    {
        void OnPointerDown(PointerEventData eventData);
    }
}
