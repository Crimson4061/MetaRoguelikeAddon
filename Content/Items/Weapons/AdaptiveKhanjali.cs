using System.Linq;
using Microsoft.Xna.Framework;
using static Microsoft.Xna.Framework.MathHelper;
using static Microsoft.Xna.Framework.Vector2;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.DamageClass;
using static Terraria.Player.CompositeArmStretchAmount;
using TerRoguelike.TerPlayer;
using static TerRoguelike.Utilities.TerRoguelikeUtils;
using MetaRoguelikeAddon.Content.Projectiles;
using MetaRoguelikeAddon.MetaRoguelikeAddonPlayer;

namespace MetaRoguelikeAddon.Content.Items.Weapons;

public class AdaptiveKhanjali : ModItem
{
	MRLPlayer modPlayer;
	public override void SetDefaults()
	{
		Item.damage = 90;
		Item.DamageType = Melee;
		Item.width = 38;
		Item.height = 38;
		Item.useTime = 12;
		Item.useAnimation = 12;
		Item.useStyle = ItemUseStyleID.Rapier;
		Item.noMelee = true;
		Item.channel = true;
		Item.knockBack = 5f;
		Item.rare = ItemRarityID.Yellow;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<AdaptiveKhanjaliHoldout>();
		Item.shootSpeed = 19f;
	}
}