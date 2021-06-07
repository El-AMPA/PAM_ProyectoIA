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
    private void Start()
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
}
