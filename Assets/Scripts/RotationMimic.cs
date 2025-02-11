using UnityEngine;
using PurrNet;

public class RotationMimic : NetworkBehaviour
{
    [SerializeField] private Transform mimicObject;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }

    private void Update()
    {
        if (!mimicObject)
        {
            return;
        }

        transform.rotation = mimicObject.rotation;
    }
}
