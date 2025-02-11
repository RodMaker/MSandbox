using UnityEngine;
using PurrNet;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private SyncVar<int> health = new(100);
    [SerializeField] private int selfLayer, otherLayer;

    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private SoundPlayer soundPlayerPrefab;
    [SerializeField] private AudioClip deathSound;
    [SerializeField, Range(0, 1f)] private float deathAudioVolume = 0.5f;

    public Action<PlayerID> OnDeath_Server;

    public int Health => health;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        var actualLayer = isOwner ? selfLayer : otherLayer;
        SetLayerRecursive(gameObject, actualLayer);

        if (isOwner)
        {
            InstanceHandler.GetInstance<MainGameView>().UpdateHealth(health.value);
            health.onChanged += OnHealthChanged;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        health.onChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int newHealth)
    {
        InstanceHandler.GetInstance<MainGameView>().UpdateHealth(newHealth);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    [ServerRpc(requireOwnership:false)]
    public void ChangeHealth(int amount, RPCInfo info = default)
    {
        health.value += amount;

        if (health <= 0)
        {
            if (InstanceHandler.TryGetInstance(out ScoreManager scoreManager))
            {
                scoreManager.AddKill(info.sender);
                if (owner.HasValue)
                {
                    scoreManager.AddDeath(owner.Value);
                }
            }

            OnDeath_Server?.Invoke(owner.Value);
            Destroy(gameObject);
        }
    }

    [ObserversRpc(runLocally:true)]
    private void PlayDeathEffects()
    {
        Instantiate(deathParticles, transform.position + Vector3.up, Quaternion.identity);

        var soundPlayer = Instantiate(soundPlayerPrefab, transform.position + Vector3.up, Quaternion.identity);
        soundPlayer.PlaySound(deathSound, deathAudioVolume);
    }
}
