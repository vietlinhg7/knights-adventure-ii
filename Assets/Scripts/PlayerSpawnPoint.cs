using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private CinemachineCamera CinemachineCamera;
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = gameObject.transform.position;
        if (CinemachineCamera != null)
        {
            CinemachineCamera.Target.TrackingTarget = player.transform;
        }
    }

}
