using System.Collections.Generic;
using UnityEngine;

public class Outpost : MonoBehaviour
{
    public static List<Outpost> OutpostList = new List<Outpost>();

    public float CaptureValue { get; private set; }
    public int CurrentTeam { get; private set; }

    [SerializeField, Tooltip("Time in Seconds")]
    private float _CaptureTime = 5f;

    private SkinnedMeshRenderer _FlagRenderer;

    private void OnEnable()
    {
        _FlagRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        OutpostList.Add(this);
    }

    private void OnDisable()
    {
        OutpostList.Remove(this);
    }

    private void OnTriggerStay(Collider other)
    {
        AUnit u = other.GetComponent<AUnit>();
        if(u == null)
        {
            return;
        }

        if(u.TeamNumber == CurrentTeam)
        {
            CaptureValue += Time.fixedDeltaTime / _CaptureTime;
            if(CaptureValue > 1f) CaptureValue = 1f;
        }
        else
        {
            CaptureValue -= Time.fixedDeltaTime / _CaptureTime;
            if(CaptureValue <= 0f)
            {
                CaptureValue = 0f;
                CurrentTeam = u.TeamNumber;
            }
        }
    }

    private void Update()
    {
        Color teamColor = GameManager.Instance.TeamColors[CurrentTeam];
        _FlagRenderer.material.color = Color.Lerp(Color.white, teamColor, CaptureValue);
    }
}


public static class ListExntensions
{
    public static Outpost GetRandomOutpost(this List<Outpost> lst)
    {
        if(lst.Count > 0)
        {
            int rnd = Random.Range(0, lst.Count);
            return lst[rnd];
        }
        return null;
    }
}
