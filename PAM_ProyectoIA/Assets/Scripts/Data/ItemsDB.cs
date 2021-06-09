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
					pokemon.CureStatus();
					pokemon.CureVolatileStatus();
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
					//Curar al pokemon
                }
            }
		}
	};
}

public enum ItemID
{
	fullHeal, maxPotion
}
