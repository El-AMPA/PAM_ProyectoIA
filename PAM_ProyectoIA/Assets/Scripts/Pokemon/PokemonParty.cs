using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
	[SerializeField] private List<Pokemon> pokemon;

	public List<Pokemon> Pokemons
	{
		get { 
			return pokemon; 
		}
	}
	public void Init()
	{
		foreach(var poke in pokemon)
		{
			poke.Init();
		}
	}

	public Pokemon GetHealthyPokemon()
	{
		return pokemon.Where(x => x.HP > 0).FirstOrDefault();
	}

	[SerializeField] private int nFullHeals;

	public float NFullHeals
    {
		get
        {
			return nFullHeals;
        }
    }

	[SerializeField] private int nMaxPotions;

	public float NMaxPotions
	{
		get
		{
			return nMaxPotions;
		}
	}

	public void consumeItem(ItemID item)
    {
		if (item == ItemID.fullHeal) nFullHeals--;
		else if (item == ItemID.maxPotion) nMaxPotions--;

		Debug.Log($"Consumed a {item.ToString()}");
    }
}
