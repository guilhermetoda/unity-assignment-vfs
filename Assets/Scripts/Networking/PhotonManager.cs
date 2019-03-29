using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
	private int _TeamNumber = 0;

	[SerializeField]
	private string _UserName = "DUDE";

	private void Start()
	{
		PhotonNetwork.NickName = _UserName;
		// PhotonNetwork.SendRate = 50;
		// PhotonNetwork.SerializationRate = 25;
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		print("OnConnectedToMaster");
		RoomOptions options = new RoomOptions();
		options.MaxPlayers = 20;
		options.IsVisible = true;
		options.IsOpen = true;

		PhotonNetwork.JoinOrCreateRoom("GD52", options, TypedLobby.Default);
	}

	public override void OnJoinedRoom()
	{
		print("OnJoinedRoom: "+ PhotonNetwork.CurrentRoom.Name);
		var clone = PhotonNetwork.Instantiate("Net_NightShade", transform.position, transform.rotation);
		PhotonView.Get(clone).RPC("RPC_SetTeam", RpcTarget.AllBufferedViaServer, _TeamNumber);
	}

}
