using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_dIAblo : PokemonIA
{
	[Range(0.0f, 1.0f)]
	[SerializeField] float offensiveThreshold;
	[Range(0.0f, 1.0f)]
	[SerializeField] float defensiveThreshold;
	[SerializeField] int switchThreshold;

	Pokemon lastSwitch;
	int switchCounter;

	public override Pokemon decideSwitch()
	{
		return null;
	}

	public override ItemID decideItemUse()
	{
		//Si su Pokemon tiene la mitad de la vida o menos y tiene Pociones, tiene un 20% de probabilidades de usarla.
		ItemID item = ItemID.NULL;
		if (myParty.NFullHeals > 0)
		{
			if (myUnit.Pokemon.HP >= offensiveThreshold * myUnit.Pokemon.MaxHP && (myUnit.Pokemon.Status != null || myUnit.Pokemon.VolatileStatus!= null)) item = ItemID.fullHeal;
		}
		if (myParty.NMaxPotions > 0)
		{
			if (myUnit.Pokemon.HP <= defensiveThreshold * myUnit.Pokemon.MaxHP) item = ItemID.maxPotion;
		}
		return item;
	}

	public override Move decideNextMove()
	{
		//Coge el primer movimiento con tipo supereficaz que encuentra
		Move moveChoice = null;
		int maxDamage = 0;
		List<Move> killMoves = new List<Move>();
		List<Move> statusMoves = new List<Move>();

		Pokemon unitToAttack;
		if (switchCounter >= switchThreshold) unitToAttack = lastSwitch;
		else unitToAttack = opposingUnit.Pokemon;

		foreach (Move move in myUnit.Pokemon.Moves)
		{
			if (move.PP == 0) continue;
			int dmg = unitToAttack.SimulateDamage(move, myUnit.Pokemon) * (move.Base.Accuracy / 100);
			if (dmg > unitToAttack.HP) //Si mata al oponente
            {
				if (move.Base.Priority > 0) //Si le matamos Y tiene prioridad, lo mejor es utilizar este ataque
                {
					killMoves.Clear();
					killMoves.Add(move);
					break;
                }
				else killMoves.Add(move);
            }

			if (move.Base.Category == MoveCategory.Status)
            {
				if (myUnit.Pokemon.HP >= offensiveThreshold * myUnit.Pokemon.MaxHP && move.Base.Effects.Status != ConditionID.rest)
                {
					if (unitToAttack.Status == null && move.Base.Effects.Status != ConditionID.none)
                    {
						statusMoves.Add(move);
                    }
					else if (unitToAttack.VolatileStatus == null && move.Base.Effects.VolatileStatus != ConditionID.none)
					{
						statusMoves.Add(move);
					}
					else if (move.Base.Effects.Boosts.Count > 0)
                    {
						statusMoves.Add(move);
                    }
                }

				else if (myUnit.Pokemon.HP <= defensiveThreshold * myUnit.Pokemon.MaxHP)
                {
					if (move.Base.Effects.Status == ConditionID.rest) statusMoves.Add(move);
                }

			}

			else if (dmg > maxDamage)
            {
				maxDamage = dmg;
				moveChoice = move;
            }
		}

		if (killMoves.Count > 0) return killMoves[Random.Range(0, killMoves.Count)];
		if (statusMoves.Count > 0) return statusMoves[Random.Range(0, statusMoves.Count)];
		if (moveChoice != null) return moveChoice;
		return myUnit.Pokemon.GetRandomMove();
	}

	public override Pokemon decideNextPokemon()
	{
		return myParty.GetHealthyPokemon();
	}

	public override void onEnemySwitch(Pokemon pokemon)
    {
		lastSwitch = pokemon;
		switchCounter++;
    }

	public override void onEnemyAttack()
    {
		lastSwitch = null;
		switchCounter = 0;
	}
}
