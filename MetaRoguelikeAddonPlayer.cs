using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using MetaRoguelikeAddon.Content.Items.Weapons;

namespace MetaRoguelikeAddon.MetaRoguelikeAddonPlayer;

public class MRLPlayer : ModPlayer
{
	public Item adBladeRef;
	public float adBladeScale = 1f;

	public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
	{
		static Item createItem(int itemtype)
		{
			Item newitem = new();
			newitem.SetDefaults(itemtype);
			return newitem;
		}

		return
		[
			createItem(ModContent.ItemType<AdaptiveSMG>()),
			createItem(ModContent.ItemType<ModdedAdaptiveBlade>()),
			createItem(ModContent.ItemType<AdaptiveKhanjali>())
		];
	}

	public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod,
		bool mediumCoreDeath)
	{
		itemsByMod["TerRoguelike"].RemoveAt(1);
	}
}