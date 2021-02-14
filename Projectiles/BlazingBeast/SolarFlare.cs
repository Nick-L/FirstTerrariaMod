using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

/*
 * I
 * https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile
 */
namespace TestMod.Projectiles.BlazingBeast
{
    public class SolarFlare : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Flare");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 100;
            //The ai style of the projectile, please reference the source code of Terraria
            projectile.aiStyle = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            projectile.penetrate = 5;
            //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
        }
        
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (target.hurtCooldowns[1] <= 0)
            {
                target.AddBuff(BuffID.Burning, 60);
            }
        }
        
        
        public override void AI()
        {
            // This is used for counter as it syncs to server automatically so multiplayer doesn't go out of sync
            projectile.ai[0] += 1;
            if (projectile.ai[0] >= 30)
            {
                projectile.velocity.Y += 0.1f;
            }
        }
        
    }
}
