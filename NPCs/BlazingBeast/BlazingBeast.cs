using IL.Terraria.World.Generation;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TestMod.Projectiles.BlazingBeast;
using static Terraria.ModLoader.ModContent;


/*
 * Reference for public members of ModNPC class
 * https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_mod_n_p_c.html
 */

namespace TestMod.NPCs.BlazingBeast
{
    [AutoloadBossHead]
    public class BlazingBeast : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Beast");
        }

        public override void SetDefaults()
        {
            // Set width/height to match image
            npc.width = 100;
            npc.height = 100;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0;
            // -1 means custom ai
            npc.aiStyle = -1;
            npc.lifeMax = 2100;
            npc.defense = 8;
            // TODO Change damage once ai etc is done
            npc.damage = 1;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            // TODO these are placeholders change once balancing starts
            npc.damage = 1;
            npc.lifeMax = (int)(2100 * numPlayers);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void NPCLoot()
        {
            // TODO add loot (maybe none)
            Main.dayTime = false;
        }

        #region AI variables 
        private const int maxSpeed = 10;
        private int gameTicksCount = 0;
        private int solarRayCount = 0;
        private Phase bossPhase = Phase.chase;
        readonly Phase[] phasesList = new Phase[] { Phase.blind, Phase.chase, Phase.solarFlare };

        private enum Phase
        {
            changePhase,
            chase,
            blind,
            solarFlare,
            summonMinions,
            enrage
        }
        #endregion

        public override void AI()
        {
            this.npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            if (!player.active || player.dead)
            {
                DespawnBoss(this.npc);
            }
            else
            {
                switch (bossPhase) {
                    case Phase.changePhase:
                        this.npc.velocity = new Vector2(0, 0);
                        bossPhase = (Phase)phasesList.GetValue(Main.rand.Next(phasesList.Length));
                        break;
                    case Phase.chase:
                        ChaseAI(player);
                        break;
                    case Phase.blind:
                        BlindAI(player);
                        break;
                    case Phase.solarFlare:
                        SolarRayAI(player);
                        break;
                    case Phase.summonMinions:
                        SummonMinionsAI(player);
                        break;
                    case Phase.enrage:
                        EnrageAI(player);
                        break;
                    default:
                        bossPhase = Phase.changePhase;
                        break;
                }
                gameTicksCount++;
            }
        }

        #region AI Phases
        private void ChaseAI(Player player)
        {
            if (gameTicksCount > 50)
            {
                this.npc.velocity = PathBossTowardsTarget(player.position, maxSpeed);
                gameTicksCount = 0;

                // 10% chance to change phase 
                if (Main.rand.Next(10) > 8)
                {
                    bossPhase = Phase.changePhase;
                }
            }
        }

        private void BlindAI(Player player)
        {
            // TODO Change this to use custom debuff once implemented
            player.AddBuff(BuffID.Blackout, (60 * 5));
            if (gameTicksCount > 60)
            {
                gameTicksCount = 0;
                bossPhase = Phase.changePhase;
            }
        }

        private void SolarRayAI(Player player)
        {
            if(gameTicksCount > 80)
            {
                // Teleport boss to directly above player
                this.npc.position = new Vector2((player.position.X - (npc.width/2)), (player.position.Y - 400));

                Projectile.NewProjectile(npc.position.X + npc.width/2, npc.position.Y, 0f, 0f, ProjectileType<SolarFlare>(), 1, 0f, Main.myPlayer, npc.whoAmI, 0);

                // Generate projectiles to the right and left of the boss
                for (int i = 1; i < 8; i++)
                {
                    Projectile.NewProjectile(npc.position.X - (300 * i) , npc.position.Y, 0f, 0f, ProjectileType<SolarFlare>(), 1, 0f, Main.myPlayer, npc.whoAmI, 0);
                    Projectile.NewProjectile(npc.position.X + (300 * i), npc.position.Y, 0f, 0f, ProjectileType<SolarFlare>(), 1, 0f, Main.myPlayer, npc.whoAmI, 0);
                }

                // Shoot solar rays 3 times then change phase
                if (solarRayCount < 3)
                {
                    solarRayCount++;
                }
                else
                {
                    solarRayCount = 0;
                    bossPhase = Phase.changePhase;
                }
                gameTicksCount = 0;
            }
            
        }

        /*
         * TODO not currently implemented
         * Add to phasesList array once it is implemented should currently be unreachable
         */
        private void SummonMinionsAI(Player player)
        {
            throw new NotImplementedException();
        }

        // TODO potentially change this so it isn't just faster version of chase
        private void EnrageAI(Player player)
        {
            if (gameTicksCount > 50)
            {
                this.npc.velocity = PathBossTowardsTarget(player.position, maxSpeed * 5);
                gameTicksCount = 0;
            }
        }
        #endregion

        /*
         * Potentially move these to their own file/folder depending how many are needed for other bosses/npcs for reusability
         */
        #region UtilityFunctions

        private void DespawnBoss(NPC boss)
        {
            boss.TargetClosest();

            if (!Main.player[npc.target].active || Main.player[npc.target].dead)
            {
                boss.noTileCollide = true;
                boss.velocity = new Vector2(0, 10);

                if (boss.timeLeft > 10)
                {
                    boss.timeLeft = 10;
                }
                return;
            }
        }

        private Vector2 PathBossTowardsTarget(Vector2 target, int bossSpeed)
        {
            Vector2 newBossVelocity = new Vector2(target.X - this.npc.position.X, target.Y - this.npc.position.Y);
            newBossVelocity.Normalize();

            return newBossVelocity *= bossSpeed;
        }
        #endregion
    }
}