using UnityEngine;
using PurrNet;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private RotationMimic cameraMimic;
    [SerializeField] private List<Renderer> renderers = new();

    protected override void OnSpawned()
    {
        base.OnSpawned();

        InstanceHandler.GetInstance<PlayerCameraManager>().RegisterCamera(this);
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();

        InstanceHandler.GetInstance<PlayerCameraManager>().UnregisterCamera(this);
    }

    private void TogglePlayerBody(bool toggle)
    {
        foreach (var rend in renderers)
        {
            rend.shadowCastingMode = toggle ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
        }
    }

    public void ToggleCamera(bool toggle)
    {
        playerCamera.Priority = toggle ? 10 : 0;
        TogglePlayerBody(!toggle);
    }

    private void Update()
    {
        if (isOwner)
        {
            return;
        }

        transform.rotation = cameraMimic.transform.rotation;

        // Confine mouse to window
        //Cursor.lockState = CursorLockMode.Confined;
    }
}
