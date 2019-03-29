using UnityEngine;

public class GameManager : MonoBehaviour
{

	#region Singleton
		
		public static GameManager Instance;
		private void Awake()
		{
			if(Instance == null)
			{
				Instance = this;
				return;
			}
			Destroy(this);
		}

	#endregion Singleton

	public Color[] TeamColors;

}
