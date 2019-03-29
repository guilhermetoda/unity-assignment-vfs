using System.Collections;
using UnityEngine;

public class EnableParticles : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] _ParticlesToEnable;

    [ContextMenu("Enable Particles")]
    public void EnableThisParticles()
    {
        for (int i = 0; i < _ParticlesToEnable.Length; i++)
        {
            _ParticlesToEnable[i].gameObject.SetActive(true);
            StartCoroutine(DisableAfterTime(_ParticlesToEnable[0].main.duration, _ParticlesToEnable[i].gameObject));
        }
    }

    private IEnumerator DisableAfterTime(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

}
