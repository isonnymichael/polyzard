using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections;
using System.IO; //for File.Exists
using UnityEngine.SceneManagement;
using Thirdweb;
using Newtonsoft.Json;

//script used in "Menu" Sence.
public class Menu : MonoBehaviour {

	public Button newGameBtn;
	public Button continueBtn;

	public GameObject connectedObject;
	public GameObject disconnecteObject;

	[SerializeField] TextMeshProUGUI txtWalletAddress;
	[SerializeField] TextMeshProUGUI txtToken;
	[SerializeField] TextMeshProUGUI txtNotConnected;

	public static bool newGame = true; //if to start a new game

	void Start ()
	{

		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			if (PlayerPrefs.HasKey ("PlayerData")) //if there is a saved playerpref, we enable the continue button
				continueBtn.interactable = true;
			else
				continueBtn.interactable = false;
		}else{
			if (File.Exists (Application.persistentDataPath + "/PlayerData.xml")) //if there is a saved game, we enable the continue button
				continueBtn.interactable = true;
			else
				continueBtn.interactable = false;
		}

		ConnectWallet();
	}

	public async void ConnectWallet()
	{
		Web3Auth.Instance.addressWallet = await Web3Auth.Instance.sdk.wallet.Connect(new WalletConnection(){
			provider = WalletProvider.MetaMask,
			chainId = Web3Auth.Instance.chainId
		});

		txtNotConnected.text = "Connecting . . .";
		connectingOnRightChain();
	}

	public async void connectingOnRightChain(){
		int myChainId = await Web3Auth.Instance.sdk.wallet.GetChainId();
		if(myChainId == Web3Auth.Instance.chainId)
		{
			// connect to db
			FirebaseDatabase.getData(Web3Auth.Instance.addressWallet);

			string abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"burnTokens\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"subtractedValue\",\"type\":\"uint256\"}],\"name\":\"decreaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"addedValue\",\"type\":\"uint256\"}],\"name\":\"increaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"mintTo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
			// Connect ke TOKEN SMART CONTRACT
			Web3Auth.Instance.contractToken = Web3Auth.Instance.sdk.GetContract("0xBDeDf5B4D79f6E1ef0E372477AabE7B39d6C8f3a",abi);
			var token = await Web3Auth.Instance.contractToken.ERC20.Balance();
			Web3Auth.Instance.symbolToken = token.symbol;
			Web3Auth.Instance.valueToken = token.value;
			Web3Auth.Instance.displayValueToken = token.displayValue;
			txtToken.text = Web3Auth.Instance.displayValueToken+" "+Web3Auth.Instance.symbolToken;
			txtWalletAddress.text = Web3Auth.Instance.addressWallet;

			// Connect ke IDLE GAME SMART CONTRACT
			string abiGame = "[{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"claimToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_activatedSkillID\",\"type\":\"uint256\"}],\"name\":\"nextLevel\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"contract SemangkaToken\",\"name\":\"_token\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_skillId\",\"type\":\"uint256\"}],\"name\":\"unlockSkill\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_stat\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_indexStats\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"upgradeStats\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_player\",\"type\":\"address\"}],\"name\":\"getPlayerData\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"activatedSkillID\",\"type\":\"uint256\"},{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"damageLevel\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"speedLevel\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"unlocked\",\"type\":\"bool\"}],\"internalType\":\"struct MyIdleGame.Skill[]\",\"name\":\"skills\",\"type\":\"tuple[]\"}],\"internalType\":\"struct MyIdleGame.Player\",\"name\":\"\",\"type\":\"tuple\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"activatedSkillID\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"token\",\"outputs\":[{\"internalType\":\"contract SemangkaToken\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
			Web3Auth.Instance.contractGame = Web3Auth.Instance.sdk.GetContract("0x49423D1d2EbCe84eba212c81A15ebA529C812DbC",abiGame);
			

			toggleStartScreen();
		}
		else
		{
			txtNotConnected.text = "Your not connect, please refresh . . .";
		}
			
	}

	public void ForceMenu(){
		SceneManager.LoadScene(0);
	}

	public void NewGame () //function called when click new game button
	{
		newGame = true; //set newGame to true

		SceneManager.LoadScene(1); //load "Game" scene
	}

	public void LoadGame () //function called when click continue button
	{
		GameSFX.Instance.playClickSound();
		
		newGame = false;

		SceneManager.LoadScene(1);
	}

	public void toggleStartScreen()
	{
		connectedObject.SetActive(true);
		disconnecteObject.SetActive(false);
	}

	public void OnGetDataFB(string result)
	{
		// get data from db
		Debug.Log("LOG GET DATA FROM DB");
		Debug.Log(result);

		//string savedGame = "{\"level\":\"11\",\"money\":\"99990\",\"tempMoney\":\"9\",\"activatedSkillID\":\"1\",\"quitTime\":\"2023-03-28T19:20:03.0197181+07:00\",\"skills\":[{\"ID\":\"0\",\"damageLevel\":\"0\",\"speedLevel\":\"0\",\"unlocked\":\"true\"},{\"ID\":\"1\",\"damageLevel\":\"10\",\"speedLevel\":\"10\",\"unlocked\":\"false\"},{\"ID\":\"2\",\"damageLevel\":\"0\",\"speedLevel\":\"0\",\"unlocked\":\"false\"},{\"ID\":\"3\",\"damageLevel\":\"0\",\"speedLevel\":\"0\",\"unlocked\":\"false\"},{\"ID\":\"4\",\"damageLevel\":\"0\",\"speedLevel\":\"0\",\"unlocked\":\"false\"},{\"ID\":\"5\",\"damageLevel\":\"0\",\"speedLevel\":\"0\",\"unlocked\":\"false\"},{\"ID\":\"6\",\"damageLevel\":\"0\",\"speedLevel\":\"0\",\"unlocked\":\"false\"},{\"ID\":\"7\",\"damageLevel\":\"0\",\"speedLevel\":\"0\",\"unlocked\":\"false\"}]}";
		PlayerDataStatic.playerData = JsonUtility.FromJson<PlayerData>(result);
	}

}
