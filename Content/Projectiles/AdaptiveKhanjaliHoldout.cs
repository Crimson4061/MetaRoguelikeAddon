using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Audio.SoundEngine;
using static Terraria.Main;
using static Terraria.ModLoader.DamageClass;
using static Terraria.ModLoader.ModContent;
using static Terraria.Projectile;
using TerRoguelike.TerPlayer;
using TerRoguelike.Projectiles;
using static TerRoguelike.Utilities.TerRoguelikeUtils;
using MetaRoguelikeAddon.MetaRoguelikeAddonPlayer;
using MetaRoguelikeAddon.Content.Projectiles;

namespace MetaRoguelikeAddon.Content.Projectiles;

public class AdaptiveKhanjaliHoldout : ModProjectile
{
	public ref float Charge => ref Projectile.ai[0];
	public float reqCharge = 360f/13f;
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

		var pointingRotation = (AimWorld() - Owner.MountedCenter).ToRotation();
		Projectile.Center = Owner.MountedCenter + pointingRotation.ToRotationVector2() * 19f;

		if (Owner.channel) //Keep the player's hands full relative to attack speed
		{
			Projectile.timeLeft = 2;
			Owner.itemTime = (int) (9f / Owner.GetAttackSpeed(Generic)) + 1;
			Owner.itemAnimation = (int) (9f / Owner.GetAttackSpeed(Generic)) + 1;
			Owner.heldProj = Projectile.whoAmI;
		}

		var chargeAmt = (float)(Owner.GetAttackSpeed(Generic)*13f/6f);
		Charge += chargeAmt;
		if (Charge < reqCharge) return;

		PlaySound(SoundID.Item69 with { Volume = 0.1f }, Owner.Center); //idk what sound to use here and i don't feel like importing file.wav rn
		Stab();
	}

	public void Stab()
	{
		while (true)
		{
			/*if ((Charge <= reqCharge) && trmodPlayer.swingAnimCompletion == 0)
			{
				trmodPlayer.swingAnimCompletion += float.Epsilon; // start the swing anim
			}*/

			var shotsToFire = Owner.ModPlayer().shotsToFire; //multishot support
			var damage = (int)Projectile.damage;
			PlaySound(SoundID.Item1 with { Volume = SoundID.Item41.Volume * 1f });
			for (var i = 0; i < shotsToFire; i++)
			{
				float mainAngle;
				const float spread = 40f;
				if (shotsToFire == 1)
				{
					mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation();
				}
				else if (shotsToFire % 2 == 0)
				{
					mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation() - (shotsToFire - 1) * MathHelper.Pi / (spread * 2f) + i * MathHelper.Pi / spread;
				}
				else
				{
					mainAngle = (Projectile.Center - Owner.MountedCenter).ToRotation() - (shotsToFire - 1) / 2f * MathHelper.Pi / spread + i * MathHelper.Pi / spread;
				}

				var direction = mainAngle.ToRotationVector2();
				var spawnedProjectile = NewProjectile(Projectile.GetSource_FromThis(), Owner.MountedCenter + direction * 19f, Vector2.Zero, ProjectileType<AdaptiveKhanjaliStab>(), damage, 1f, Owner.whoAmI);
				var spawnedProj = projectile[spawnedProjectile];
				spawnedProj.rotation = direction.ToRotation();
				spawnedProj.scale = trmodPlayer.scaleMultiplier;
				spawnedProj.ModProj().swingDirection = Owner.direction;
			}

			Charge -= reqCharge;
			// support for swinging more than once a frame if one has that much attack speed
			if (Charge <= reqCharge) break;
		}
	}
}