using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobblePro : MonoBehaviour
{
    public new Transform transform;
    public Renderer rend;
    private Vector3 lastPos;
    private Vector3 velocity;
    private Vector3 lastRot;
    private Vector3 angularVelocity;
    public float MaxWobble = .03f;
    public float wobbleSpeed = 1f;
    public float Recovery = 1f;
    private float wobbleX;
    private float wobbleZ;
    private float wobbleAmountToAddX;
    private float wobbleAmountToAddZ;
    private float pulse;
    float time = .5f;

    private const string WobbleX = "_Wx";
    private const string WobbleZ = "_Wz";

    private void OnEnable()
    {
        time = .5f;
        lastPos = Vector3Pot.zero;
        lastRot = Vector3Pot.zero;
        wobbleAmountToAddX = 0;
        wobbleAmountToAddZ = 0;
        wobbleX = 0;
        wobbleZ = 0;
        pulse = 0;
        velocity = Vector3Pot.zero;
        angularVelocity = Vector3Pot.zero;
    }
    private void Update()
    {
        time += Time.deltaTime;

        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * Recovery);
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * Recovery);

        pulse = 2 * Mathf.PI * wobbleSpeed;
        wobbleX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        rend.material.SetFloat(WobbleX, wobbleX);
        rend.material.SetFloat(WobbleZ, wobbleZ);

        // velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.localEulerAngles - lastRot;

        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.62f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.62f)) * MaxWobble, -MaxWobble, MaxWobble);

        lastPos = transform.position;
        lastRot = transform.localEulerAngles;
    }
}
