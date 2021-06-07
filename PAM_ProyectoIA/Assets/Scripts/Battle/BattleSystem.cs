using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{

	[SerializeField] BattleUnit playerUnit;
	[SerializeField] BattleUnit enemyUnit;
	[SerializeField] BattleDialogBox dialogBox;
	[SerializeField] PartyScreen partyScreen;
	[SerializeField] PokemonParty playerParty;
	[SerializeField] PokemonParty enemyParty;

	BattleState state;
	int currentAction;
	int currentMove;
	int currentMember;

	private void Start()
	{
		ConditionsDB.Init();
		StartCoroutine(SetupBattle());
	}

	public IEnumerator SetupBattle()
	{
		playerParty.Init();
		enemyParty.Init();

		playerUnit.Setup(playerParty.GetHealthyPokemon());
		enemyUnit.Setup(enemyParty.GetHealthyPokemon());

		partyScreen.Init();

		dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

		yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

		ChooseFistTurn();
	}

	IEnumerator BattleOver(bool won)
	{
		state = BattleState.BattleOver;
		yield return OnBattleOver(won);
	}

	void ActionSelection()
	{
		state = BattleState.ActionSelection;
		StartCoroutine(dialogBox.TypeDialog("Choose an action"));
		dialogBox.EnableActionSelector(true);
	}

	void MoveSelection()
	{
		state = BattleState.MoveSelection;
		dialogBox.EnableActionSelector(false);
		dialogBox.EnableDialogText(false);
		dialogBox.EnableMoveSelector(true);
	}

	private void Update()
	{
		if (state == BattleState.ActionSelection)
		{
			HandleActionSelection();
		}
		else if (state == BattleState.MoveSelection)
		{
			HandleMoveSelection();
		}
		else if(state == BattleState.PartyScreen)
		{
			HandlePartySelection();
		}
	}

	void HandleActionSelection()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow)) ++currentAction;
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) --currentAction;
		else if (Input.GetKeyDown(KeyCode.DownArrow)) currentAction += 2;
		else if (Input.GetKeyDown(KeyCode.UpArrow)) currentAction -= 2;

		currentAction = Mathf.Clamp(currentAction, 0, 3);

		dialogBox.UpdateActionSelection(currentAction);

		if (Input.GetKeyDown(KeyCode.Z))
		{
			if(currentAction == 0)
			{
				//Fight
				MoveSelection();
			}
			
			else if (currentAction == 1)
			{
				//Bag
			}

			if (currentAction == 2)
			{
				//Pokemon
				OpenPartyScreen();
			}

			else if (currentAction == 3)
			{
				//Run
			}
		}
	}

	void OpenPartyScreen()
	{
		state = BattleState.PartyScreen;
		partyScreen.SetPartyData(playerParty.Pokemons);
		partyScreen.gameObject.SetActive(true);
	}

	void HandleMoveSelection()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow)) ++currentMove;
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) --currentMove;
		else if (Input.GetKeyDown(KeyCode.DownArrow)) currentMove += 2;
		else if (Input.GetKeyDown(KeyCode.UpArrow)) currentMove -= 2;

		currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

		dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

		if (Input.GetKeyDown(KeyCode.Z))
		{
			dialogBox.EnableMoveSelector(false);
			dialogBox.EnableDialogText(true);
			StartCoroutine(PlayerMove());
		}
		else if (Input.GetKeyDown(KeyCode.X))
		{
			dialogBox.EnableMoveSelector(false);
			dialogBox.EnableDialogText(true);
			ActionSelection();
		}
	}

	void HandlePartySelection()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow)) ++currentMember;
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) --currentMember;
		else if (Input.GetKeyDown(KeyCode.DownArrow)) currentMember += 2;
		else if (Input.GetKeyDown(KeyCode.UpArrow)) currentMember -= 2;

		currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

		partyScreen.UpdateMemberselection(currentMember);

		if (Input.GetKeyDown(KeyCode.Z))
		{
			var selectedmember = playerParty.Pokemons[currentMember];
			if (selectedmember.HP <= 0)
			{
				partyScreen.SetMessageText("You can't send out a fainted Pokémon!");
				return;
			}
			if (selectedmember == playerUnit.Pokemon)
			{
				partyScreen.SetMessageText("That Pokémon is already out!");
				return;
			}

			partyScreen.gameObject.SetActive(false);
			state = BattleState.Busy;
			StartCoroutine(SwitchPokemon(selectedmember));
		}
		else if (Input.GetKeyDown(KeyCode.X) && playerUnit.Pokemon.HP >= 0)
		{
			partyScreen.gameObject.SetActive(false);
			ActionSelection();
		}
	}

	IEnumerator SwitchPokemon(Pokemon newPokemon)
	{
		bool currentPokemonFainted = true;
		if (playerUnit.Pokemon.HP > 0)
		{
			currentPokemonFainted = false;
			yield return dialogBox.TypeDialog($"Come back, {playerUnit.Pokemon.Base.Name}!");
			playerUnit.PlayFaintAnimation();
			yield return new WaitForSeconds(2f);
		}

		playerUnit.Setup(newPokemon);
		dialogBox.SetMoveNames(newPokemon.Moves);
		yield return dialogBox.TypeDialog($"Go, {newPokemon.Base.Name}!");

		if (currentPokemonFainted)
		{
			ChooseFistTurn();
		}
		else
		{
			//if(state == BattleState.PerformMove)
			StartCoroutine(EnemyMove());
		}
	}

	IEnumerator PlayerMove()
	{
		state = BattleState.PerformMove;

		var move = playerUnit.Pokemon.Moves[currentMove];
		yield return RunMove(playerUnit, enemyUnit, move);

		if(state == BattleState.PerformMove)
			StartCoroutine(EnemyMove());
	}

	IEnumerator EnemyMove()
	{
		state = BattleState.PerformMove;

		var move = enemyUnit.Pokemon.GetRandomMove();
		yield return RunMove(enemyUnit, playerUnit, move);

		if (state == BattleState.PerformMove)
			ActionSelection();
	}

	void ChooseFistTurn()
	{
		if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
		{
			ActionSelection();
		}
		else StartCoroutine(EnemyMove());
	}

	IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
	{
		bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();

		if (!canRunMove)
		{
			yield return ShowStatusChanges(sourceUnit.Pokemon);
			yield return sourceUnit.Hud.UpdateHP();
			yield break;
		}

		move.PP--;
		yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

		if(CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
		{
			sourceUnit.PlayAttackAnimation();
			yield return new WaitForSeconds(1f);
			targetUnit.PlayHitAnimation();

			if (move.Base.Category == MoveCategory.Status)
			{
				yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
			}
			else
			{
				var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
				yield return targetUnit.Hud.UpdateHP();
				yield return ShowDamageDetails(damageDetails);
			}

			if(move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0){
				foreach(var secondary in move.Base.Secondaries)
				{
					var rnd = UnityEngine.Random.Range(1, 101);
					if (rnd <= secondary.Chance)
						yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
				}
			}

			if (targetUnit.Pokemon.HP <= 0)
			{
				yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} Fainted!");
				targetUnit.PlayFaintAnimation();

				yield return new WaitForSeconds(1f);

				yield return CheckForBattleOver(targetUnit);
				//StartCoroutine(CheckForBattleOver(targetUnit));
			}

			//Para condiciones de estado tipo veneno o quemado que dañan al final del turno.
			sourceUnit.Pokemon.OnAfterTurn();
			yield return ShowStatusChanges(sourceUnit.Pokemon);
			yield return sourceUnit.Hud.UpdateHP();
			if (sourceUnit.Pokemon.HP <= 0)
			{
				yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} Fainted!");
				sourceUnit.PlayFaintAnimation();

				yield return new WaitForSeconds(1f);

				yield return CheckForBattleOver(sourceUnit);
				//StartCoroutine(CheckForBattleOver(targetUnit));
			}
		}
		else
		{
			yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.name}'s attack missed!");
		}
		
	}

	IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
	{
		//Aumento de stats
		if (effects.Boosts != null)
		{
			if (moveTarget == MoveTarget.Self)
				source.ApplyBoosts(effects.Boosts);
			else
				target.ApplyBoosts(effects.Boosts);
		}

		//Condiciones de estado
		if(effects.Status != ConditionID.none)
		{
			target.SetStatus(effects.Status);
		}

		//Condiciones de estado volátiles
		if (effects.VolatileStatus != ConditionID.none)
		{
			target.SetVolatileStatus(effects.VolatileStatus);
		}

		yield return ShowStatusChanges(source);
		yield return ShowStatusChanges(target);
	}

	bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
	{
		if (move.Base.AlwaysHits) return true;

		float moveAccuracy = move.Base.Accuracy;

		int accuracy = source.StatBoosts[Stat.Accuracy];
		int evasion = source.StatBoosts[Stat.Evasion];

		var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

		if (accuracy > 0)
			moveAccuracy *= boostValues[accuracy];
		else
			moveAccuracy /= boostValues[-accuracy];

		if (evasion > 0)
			moveAccuracy /= boostValues[evasion];
		else
			moveAccuracy *= boostValues[-evasion];

		return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
	}
	IEnumerator ShowStatusChanges(Pokemon pokemon)
	{
		while (pokemon.StatusChanges.Count > 0)
		{
			var message = pokemon.StatusChanges.Dequeue();
			yield return dialogBox.TypeDialog(message);
		}
	}

	IEnumerator CheckForBattleOver(BattleUnit faintedUnit)
	{
		if (faintedUnit.IsPlayerUnit)
		{
			var nextPokemon = playerParty.GetHealthyPokemon();
			if (nextPokemon != null)
			{
				OpenPartyScreen();
			}
			else
			{
				yield return BattleOver(false);
			}
		}
		else
		{
			var nextPokemon = enemyParty.GetHealthyPokemon();
			if (nextPokemon != null)
			{
				faintedUnit.Setup(enemyParty.GetHealthyPokemon());

				yield return dialogBox.TypeDialog($"Your rival sent {nextPokemon.Base.Name} out!");
				yield return new WaitForSeconds(1f);
				StartCoroutine(EnemyMove());
			}
			else
			{
				yield return BattleOver(true);
			}
		}
	}

	IEnumerator OnBattleOver(bool won)
	{
		if (won)
		{
			yield return dialogBox.TypeDialog("You won the battle!");
			yield return new WaitForSeconds(5f);
			//Volver al menú o lo que sea
		}
		else
		{
			yield return dialogBox.TypeDialog("You lost the battle...");
			yield return new WaitForSeconds(5f);
			//Volver al menú o lo que sea
		}
	}
	IEnumerator ShowDamageDetails(DamageDetails damageDetails) {
		if (damageDetails.Critical > 1f)
			yield return dialogBox.TypeDialog("A critical hit!");

		if (damageDetails.TypeEffectiveness > 1f)
			yield return dialogBox.TypeDialog("It's super effective!");
		else if (damageDetails.TypeEffectiveness == 0f)
			yield return dialogBox.TypeDialog("It does not affect the opposing Pokémon...");
		else if (damageDetails.TypeEffectiveness < 1f)
			yield return dialogBox.TypeDialog("It's not very effective...");
	}
}
