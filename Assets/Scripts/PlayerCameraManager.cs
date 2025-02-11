using UnityEngine;
using System;
using System.Collections.Generic;
using PurrNet;

public class PlayerCameraManager : MonoBehaviour
{
    private List<PlayerCamera> allPlayerCameras = new();

    private bool canSwitchCamera;

    private int currentCameraIndex;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<PlayerCameraManager>();
    }

    public void RegisterCamera(PlayerCamera cam)
    {
        if (allPlayerCameras.Contains(cam))
            return;
        allPlayerCameras.Add(cam);
        if (cam.isOwner)
        {
            canSwitchCamera = false;
            cam.ToggleCamera(true);
        }
    }

    public void UnregisterCamera(PlayerCamera cam)
    {
        if (allPlayerCameras.Contains(cam))
            allPlayerCameras.Remove(cam);
        if (cam.isOwner)
        {
            canSwitchCamera = true;
            SwitchNext();
        }
    }

    private void Update()
    {
        if (!canSwitchCamera)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SwitchNext();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SwitchPrevious();
        }
    }

    private void SwitchNext()
    {
        if (allPlayerCameras.Count <= 0)
        {
            return;
        }

        allPlayerCameras[currentCameraIndex].ToggleCamera(false);
        currentCameraIndex++;
        if (currentCameraIndex >= allPlayerCameras.Count)
            currentCameraIndex = 0;
        allPlayerCameras[currentCameraIndex].ToggleCamera(true);
    }

    private void SwitchPrevious()
    {
        if (allPlayerCameras.Count <= 0)
        {
            return;
        }

        allPlayerCameras[currentCameraIndex].ToggleCamera(false);
        currentCameraIndex--;
        if (currentCameraIndex < 0)
            currentCameraIndex = allPlayerCameras.Count - 1;
        allPlayerCameras[currentCameraIndex].ToggleCamera(true);
    }
}
