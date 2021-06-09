using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsDB
{
	public static void Init()
	{
		foreach (var kvp in Items)
		{
			var itemId = kvp.Key;
			var item = kvp.Value;

			item.Id = itemId;
		}
	}

	public static Dictionary<ItemID, Item> Items { get; set; } = new Dictionary<ItemID, Item>()
	{
        {ItemID.fullHeal,
			new Item()
            {
				Name= "Full Heal",
				StartMessage="'s statuses have been healed",
				OnUse = (Pokemon pokemon) =>
                {

					if (pokemon.Status == null && pokemon.VolatileStatus == null)
                    {
						pokemon.StatusChanges.Enqueue($"Full heal had no effect on {pokemon.Base.Name}");
					}
					if (pokemon.Status != null)
                    {
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s {pokemon.Status.ToString()} was healed");
						pokemon.CureStatus();
					}
					if (pokemon.VolatileStatus != null)
					{
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s {pokemon.VolatileStatus.ToString()} was healed");
						pokemon.CureVolatileStatus();
					}
                }
            }
        },
		{ItemID.maxPotion,
			new Item()
            {
				Name= "Max Potion",
				StartMessage=" has been healed to full",
				OnUse = (Pokemon pokemon) =>
                {
					pokemon.HealToFull();
					if (pokemon.HpChanged)
                    {
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} was healed to full");
                    }
					else
                    {
						pokemon.StatusChanges.Enqueue($"Max Potion had no effect on {pokemon.Base.Name}");
					}
                }
            }
		}
	};
}

public enum ItemID
{
	fullHeal, maxPotion
}
