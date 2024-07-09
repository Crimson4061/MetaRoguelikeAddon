using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using static TerRoguelike.Utilities.TerRoguelikeUtils;
using MetaRoguelikeAddon.Content.Projectiles;
using static Microsoft.Xna.Framework.MathHelper;
using static Terraria.Player.CompositeArmStretchAmount;

namespace MetaRoguelikeAddon.Content.Items;

public class AdaptiveSMG : ModItem
{
    public override void SetDefaults()
    {
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

    public override bool CanUseItem(Player player)
    {
        return !Main.projectile.Any(n
            => n.active && n.owner == player.whoAmI
                        && n.type == ModContent.ProjectileType<AdaptiveSMGHoldout>());
    }

    public override void UseItemFrame(Player player)
    {
        //Calculate the dirction in which the players arms should be pointing at.
        var playerToCursor = (AimWorld() - player.Center).SafeNormalize(Vector2.UnitX);
        var armPointingDirection = playerToCursor.ToRotation();

        player.SetCompositeArmBack(true, Full, armPointingDirection - PiOver2);
        player.SetCompositeArmFront(true, Full, armPointingDirection - PiOver2);
        CleanHoldStyle(player, player.compositeFrontArm.rotation + PiOver2,
            player
                .GetFrontHandPosition(player.compositeFrontArm.stretch,
                    player.compositeFrontArm.rotation)
                .Floor(), new Vector2(42, 30), new Vector2(-12, -4));
        player.ChangeDir(AimWorld().X > player.Center.X ? 1 : -1);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
        => player.ChangeDir(AimWorld().X > player.Center.X ? 1 : -1);
}