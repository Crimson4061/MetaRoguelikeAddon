using Terraria;
using Terraria.ModLoader;
using TerRoguelike;
using System.Collections;
using System.Collections.Generic;
using TerRoguelike.TerPlayer;
using MetaRoguelikeAddon.Content.Items;
using MetaRoguelikeAddon.Content.Projectiles;

namespace MetaRoguelikeAddon.MetaRoguelikeAddonPlayer{

	public class MRLPlayer : ModPlayer{
		private ModPlayer trmodPlayeraaaaa;
		public Item adBladeRef;
		public float adBladeScale = 1f;
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath){
			static Item createItem(int itemtype){
				Item newitem = new Item();
				newitem.SetDefaults(itemtype);
				return newitem;
			}

			IEnumerable<Item> itemlist = new List<Item>(){
				createItem(ModContent.ItemType<AdaptiveSMG>()),
				createItem(ModContent.ItemType<ModdedAdaptiveBlade>())
			};
			return itemlist;
		}
		public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
		{
			itemsByMod["TerRoguelike"].RemoveAt(1);
		}
	}
}