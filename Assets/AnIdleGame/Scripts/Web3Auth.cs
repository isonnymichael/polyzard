using UnityEngine;
using System.Collections;
using Thirdweb;

//script used to keep music playing between two sences
public class Web3Auth : MonoBehaviour {
	
	private static Web3Auth instance;

	public ThirdwebSDK sdk;
	public string addressWallet;
	public string valueToken;
	public string displayValueToken;
	public string symbolToken;
	public int chainId = 80001;

	public Contract contractToken;
	public Contract contractGame;

	public static Web3Auth Instance
	{
		get
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer) {
				if (instance == null)
					instance = GameObject.Find("Web3").GetComponent<Web3Auth> ();
			}
			
			return instance;
		}
	}

	void Awake ()
	{
		if (instance == null)
		{
			sdk = new ThirdwebSDK("mumbai");
			instance = this;
		}
		else
		{
			if (this != instance)
				Destroy (this.gameObject);
		}

		DontDestroyOnLoad (this.gameObject);
	}
}
