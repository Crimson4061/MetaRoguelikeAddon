using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TerRoguelike.TerPlayer;
using static TerRoguelike.Utilities.TerRoguelikeUtils;
using MetaRoguelikeAddon.MetaRoguelikeAddonPlayer;
using static System.Math;
using static Microsoft.Xna.Framework.MathHelper;
using static Terraria.Audio.SoundEngine;
using static Terraria.Collision;
using static Terraria.ID.SoundID;
using static Terraria.Main;
using static Terraria.ModLoader.DamageClass;
using static Terraria.ModLoader.ModContent;
using static Terraria.Projectile;

namespace MetaRoguelikeAddon.Content.Projectiles
{
    public class AdaptiveSMGHoldout : ModProjectile
    {
        //This manages whatever happens when you hold down with adaptive gun
        //public override string Texture => "TerRoguelike/Projectiles/InvisibleProj";

        public ref float Charge => ref Projectile.ai[0];

        public Player Owner => player[Projectile.owner];
        public MRLPlayer modPlayer;
        public TerRoguelikePlayer trmodPlayer;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            /*if (modPlayer == null)
            {
                modPlayer = Owner.ModPlayer();
            }*/
            trmodPlayer = Owner.GetModPlayer<TerRoguelikePlayer>();
            if (Owner.channel) //Keep the player's hands full relative to attack speed
            {
                Projectile.timeLeft = 2;
                Owner.itemTime = (int) (20 / Owner.GetAttackSpeed(Generic));
                if (Owner.itemTime < 2)
                {
                    Owner.itemTime = 2;
                }

                Owner.itemAnimation = (int) (20 / Owner.GetAttackSpeed(Generic));
                if (Owner.itemAnimation < 2)
                {
                    Owner.itemAnimation = 2;
                }

                Owner.heldProj = Projectile.whoAmI;
            }

            var pointingRotation = (AimWorld() - Owner.MountedCenter).ToRotation();
            Projectile.Center = Owner.MountedCenter + pointingRotation.ToRotationVector2() * 40f;


            if (Charge > 0f) // attack when charge is full. scales great with attack speed
            {
                ShootBullet();
            }

            Charge += 2.2f * Owner.GetAttackSpeed(Generic); //increase charge relative to attack speed
        }
        
        public void ShootBullet()
        {
            while (true)
            {
                var distance = CanHit(Owner.MountedCenter, 1, 1, 
                    Projectile.Center, 1, 1) ? 30f : 5f;
                var shotsToFire = Owner.ModPlayer().shotsToFire; //multishot support
                
                if (trmodPlayer.shotgunComponent > 0)
                {
                    PlaySound(Item36 with { Volume = Item36.Volume * 0.6f });
                }
                else if (trmodPlayer.sniperComponent > 0)
                {
                    PlaySound(Item40 with { Volume = Item40.Volume * 0.6f });
                }
                else if (trmodPlayer.minigunComponent > 0)
                {
                    PlaySound(Item11 with { Volume = Item11.Volume * 0.6f });
                }
                else
                {
                    PlaySound(Item41 with { Volume = Item41.Volume * 0.6f });
                }

                for (var i = 0; i < shotsToFire; i++)
                {
                    float mainAngle;
                    const float spread = 90f;
                    if (shotsToFire == 1)
                    {
                        mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation();
                    }
                    else if (shotsToFire % 2 == 0)
                    {
                        mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation()
                            - (shotsToFire - 1) * 2 * Pi / (spread * 4f) + i * Pi / spread;
                    }
                    else
                    {
                        mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation() 
                            - (shotsToFire - 1) / 2f * Pi / spread + i * Pi / spread;
                    }

                    mainAngle += rand.NextFloat(-Pi * 0.01f, Pi * 0.01f + float.Epsilon);
                    if (trmodPlayer.shotgunComponent > 0)
                    {
                        mainAngle += rand.NextFloat(-Pi * 0.01f, Pi * 0.01f + float.Epsilon);
                    }

                    if (trmodPlayer.minigunComponent > 0)
                    {
                        mainAngle += rand.NextFloat(-Pi * 0.025f, Pi * 0.025f + float.Epsilon);
                    }

                    mainAngle += rand.NextFloat(-Pi * 0.015f, Pi * 0.015f + float.Epsilon);
                    var direction = mainAngle.ToRotationVector2();
                    NewProjectile(Projectile.GetSource_FromThis(), 
                        Owner.MountedCenter + direction * distance + Vector2.UnitY * Owner.gfxOffY, 
                        direction * 11.25f, ProjectileType<AdaptiveSMGBullet>(), 
                        (int) Round(Projectile.damage * 0.4f - rand.NextFloat(-2f, 2f)), 1f, Owner.whoAmI, 
                        trmodPlayer.scaleMultiplier);
                }

                Charge -= 20f;
                if (!(Charge > 0f)) return; // if the player has enough attack speed to shoot more than once a frame, allow it.
            }
        }
    }
}