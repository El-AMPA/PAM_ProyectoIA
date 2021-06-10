using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
	public static void Init()
	{
		foreach(var kvp in Conditions)
		{
			var conditionId = kvp.Key;
			var condition = kvp.Value;

			condition.Id = conditionId;
		}
	}

public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
	{
		{ConditionID.psn,
			new Condition()
			{
				Name= "Poison",
				StartMessage="has been poisoned",
				OnAfterTurn = (Pokemon pokemon) =>
				{
					pokemon.UpdateHP(pokemon.MaxHP / 8);
					pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison");
				}
			} 
		},
		{ConditionID.brn,
			new Condition()
			{
				Name= "Burn",
				StartMessage="has been burned",
				OnAfterTurn = (Pokemon pokemon) =>
				{
					pokemon.UpdateHP(pokemon.MaxHP / 16);
					pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to burn");
				}
			}
		},
		{ConditionID.par,
			new Condition()
			{
				Name= "Paralyzed",
				StartMessage="has been paralized",
				OnBeforeMove = (Pokemon pokemon) =>
				{
					if (Random.Range(1, 5) == 1){
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s paralized and can't move");
						return false;
					}

					return true;
				}
			}
		},
		{ConditionID.frz,
			new Condition()
			{
				Name= "Freeze",
					StartMessage="has been frozen",
				OnBeforeMove = (Pokemon pokemon) =>
				{
					if (Random.Range(1, 5) == 1){
						pokemon.CureStatus();
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is not frozen anymore");
						return true;
					}
					pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s frozen and can't move");
					return false;
				}
			}
		},
		{ConditionID.slp,
			new Condition()
			{
				Name= "Sleep",
				StartMessage="has fallen asleep",
				OnStart = (Pokemon pokemon) =>
				{
					pokemon.StatusTime = Random.Range(1, 4);
					Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
				},
				OnBeforeMove = (Pokemon pokemon) =>
				{
					if (pokemon.StatusTime <= 0)
					{
						pokemon.CureStatus();
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
						return true;
					}

					pokemon.StatusTime--;
					pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
					return false;
				}
			}
		},
		{ConditionID.rest,
			new Condition()
			{
				Name= "Rest",
				StartMessage="has fallen asleep and healed to full",
				OnStart = (Pokemon pokemon) =>
				{
					pokemon.StatusTime = 2;
					pokemon.HealToFull();
					Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
				},
				OnBeforeMove = (Pokemon pokemon) =>
				{
					if (pokemon.StatusTime <= 0)
					{
						pokemon.CureStatus();
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
						return true;
					}

					pokemon.StatusTime--;
					pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
					return false;
				}
			}
		},
		{ConditionID.tox,
			new Condition()
			{
				Name= "Toxic",
				StartMessage="has been badly poisoned",
				OnStart = (Pokemon pokemon) =>
				{
					pokemon.StatusTime = 1;
				},
				OnAfterTurn = (Pokemon pokemon) =>
				{
					pokemon.UpdateHP(Mathf.FloorToInt((pokemon.StatusTime * 6.25f * pokemon.MaxHP) / 100.0f));
					pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to toxic poison");
					pokemon.StatusTime++;
				}
			}
		},
		//Volatile Status Conditions
		{ConditionID.confusion,
			new Condition()
			{
				Name= "Confusion",
				StartMessage="is now confused",
				OnStart = (Pokemon pokemon) =>
				{
					//Confused for 1-4 turns
					pokemon.VolatileStatusTime = Random.Range(1, 5);
					Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} moves");
				},
				OnBeforeMove = (Pokemon pokemon) =>
				{
					if (pokemon.VolatileStatusTime <= 0)
					{
						pokemon.CureVolatileStatus();
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} isn't confused anymore!");
						return true;
					}
					pokemon.VolatileStatusTime--;
					pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused!");

					//50% chance of taking damage
					if(Random.Range(1,3) == 1) return true;

					//Hurt by confusion
					pokemon.UpdateHP(pokemon.MaxHP/8);
					pokemon.StatusChanges.Enqueue("It's so confused that it has hurt itself!");
					return false;
				}
			}
		},
	};

}
public enum ConditionID
{
	none, psn, brn, slp, par, frz, rest, tox,
	confusion
}