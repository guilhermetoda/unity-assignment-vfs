using System.Collections;
using UnityEngine;

public class CoroutineTester : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(MyFirstCoroutine());
		print("End of Start");
	}

	private IEnumerator MyCoroutine_3()
	{
		while(true)
		{
			//What is this???
			yield return null; //wait 1 frame
			yield return null; //wait 1 frame
		}
	}

	private IEnumerator MyCoroutine_2()
	{
		yield return new WaitForSecondsRealtime(1);
		yield return new WaitForSeconds(1);
		yield return new WaitForSeconds(1);
	}

	private IEnumerator MyFirstCoroutine()
	{
		print(Time.time);
		yield return null; //wait 1 frame
		print(Time.time);
		yield return new WaitForSeconds(2f);
		print(Time.time);
		yield return StartCoroutine(MyCoroutine_2());
		print(Time.time);
		yield return null; //wait 1 frame
		print(Time.time);
	}

}
