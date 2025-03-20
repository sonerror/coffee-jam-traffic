using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.Bus
{
    [SelectionBase]
    public class ParkSlot : MonoBehaviour, IPointerDownHandler
    {
        public new Transform transform;

        public GameObject adsObject;
        public GameObject vipObject;
        public GameObject normalLineObject;

        public Bus AssignedBus { get; set; }
        public bool IsSlotTaken { get; set; }
        public bool IsBusArrive { get; set; }
        public bool IsAdsSlot { get; set; }
        [SerializeField] private bool isVipSlot;
        public bool IsVipSlot => isVipSlot; // set up in load level
        public bool IsUnlockByVipPass { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsAdsSlot || !TransportCenter.cur.IsTimeForBooster() || LevelDataFragment.cur.gameData.level == 0) return;
            UIManager.ins.OpenUI<CanvasOfferAdsSlot>().Fetch(this);
        }

        public void UnlockSlot(bool unlockByVip = false)
        {
            adsObject.SetActive(false);
            IsAdsSlot = false;

            vipObject.SetActive(unlockByVip);
            IsUnlockByVipPass = unlockByVip;

            normalLineObject.SetActive(!unlockByVip);
        }

        public void PreParkTheBus(Bus bus)
        {
            IsSlotTaken = true;
            AssignedBus = bus;
            IsBusArrive = false;
            bus.CurrentParkSlot = this;
        }

        public void ReleaseBus()
        {
            IsSlotTaken = false;
            if (AssignedBus != null) AssignedBus.CurrentParkSlot = null;
            AssignedBus = null;
            IsBusArrive = false;

            if (IsVipSlot)
            {
                TransportCenter.cur.CheckEndGameStatus();
                SetupVipSlot(false);
            }
        }

        public void SetupVipSlot(bool isOn)
        {
            //////////////////////////////

            gameObject.SetActive(isOn);
            IsSlotTaken = true;
            IsBusArrive = true;
        }

        public void SetupSlot(bool isAdsSlot)
        {
            adsObject.SetActive(isAdsSlot);
            vipObject.SetActive(false);
            IsAdsSlot = isAdsSlot;
            IsUnlockByVipPass = false;
            normalLineObject.SetActive(true);
        }

        public (Vector3, Vector3) GetEnterAndRestPoint(Plane upPlane)
        {
            var pos = transform.position;
            var dir = transform.forward * -1f;

            var ray = new Ray(pos, dir);
            if (upPlane.Raycast(ray, out var time))
            {
                var intersectPoint = ray.GetPoint(time);
                return (intersectPoint, pos);
            }

            return (pos, pos + dir * 1.32f);
        }

        public Vector3 GetParkPoint()
        {
            var pos = transform.position;
            var dir = transform.forward * -1f;
            return pos + dir * 1.32f;
        }
    }
}