using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerRoguelike.TerPlayer;
using TerRoguelike.Projectiles;
using MetaRoguelikeAddon.MetaRoguelikeAddonPlayer;
using static Terraria.Audio.SoundEngine;
using static Terraria.Main;
using static Terraria.ModLoader.DamageClass;
using static Terraria.ModLoader.ModContent;
using static Terraria.Projectile;
using static TerRoguelike.Utilities.TerRoguelikeUtils;

namespace MetaRoguelikeAddon.Content.Projectiles
{
    public class ModdedAdaptiveBladeHoldout : ModProjectile
    {
        //This manages whatever happens when you hold down with adaptive blade
        //public override string Texture => "TerRoguelike/Projectiles/InvisibleProj";

        public ref float Charge => ref Projectile.ai[0];

        public bool autoRelease = false;
        public Player Owner => player[Projectile.owner];
        public TerRoguelikePlayer trmodPlayer;
        public MRLPlayer modPlayer;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            trmodPlayer ??= Owner.ModPlayer();

            if (Charge >= 60f && !autoRelease) //cap chargetime if no autorelease
            {
                Charge = 60f;
            }

            var pointingRotation = (AimWorld() - Owner.MountedCenter).ToRotation();
            Projectile.Center = Owner.MountedCenter + pointingRotation.ToRotationVector2() * 16f;

            if (Owner.channel) //Keep the player's hands full relative to attack speed
            {
                Projectile.timeLeft = 2;
                Owner.itemTime = (int) (20 / Owner.GetAttackSpeed(Generic)) + 2;
                if (Owner.itemTime < 2)
                {
                    Owner.itemTime = 2;
                }

                Owner.itemAnimation = (int) (20 / Owner.GetAttackSpeed(Generic)) + 2;
                if (Owner.itemAnimation < 2)
                {
                    Owner.itemAnimation = 2;
                }

                Owner.heldProj = Projectile.whoAmI;
            }
            else // player released m1. swing it.
            {
                ReleaseSword();
                return;
            }

            if (autoRelease && Charge >= 60f) // autorelease swing sword
            {
                ReleaseSword();
            }


            // charge. autorelease allows overflowing of charge amount, leading to more than 1 swing a frame
            if (!(Charge < 60) && !autoRelease) return;

            var chargeAmt = 1f * Owner.GetAttackSpeed(Generic);
            autoRelease = chargeAmt >= 3f;

            Charge += chargeAmt;
            if (!(Charge >= 60f)) return;

            PlaySound(new SoundStyle("TerRoguelike/Sounds/Ding") { Volume = 0.1f }, Owner.Center);
            trmodPlayer.bladeFlashTime = 15;
        }

        public void ReleaseSword()
        {
            while (true)
            {
                if ((Charge <= 60f || (Owner.channel && autoRelease)) && trmodPlayer.swingAnimCompletion == 0)
                {
                    trmodPlayer.swingAnimCompletion += float.Epsilon; // start the swing anim
                }

                var shotsToFire = Owner.ModPlayer().shotsToFire; //multishot support
                var damageBoost = Charge >= 60f ? 4f : 1 + Charge / 60f * 2f;
                var damage = (int) (Projectile.damage * damageBoost);
                PlaySound(SoundID.Item1 with { Volume = SoundID.Item41.Volume * 1f });
                for (var i = 0; i < shotsToFire; i++)
                {
                    float mainAngle;
                    const float spread = 20f;
                    if (shotsToFire == 1)
                    {
                        mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation();
                    }
                    else if (shotsToFire % 2 == 0)
                    {
                        mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation() - (shotsToFire - 1) * 2 * MathHelper.Pi / (spread * 4f) + i * MathHelper.Pi / spread;
                    }
                    else
                    {
                        mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation() - (shotsToFire - 1) / 2f * MathHelper.Pi / spread + i * MathHelper.Pi / spread;
                    }

                    var direction = mainAngle.ToRotationVector2();
                    var spawnedProjectile = NewProjectile(Projectile.GetSource_FromThis(), Owner.MountedCenter + direction * 32f, Vector2.Zero, ProjectileType<AdaptiveBladeSlash>(), damage, 1f, Owner.whoAmI);
                    var spawnedProj = projectile[spawnedProjectile];
                    spawnedProj.rotation = direction.ToRotation();
                    spawnedProj.scale = 3 * trmodPlayer.scaleMultiplier - 1;
                    spawnedProj.ModProj().swingDirection = Owner.direction;
                    spawnedProj.ModProj().notedBoostedDamage = damageBoost;
                }

                Charge -= 60f;
                // support for swinging more than once a frame if one has that much attack speed
                if (Charge <= 60f) break;
            }
        }
    }
}