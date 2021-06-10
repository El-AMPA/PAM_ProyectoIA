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
				Name= "Cura total",
				StartMessage="ha usado cura total",
				OnUse = (Pokemon pokemon) =>
                {

					if (pokemon.Status == null && pokemon.VolatileStatus == null)
                    {
						pokemon.StatusChanges.Enqueue($"La cura total no ha tenido efecto en {pokemon.Base.Name}.");
					}
					if (pokemon.Status != null)
                    {
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} se ha recuperado del estado {pokemon.Status.ToString()}");
						pokemon.CureStatus();
					}
					if (pokemon.VolatileStatus != null)
					{
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} se ha recuperado del estado {pokemon.VolatileStatus.ToString()}");
						pokemon.CureVolatileStatus();
					}
                }
            }
        },
		{ItemID.maxPotion,
			new Item()
            {
				Name= "Poción Máxima",
				StartMessage="ha usado una poción máxima",
				OnUse = (Pokemon pokemon) =>
                {
					pokemon.HealToFull();
					if (pokemon.HpChanged)
                    {
						pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} se ha curado del todo");
                    }
					else
                    {
						pokemon.StatusChanges.Enqueue($"La poción máxima no ha tenido efecto en {pokemon.Base.Name}");
					}
                }
            }
		}
	};
}

public enum ItemID
{
	NULL, fullHeal, maxPotion
}
