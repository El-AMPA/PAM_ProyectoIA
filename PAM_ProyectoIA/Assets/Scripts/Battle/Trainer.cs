using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trainer : MonoBehaviour
{
    [SerializeField] Sprite trainerSprite;
    [SerializeField] string trainerName;

    public Sprite TrainerSprite {
        get { return trainerSprite; }
    }

    public string TrainerName
    {
        get { return trainerName; }
    }

    public PokemonParty TrainerParty
    {
        get { return gameObject.GetComponent<PokemonParty>(); }
    }
}
