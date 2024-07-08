using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerRoguelike.Projectiles;
using TerRoguelike.Managers;
using TerRoguelike.Systems;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using static TerRoguelike.Utilities.TerRoguelikeUtils;
using Terraria.GameInput;
using MetaRoguelikeAddon.Content.Projectiles;

namespace MetaRoguelikeAddon.Content.Items{
	public class AdaptiveSMG : ModItem, ILocalizedModType{
		public override void SetDefaults(){
			Item.damage = 100;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 62;
			Item.height = 32;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.channel = true;
			Item.knockBack = 5f;
			Item.rare = ItemRarityID.Cyan;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<AdaptiveSMGHoldout>();
			Item.shootSpeed = 16f;
		}

		public override bool CanUseItem(Player player){
			return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<AdaptiveSMGHoldout>());
		}

		public override void UseItemFrame(Player player){
			//Calculate the dirction in which the players arms should be pointing at.
			Vector2 playerToCursor = (AimWorld() - player.Center).SafeNormalize(Vector2.UnitX);
			float armPointingDirection = (playerToCursor.ToRotation());

			player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
			CleanHoldStyle(player, player.compositeFrontArm.rotation + MathHelper.PiOver2, player.GetFrontHandPosition(player.compositeFrontArm.stretch, player.compositeFrontArm.rotation).Floor(), new Vector2(42, 30), new Vector2(-12, -4));
			if (AimWorld().X > player.Center.X){
				player.ChangeDir(1);
			}
			else{
				player.ChangeDir(-1);
			}
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame){
			if (AimWorld().X > player.Center.X){
				player.ChangeDir(1);
			}
			else{
				player.ChangeDir(-1);
			}
		}
	}
}