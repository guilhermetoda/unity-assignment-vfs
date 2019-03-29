using Photon.Pun;
using UnityEngine;

public class NetDataSyncer : MonoBehaviourPun, IPunObservable
{

	[SerializeField, Range(0.01f, 1f)]
	private float _SmoothFactor = 1f;

	private Vector3 _NetPos;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) // Sending data
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else // Receiving Data
		{
			_NetPos = (Vector3)stream.ReceiveNext();
			transform.rotation = (Quaternion)stream.ReceiveNext();
		}
    }

    [PunRPC]
	private void RPC_SetTeam(int team)
	{
		GetComponent<AUnit>().SetTeam(team);
	}


	private void Update()
	{
		if(photonView.IsMine)
		{
			return;
		}

		transform.position = Vector3.Lerp(transform.position, _NetPos, _SmoothFactor);
	}


}
