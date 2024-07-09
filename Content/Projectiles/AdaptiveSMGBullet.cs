using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerRoguelike.TerPlayer;
using Terraria.GameContent;
using static TerRoguelike.Managers.TextureManager;
using static TerRoguelike.Utilities.TerRoguelikeUtils;
using TerRoguelike.Projectiles;
using MetaRoguelikeAddon.MetaRoguelikeAddonPlayer;
using Terraria.DataStructures;
using static Microsoft.Xna.Framework.Graphics.SpriteEffects;
using static Microsoft.Xna.Framework.MathHelper;
using static Terraria.ID.ProjectileID.Sets;
using static Terraria.Main;
using static Terraria.ModLoader.DamageClass;
using static Terraria.NPC;

namespace MetaRoguelikeAddon.Content.Projectiles
{
    public class AdaptiveSMGBullet : ModProjectile
    {
        public bool ableToHit = true;
        public TerRoguelikeGlobalProjectile modProj;
        MRLPlayer modPlayer;
        TerRoguelikePlayer trmodPlayer;
        public int setTimeLeft = 400;

        public override void SetStaticDefaults()
        {
            TrailingMode[Projectile.type] = 2;
            TrailCacheLength[Projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = Ranged;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = setTimeLeft;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            modProj = Projectile.ModProj();
        }

        public override bool? CanDamage() => ableToHit ? null : false;

        public override bool? CanHitNPC(NPC target)
        {
            // used for not immediately cutting off afterimages when the projectile would in normal circumstances be killed.
            // allows the afterimages to visually catch up so that the bullet always visually looks like it reached a point.
            return Projectile.penetrate != 1 ? null : false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = Projectile.ai[0];
            //scale support
            Projectile.position = Projectile.Center + new Vector2(-2 * Projectile.scale, -2 * Projectile.scale);
            Projectile.width = (int) (4 * Projectile.scale);
            Projectile.height = (int) (4 * Projectile.scale);
            Projectile.knockBack = 0;
            trmodPlayer = player[Projectile.owner].ModPlayer();
        }

        public override void AI()
        {
            if (trmodPlayer.heatSeekingChip > 0)
            {
                modProj.HomingAI(Projectile,
                    (float) Math.Log(trmodPlayer.heatSeekingChip + 1, 1.2d) / (833f * Projectile.MaxUpdates));
            }

            if (trmodPlayer.bouncyBall > 0)
            {
                modProj.extraBounces += trmodPlayer.bouncyBall;
            }

            if (Projectile.timeLeft <= 2 * Projectile.MaxUpdates)
            {
                ableToHit = false;
                Projectile.velocity = Vector2.Zero;
                return;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var lightTexture = TextureAssets.Projectile[Type].Value;
            var deathTime = 2 * Projectile.MaxUpdates;
            var innateOffset = lightTexture.Size() * 0.5f * Projectile.scale - screenPosition;

            Vector2 drawPosition;
            for (var i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                if (i > 0 && Projectile.oldPos[i + 1] == Vector2.Zero) continue;

                for (var j = 0; j < 8; j++)
                {
                    var progress = i * 8 + j;
                    if (Projectile.timeLeft <= deathTime && Projectile.timeLeft > progress + 5) continue;

                    var jCompletion = j / 8f;
                    var completion = (float) progress / 64;
                    var color = Color.Lerp(Color.White, Color.Red, (completion * 3));
                    drawPosition = Projectile.oldPos[i] + innateOffset;
                    var offset = (Projectile.oldPos[i + 1] - Projectile.oldPos[i]) * jCompletion;

                    var scale = new Vector2(0.8f) * Lerp(0.25f, 1f, 1f - completion);
                    EntitySpriteDraw(lightTexture, drawPosition + offset, null, color,
                        Projectile.oldRot[i], lightTexture.Size() * 0.5f, scale * Projectile.scale,
                        None, 0);
                }
            }

            if (trmodPlayer == null) return false;
            if (trmodPlayer.volatileRocket <= 0 || Projectile.velocity == Vector2.Zero) return false;
            
            var rocketTexture = TexDict["VolatileRocket"];
            drawPosition = Projectile.Center - screenPosition;

            EntitySpriteDraw(rocketTexture, drawPosition, null, Color.White,
                Projectile.velocity.ToRotation(), rocketTexture.Size() * 0.5f, Projectile.scale,
                None);

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref HitModifiers modifiers)
        {
            //currently, no piercing is available for this.
            Projectile.timeLeft = 2 * Projectile.MaxUpdates;
            ableToHit = false;
            Projectile.velocity = Vector2.Zero;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            modProj.bounceCount++;
            if (modProj.bounceCount >= 1 + modProj.extraBounces)
            {
                if (oldVelocity != Vector2.Zero)
                {
                    Projectile.Center = TileCollidePositionInLine(Projectile.Center,
                        Projectile.Center + oldVelocity.SafeNormalize(Vector2.UnitY) * 16);
                }

                if (Projectile.timeLeft > 2 * Projectile.MaxUpdates)
                {
                    Projectile.timeLeft = 2 * Projectile.MaxUpdates;
                }

                ableToHit = false;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                // If the projectile hits the left or right side of the tile, reverse the X velocity
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                    Projectile.timeLeft = setTimeLeft;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                if (!(Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)) return false;
                Projectile.velocity.Y = -oldVelocity.Y;
                Projectile.timeLeft = setTimeLeft;
            }

            return false;
        }
    }
}