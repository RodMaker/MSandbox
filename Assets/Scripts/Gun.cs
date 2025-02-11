using System;
using UnityEngine;
using PurrNet;
using System.Collections;
using System.Collections.Generic;
using PurrNet.StateMachine;
using UnityEngine.UIElements;

public class Gun : StateNode
{
    [Header("Stats")]
    [SerializeField] private float range = 20f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private bool automatic;

    [Header("Recoil")]
    [SerializeField] private float recoilStrength = 1f;
    [SerializeField] private float recoilDuration = 0.2f;
    [SerializeField] private AnimationCurve recoilCurve;
    [SerializeField] private float rotationAmount = 25f;
    [SerializeField] private AnimationCurve rotationCurve;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform rightHandTarget, leftHandTarget;
    [SerializeField] private Transform rightIkTarget, leftIkTarget;
    [SerializeField] private List<Renderer> renderers = new();
    [SerializeField] private ParticleSystem environmentHitEffect, playerHitEffect;
    [SerializeField] private SoundPlayer soundPlayerPrefab;
    [SerializeField] private AudioSource shotSoundPlayer;
    [SerializeField, Range(0f, 1f)] private float environmentHitVolume, playerHitVolume, shotVolume;
    [SerializeField] private List<AudioClip> environmentHitSounds, playerHitSounds, shotSounds;

    private float lastFireTime;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine recoilCoroutine;

    private void Awake()
    {
        ToggleVisuals(false);
    }

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        ToggleVisuals(true);
    }

    public override void Exit(bool asServer)
    {
        base.Exit(asServer);

        ToggleVisuals(false);
    }

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void ToggleVisuals(bool toggle)
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = toggle;
        }
    }

    public override void StateUpdate(bool asServer)
    {
        base.StateUpdate(asServer);

        //SetIKTargets();

        if (!isOwner)
        {
            return;
        }

        if (automatic && !Input.GetKey(KeyCode.Mouse0) || !automatic && !Input.GetKeyDown(KeyCode.Mouse0))
        {
            return;
        }

        if (lastFireTime + fireRate > Time.unscaledTime)
        {
            return;
        }

        PlayShotEffect();
        lastFireTime = Time.unscaledTime;

        if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, range, hitLayer))
        {
            return;
        }

        if (!hit.transform.TryGetComponent(out PlayerHealth playerHealth))
        {
            EnvironmentHit(hit.point, hit.normal);
            return;
        }

        PlayerHit(playerHealth, playerHealth.transform.InverseTransformPoint(hit.point), hit.normal);
        playerHealth.ChangeHealth(-damage);
    }

    [ObserversRpc(runLocally: true)]
    private void PlayerHit(PlayerHealth player, Vector3 localposition, Vector3 normal)
    {
        if (!player || !player.transform) 
        { 
            return; 
        }

        if (playerHitEffect)
        {
            Instantiate(playerHitEffect, player.transform.TransformPoint(localposition), Quaternion.LookRotation(normal));
        }

        var soundPlayer = Instantiate(soundPlayerPrefab, player.transform.TransformPoint(localposition), Quaternion.identity);
        soundPlayer.PlaySound(playerHitSounds[UnityEngine.Random.Range(0, playerHitSounds.Count)], playerHitVolume);
    }

    [ObserversRpc(runLocally:true)]
    private void EnvironmentHit(Vector3 position, Vector3 normal)
    {
        if (environmentHitEffect)
        {
            Instantiate(environmentHitEffect, position, Quaternion.LookRotation(normal));
        }

        var soundPlayer = Instantiate(soundPlayerPrefab, position, Quaternion.identity);
        soundPlayer.PlaySound(environmentHitSounds[UnityEngine.Random.Range(0, environmentHitSounds.Count)], environmentHitVolume);
    }

    private void SetIKTargets()
    {
        rightIkTarget.SetPositionAndRotation(rightHandTarget.position, rightHandTarget.rotation);
        leftIkTarget.SetPositionAndRotation(leftHandTarget.position, leftHandTarget.rotation);
    }

    [ObserversRpc(runLocally: true)]
    private void PlayShotEffect()
    {
        if (muzzleFlash)
        {
            muzzleFlash.Play();
        }

        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
        }

        recoilCoroutine = StartCoroutine(PlayRecoil());

        if (isOwner)
        {
            shotSoundPlayer.PlayOneShot(shotSounds[UnityEngine.Random.Range(0, shotSounds.Count)], shotVolume / 3f);
        }
        else
        {
            shotSoundPlayer.PlayOneShot(shotSounds[UnityEngine.Random.Range(0, shotSounds.Count)], shotVolume);
        }
    }

    private IEnumerator PlayRecoil()
    {
        float elapsed = 0f;

        while (elapsed < recoilDuration)
        {
            elapsed += Time.deltaTime;
            float curveTime = elapsed / recoilDuration;

            // Position recoil
            float recoilValue = recoilCurve.Evaluate(curveTime);
            Vector3 recoilOffset = Vector3.back * (recoilValue + recoilStrength);
            transform.localPosition = originalPosition + recoilOffset;

            // Rotation recoil
            float rotationValue = rotationCurve.Evaluate(curveTime);
            Vector3 rotationOffset = new Vector3(rotationValue + rotationAmount, 0f, 0f);
            transform.localRotation = originalRotation * Quaternion.Euler(rotationOffset);

            yield return null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}
