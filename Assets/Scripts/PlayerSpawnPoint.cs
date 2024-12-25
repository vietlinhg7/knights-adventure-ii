using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private CinemachineCamera CinemachineCamera;
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        SaveLoadManager saveLoadManager = FindFirstObjectByType<SaveLoadManager>();
        if (saveLoadManager.LoadData() != null)
            saveLoadManager.ApplyLoadedData(player, saveLoadManager.LoadData());
        player.transform.position = gameObject.transform.position;
        if (CinemachineCamera != null)
        {
            CinemachineCamera.Target.TrackingTarget = player.transform;
        }
    }

}
