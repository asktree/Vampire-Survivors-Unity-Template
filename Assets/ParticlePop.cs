using UnityEngine;

public class ParticlePop : MonoBehaviour
{
    public int emitCount = 10;

    void Start()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("ParticleSystem component not found on ParticlePop object!");
            return;
        }

        particleSystem.Emit(emitCount);

        float maxLifetime = particleSystem.main.startLifetime.constantMax;
        Destroy(gameObject, maxLifetime);
    }
}
