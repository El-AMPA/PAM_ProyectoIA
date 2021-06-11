using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_dIAblo : PokemonIA
{
	public override Pokemon decideSwitch()
	{
		return null;
	}

	public override ItemID decideItemUse()
	{
		//Si su Pokemon tiene la mitad de la vida o menos y tiene Pociones, tiene un 20% de probabilidades de usarla.
		ItemID item = ItemID.NULL;
		if (myParty.NMaxPotions > 0)
		{
			int random = Random.Range(0, 100);
			if (random < 40) item = ItemID.maxPotion;
		}
		return item;
	}

	public override Move decideNextMove()
	{
		//Coge el primer movimiento con tipo supereficaz que encuentra
		Move moveChoice = null;
		int maxDamage = 0;
		List<Move> killMoves = new List<Move>();
		foreach (Move move in myUnit.Pokemon.Moves)
		{
			int dmg = opposingUnit.Pokemon.SimulateDamage(move, myUnit.Pokemon);
			if (dmg > opposingUnit.Pokemon.HP) //Si mata al oponente
            {
				if (move.Base.Priority > 0) //Si le matamos Y tiene prioridad, lo mejor es utilizar este ataque
                {
					killMoves.Clear();
					killMoves.Add(move);
					break;
                }
				else killMoves.Add(move);
            }
			else if (dmg > maxDamage)
            {
				maxDamage = dmg;
				moveChoice = move;
            }
		}

		if (killMoves.Count > 0) return killMoves[Random.Range(0, killMoves.Count)];
		else return moveChoice;
	}

	public override Pokemon decideNextPokemon()
	{
		return myParty.GetHealthyPokemon();
	}

}
