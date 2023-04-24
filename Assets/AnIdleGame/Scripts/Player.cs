﻿using UnityEngine;
using System.Collections;
using Thirdweb;

//script used to control accack
public class Player : MonoBehaviour {

	private Animator playerAnim; //player animator
	private Cooldown cooldown;

	void Awake ()
	{
		playerAnim = GetComponentsInChildren<Animator> ()[0];

		cooldown = GetComponentInChildren<Cooldown> ();
	}

	public void StartAttack () //function for other scripts to call the attack coroutine
	{
		StartCoroutine ("Attack");
	}

	public void StopAttack () //function to stop attack
	{
		StopCoroutine ("Attack");

		cooldown.StopCoroutine ("WaitForCooldown");
	}

	private IEnumerator Attack () //function to make the attack
	{
		while (GameManager.Instance.aliveEnemies.Count > 0) //only attack there is at least one alvie enemy
		{
			yield return cooldown.StartCoroutine ("WaitForCooldown"); //wait until cooldown finishes

			playerAnim.SetTrigger ("Attack"); //trigger the player animation

			AnimatorStateInfo animasiStateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
			float durasiAnimasi = animasiStateInfo.length;

			yield return new WaitForSeconds(durasiAnimasi);

			//get how many targets to attack. If more enemies than the number of max targets, n equals to max target, otherwise the amount of enemies left
			int n = (PlayerStats.Instance.activeSkill.numberOfTargets <= GameManager.Instance.aliveEnemies.Count) ? PlayerStats.Instance.activeSkill.numberOfTargets : GameManager.Instance.aliveEnemies.Count;

			int[] targets = GameManager.Instance.aliveEnemies.ToArray (); //make a copy of alive enemies and turn it into an array

			Shuffle (targets); //shuffle the array

			for (int i = 0; i < n; i++) //attack the first n enemies
			{
				GameManager.Instance.enemies[targets[i]].TakeDamage (PlayerStats.Instance.activeSkill.AttackDamage);
			}

			
		}

		if (GameManager.Instance.isBattling) //if we kill all enemies in time
		{
			Debug.Log ("All Killed");

			GameManager.Instance.StopBattle (true); //we stop the battle with levelCompleted set to true
		}
	}

	private static void Shuffle<T> (T[] array) //function used to shuffle an array
	{
		int n = array.Length;

		for (int i = 0; i < n; i++)
		{
			int index = i + Random.Range(0, n - i);

			T t = array[index];

			array[index] = array[i];

			array[i] = t;
		}
	}

	public async void getToken(){

		try
        {
			var _tempMoney = PlayerStats.Instance.TempMoney;

			if (Application.platform == RuntimePlatform.WebGLPlayer) {
				var data = await Web3Auth.Instance.contractGame.Read<TransactionResult>("claimToken", new object[] { PlayerStats.Instance.TempMoney });
			}
			

			PlayerStats.Instance.Money += _tempMoney;
			PlayerStats.Instance.TempMoney =  PlayerStats.Instance.TempMoney - _tempMoney;
			if (Application.platform == RuntimePlatform.WebGLPlayer) {
				FirebaseDatabase.getTempMoney(Web3Auth.Instance.addressWallet, PlayerStats.Instance.TempMoney);
			}
			
        }
        catch (System.Exception e)
        {
            Debugger.Instance.Log("Error", e.Message);
        }

		
	}
}
