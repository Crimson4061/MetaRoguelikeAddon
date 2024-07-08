using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerRoguelike.Projectiles;
using TerRoguelike.Managers;
using TerRoguelike.Systems;
using TerRoguelike.TerPlayer;
using MetaRoguelikeAddon.Content.Projectiles;

using static TerRoguelike.Utilities.TerRoguelikeUtils;

namespace MetaRoguelikeAddon.Content.Items
{
	public class ModdedAdaptiveBlade : ModItem, ILocalizedModType
	{
		public override void SetDefaults()
		{
			Item.damage = 100;
			Item.DamageType = DamageClass.Melee;
			Item.width = 76;
			Item.height = 76;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.channel = true;
			Item.knockBack = 5f;
			Item.rare = ItemRarityID.Cyan;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ModdedAdaptiveBladeHoldout>();
			Item.shootSpeed = 32f;
		}

		public override bool CanUseItem(Player player)
		{
			return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<ModdedAdaptiveBladeHoldout>());
		}

		public override void UseItemFrame(Player player)
		{
			TerRoguelikePlayer trmodPlayer = player.GetModPlayer<TerRoguelikePlayer>();

			if (AimWorld().X > player.Center.X && trmodPlayer.swingAnimCompletion <= 0)
			{
				player.ChangeDir(1);
			}
			else if (AimWorld().X <= player.Center.X && trmodPlayer.swingAnimCompletion <= 0)
			{
				player.ChangeDir(-1);
			}
			trmodPlayer.lockDirection = true;

			//Calculate the dirction in which the players arms should be pointing at.
			if (trmodPlayer.swingAnimCompletion <= 0 || trmodPlayer.playerToCursor == Vector2.Zero)
				trmodPlayer.playerToCursor = (AimWorld() - player.Center).SafeNormalize(Vector2.UnitX);
			float armPointingDirection = (trmodPlayer.playerToCursor.ToRotation() - (MathHelper.Pi * player.direction / 3f));
			if (trmodPlayer.swingAnimCompletion > 0)
			{
				trmodPlayer.swingAnimCompletion += 1f / (20f / player.GetAttackSpeed(DamageClass.Generic));
				if (trmodPlayer.swingAnimCompletion > 1f)
					trmodPlayer.swingAnimCompletion = 1f;
				armPointingDirection += MathHelper.Lerp(0f, MathHelper.TwoPi * 9f / 16f, trmodPlayer.swingAnimCompletion) * player.direction;
				if (trmodPlayer.swingAnimCompletion >= 1f)
				{
					trmodPlayer.swingAnimCompletion = 0;
					trmodPlayer.playerToCursor = Vector2.Zero;
					trmodPlayer.lockDirection = false;
				}
					
			}
			if (player.itemTime == 1)
			{
				trmodPlayer.swingAnimCompletion = 0;
				trmodPlayer.playerToCursor = Vector2.Zero;
				trmodPlayer.lockDirection = false;
			}
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
			CleanHoldStyle(player, player.compositeFrontArm.rotation + MathHelper.PiOver2, player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation).Floor(), new Vector2(76, 76), new Vector2(-28, 28));
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			TerRoguelikePlayer trmodPlayer = player.GetModPlayer<TerRoguelikePlayer>();

			if (AimWorld().X > player.Center.X && trmodPlayer.swingAnimCompletion <= 0)
			{
				player.ChangeDir(1);
			}
			else if (AimWorld().X <= player.Center.X && trmodPlayer.swingAnimCompletion <= 0)
			{
				player.ChangeDir(-1);
			}
		}
	}
}