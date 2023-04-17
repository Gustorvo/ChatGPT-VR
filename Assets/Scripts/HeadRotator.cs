using UnityEngine;

public class HeadRotator : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField, Range(0, 10)] float vertitalMargin, horizontalMargin;
    [SerializeField, Range(0.5f, 20)] float rotationSpeedAprx = 10;
    


    private Quaternion targetRotation = Quaternion.identity;
    private float speed = 0f;
    private float time = 0f;
    private Quaternion startRotation;
    private float rotationStartTime;

    private void Awake()
    {
        RotateHead();
    }

    void RotateHead()
    {
        startRotation = head.localRotation;
        rotationStartTime = Time.time;
        float vertical = Random.Range(-vertitalMargin, vertitalMargin);
        float horizontal = Random.Range(-horizontalMargin, horizontalMargin);
        float nextRotationInSeconds = Random.Range(0.1f, 3f);
        speed = Random.Range(rotationSpeedAprx - rotationSpeedAprx * 0.5f, rotationSpeedAprx + rotationSpeedAprx * 0.5f);
        Invoke("RotateHead", nextRotationInSeconds);
        targetRotation = Quaternion.Euler(vertical, horizontal, 0f);
    }

    private void Update()
    {
        head.localRotation = Quaternion.Lerp(head.localRotation, targetRotation, speed * Time.deltaTime);
    }
}
