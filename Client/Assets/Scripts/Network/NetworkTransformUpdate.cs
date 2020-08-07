using UnityEngine;

[RequireComponent(typeof(NetworkIdentifier))]
public class NetworkTransformUpdate : MonoBehaviour
{
    public float updateRate = 0.1f;
    public float minDistanceUpdate= 0.3f, minRotUpdate = 0.5f;

    float nextUpdate;
    NetworkIdentifier identifier;

    Vector3 lastSendPos;
    float lastSendRot;
    float unUpdatedTime;

    void Start()
    {
        identifier = GetComponent<NetworkIdentifier>();
    }

    void Update()
    {
        if(Time.time > nextUpdate)
        {
            nextUpdate = Time.time + updateRate;

            if (identifier.isMine)
            {
                if(Vector3.Distance(lastSendPos, transform.position) > minDistanceUpdate || Mathf.Abs(lastSendRot - transform.eulerAngles.z) > minRotUpdate || unUpdatedTime > 1.0f)
                {
                    lastSendPos = transform.position;
                    lastSendRot = transform.eulerAngles.z;
                    DataSender.SendUpdateTransform(transform.position, transform.eulerAngles.z);
                    unUpdatedTime = 0;
                }else
                {
                    unUpdatedTime += Time.deltaTime;
                }
            }
        }
    }
}
