using UnityEngine;

namespace DeliverySystem
{
    public class DeliveryTask : MonoBehaviour
    {
        public float expirationTime;

        private Transform player;
        private float distance;

        public void Initialize(Transform playerTransform)
        {
            player = playerTransform;
            distance = Vector3.Distance(transform.position, player.position);
            expirationTime = Time.time + distance * DeliveryManager.Instance.baseTimePerUnitDistance;
        }

        private void Update()
        {
            if (Time.time > expirationTime)
                DeliveryManager.Instance.FailDelivery();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                DeliveryManager.Instance.CompleteDelivery();
        }
    }
}