using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using DG.Tweening;
using UnityEngine;

public class ShipperController : MonoBehaviour
{
    [SerializeField] private Transform _moveTransform;
    /*
    [SerializeField] private Animator _animator;
    */

    public Transform BusBus, BusVan, BusCar;
    public SkinnedMeshRenderer SkinnedMeshRenderer;
    private void Awake()
    {
        _moveTransform = transform;
    }
    public Bus CurrentBus{get; private set;}
    public void Carry(Bus bus,Action onComplete)
    {
        CurrentBus = bus;
        var sharesMat = SkinnedMeshRenderer.sharedMaterials;
        sharesMat[1] = JunkPile.ins.GetMininonColorByBus(bus);
        SkinnedMeshRenderer.sharedMaterials = sharesMat;
        _moveTransform.position = ParkingLot.cur.ShipperStartPoint.position;
        var paths = new List<Vector3>();
        var pickUpPoint = bus.PickUpPoint.position;
        var roadPoint = ParkingLot.cur.RoadPoint.position;
        var p1 = new Vector3(bus.transform.position.x, pickUpPoint.y, roadPoint.z);
        paths.Add(p1);
        /*
        paths.Add(pickUpPoint);
        */
        var isAttachBus = false;
        var threshold=4.7f+1.1f*0.9f;
        _moveTransform.DOMove(_moveTransform.position+Vector3.right*23, 10).SetEase(Ease.Linear).SetSpeedBased(true).OnUpdate(() =>
        {
            if (!isAttachBus)
            {
                if (Vector3.Distance(p1, _moveTransform.position) <=threshold)
                {
                    _moveTransform.position = p1 + Vector3.left * threshold;
                    isAttachBus = true;
                    AttachBus(bus, p1);
                }
            }
        }).OnComplete(
        () =>
        {
            onComplete?.Invoke();
            Destroy((gameObject));
        });
    }

    /*public void LeaveTheCity(Bus bus,Action onComplete)
    { 
        /*
        _moveTransform.position = ParkingLot.cur.ShipperStartPoint.position;
        #1#
        var paths = new List<Vector3>();
        var pickUpPoint = bus.PickUpPoint.position;
        var roadPoint = ParkingLot.cur.RoadPoint.position;
        var p1 = new Vector3(pickUpPoint.x, pickUpPoint.y, roadPoint.z);
        
        /*
        _moveTransform.DOMove(p1 + Vector3.right * 20, 10).SetSpeedBased(true).SetEase(Ease.Linear);
        #1#
        
        
        _moveTransform.DOMove(_moveTransform.position+Vector3.right*20, 10).SetEase(Ease.Linear)/*.SetLookAt(0.03f)#1#.SetSpeedBased(true).OnComplete(
            () =>
            {                        onComplete?.Invoke();
            });
    }*/
    public void AttachBus(Bus bus,Vector3 point)
    
    {
        var targetHolder = BusBus;
        if (bus.Type == BusType.Car)
        {
            targetHolder = BusCar;
        }
        if (bus.Type == BusType.Van)
        {
            targetHolder = BusVan;
        }
        point.y = targetHolder.position.y;
        point.y = 1.359174f;
        bus.transform.DOKill();
        bus.transform.SetParent(null);
        bus.transform.DOJump(point, 1.38f, 1, 0.45f).OnComplete(() =>
        {
            bus.transform.SetParent(targetHolder);
        });
    }
}
