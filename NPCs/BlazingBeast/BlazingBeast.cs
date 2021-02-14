using IL.Terraria.World.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
            Main.npcFrameCount[npc.type] = 2;
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
            npc.damage = 10;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            // TODO these are placeholders change once balancing starts
            npc.damage = 15;
            npc.lifeMax = (int)(2100 * numPlayers);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void NPCLoot()
        {
            // TODO add loot (maybe none)
            Item.NewItem(npc.getRect(), ItemID.PlatinumCoin);
            Main.dayTime = false;
        }

        #region AI variables 
        private const int maxSpeed = 15;
        private int gameTicksCount = 0;
        private int solarRayCount = 0;
        private int summonCount = 0;
        private Phase bossPhase = Phase.chase;
        private bool startOfPhase = false;
        private bool minnionPhase = false;
        readonly Phase[] phasesList = new Phase[] { Phase.chase, Phase.solarFlare };
        readonly Phase[] minnionPhaseList = new Phase[] { Phase.commandSpin, Phase.commandAttack, Phase.commandHeal };
        private List<int> tinySunIds = new List<int>();

        private enum Phase
        {
            changePhase,
            chase,
            solarFlare,
            summonMinions,
            commandAttack,
            commandSpin,
            commandHeal,
            enrage
        }
        #endregion

        public override void AI()
        {
            this.npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            if (!player.active || player.dead)
            {
                foreach(int id in tinySunIds)
                {
                    Main.npc[id].life = 0;
                }
                DespawnBoss(this.npc);
            }
            else
            {
                if (npc.life < npc.lifeMax * 0.2)
                {
                    bossPhase = Phase.enrage;
                    EnrageAI(player);
                }
                else
                {
                    switch (bossPhase)
                    {
                        case Phase.changePhase:
                            startOfPhase = true;
                            if (minnionPhase)
                            {
                                if (AnyTinySunsRemaining())
                                {
                                    bossPhase = (Phase)minnionPhaseList.GetValue(Main.rand.Next(minnionPhaseList.Length));
                                }
                                else
                                {
                                    bossPhase = Phase.summonMinions;
                                }
                                minnionPhase = false;
                            }
                            else
                            {
                                bossPhase = (Phase)phasesList.GetValue(Main.rand.Next(phasesList.Length));
                                minnionPhase = true;
                            }
                            npc.netUpdate = true;
                            break;
                        case Phase.chase:
                            ChaseAI(player);
                            break;
                        case Phase.solarFlare:
                            SolarRayAI(player);
                            break;
                        case Phase.summonMinions:
                            SummonMinionsAI(player);
                            break;
                        case Phase.commandAttack:
                            CommandAttackAI();
                            break;
                        case Phase.commandHeal:
                            CommandHealAI();
                            break;
                        case Phase.commandSpin:
                            CommandSpinAI();
                            break;
                        default:
                            bossPhase = Phase.changePhase;
                            break;
                    }
                }

                gameTicksCount++;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if(bossPhase == Phase.enrage)
            {
                npc.frame.Y = frameHeight;
            }
            else
            {
                npc.frame.Y = 0;
            }
        }

        #region AI Phases
        private void ChaseAI(Player player)
        {
            if (gameTicksCount > 75)
            {
                this.npc.velocity = PathBossTowardsTarget(player.position, maxSpeed);
                gameTicksCount = 0;

                // 20% chance to change phase 
                if (Main.rand.Next(10) > 6)
                {
                    bossPhase = Phase.changePhase;
                }
                npc.netUpdate = true;
            }
        }

        private void SolarRayAI(Player player)
        {
            if(gameTicksCount > 80 || startOfPhase)
            {
                if (startOfPhase)
                {
                    this.npc.velocity = new Vector2(0, 0);
                    // Teleport boss to directly above player
                    this.npc.position = new Vector2((player.position.X - (npc.width / 2)), (player.position.Y - 400));
                    startOfPhase = false;
                }
                int solarFlareDmg = 25;
                Projectile.NewProjectile(npc.position.X + npc.width + 21, npc.position.Y, 0f, 0f, ProjectileType<SolarFlare>(), solarFlareDmg, 0f, Main.myPlayer, npc.whoAmI, 0);
                Projectile.NewProjectile(npc.position.X, npc.position.Y, 0f, 0f, ProjectileType<SolarFlare>(), solarFlareDmg, 0f, Main.myPlayer, npc.whoAmI, 0);
                int offset;
                // Generate projectiles to the right and left of the boss
                for (int i = 1; i < 8; i++)
                {
                    if(solarRayCount % 2 == 0)
                    {
                        offset = 100;
                    }
                    else
                    {
                        offset = 0;
                    }
                    Projectile.NewProjectile(npc.Center.X + offset - (200 * i), npc.Center.Y, 0f, 0f, ProjectileType<SolarFlare>(), solarFlareDmg, 0f, Main.myPlayer, npc.whoAmI, 0);
                    Projectile.NewProjectile(npc.Center.X + offset + (200 * i), npc.Center.Y, 0f, 0f, ProjectileType<SolarFlare>(), solarFlareDmg, 0f, Main.myPlayer, npc.whoAmI, 0);
                }
                npc.netUpdate = true;
                solarRayCount++;

                // Shoot solar rays 4 times then change phase
                if(solarRayCount > 4)
                {
                    solarRayCount = 0;
                    bossPhase = Phase.changePhase;
                }
                gameTicksCount = 0;
            }
            
        }
        
        private void SummonMinionsAI(Player player)
        {
            if (gameTicksCount > 60)
            {
                if(summonCount == 0)
                {
                    this.npc.velocity = new Vector2(0, 0);
                    // Teleport boss to directly above player
                    this.npc.position = new Vector2((player.position.X - (npc.width / 2)), (player.position.Y - 400));
                }
                if(summonCount > 3)
                {
                    summonCount = 0;
                    bossPhase = Phase.changePhase;
                    gameTicksCount = 0;
                    return;
                }

                if (AnyTinySunsRemaining() && summonCount == 0)
                {
                    foreach(int sunIndex in tinySunIds)
                    {
                        Main.npc[sunIndex].ai[3] = (int)TinySun.Phase.spin;
                    }
                    bossPhase = Phase.changePhase;
                    return;
                }
                int tinySunId = NPC.NewNPC((int)npc.position.X - 150, (int)npc.position.Y - (npc.height/2), mod.NPCType("TinySun"));

                tinySunIds.Add(tinySunId);
                
                // Send reference info to the newly summoned npc
                Main.npc[tinySunId].ai[0] = npc.whoAmI;
                Main.npc[tinySunId].ai[1] = summonCount;
                Main.npc[tinySunId].ai[3] = (int)TinySun.Phase.attack;

                summonCount++;
                gameTicksCount = 0;
            }
        }
        private void CommandAttackAI()
        {
            if(gameTicksCount > 40)
            {
                foreach(int summonId in tinySunIds)
                {
                    Main.npc[summonId].ai[3] = (int)TinySun.Phase.attack;
                }
            }
            bossPhase = Phase.changePhase;
            gameTicksCount = 0;
            return;
        }

        private void CommandHealAI()
        {
            if(gameTicksCount > 40)
            {
                foreach(int summonId in tinySunIds)
                {
                    Main.npc[summonId].ai[3] = (int)TinySun.Phase.heal;
                }
            }
            bossPhase = Phase.changePhase;
            gameTicksCount = 0;
            return;
        }

        private void CommandSpinAI()
        {
            if(gameTicksCount > 40)
            {
                foreach(int summonId in tinySunIds)
                {
                    Main.npc[summonId].ai[3] = (int)TinySun.Phase.spin;
                }
            }
            bossPhase = Phase.changePhase;
            gameTicksCount = 0;
            return;
        }
        // TODO potentially change this so it isn't just faster version of chase
        private void EnrageAI(Player player)
        {
            if (gameTicksCount > 30)
            {
                this.npc.velocity = PathBossTowardsTarget(player.position, maxSpeed * 2);
                gameTicksCount = 0;
            }
        }
        #endregion


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((short)gameTicksCount);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            gameTicksCount = reader.ReadInt16();
        }

        /*
         * Potentially move some of these to their own file/folder depending how many are needed for other bosses/npcs for reusability
         */
        #region UtilityFunctions

        private bool AnyTinySunsRemaining()
        {
            if (tinySunIds.Count > 0)
            {
                foreach (int id in tinySunIds)
                {
                    if (Main.npc[id].active)
                    {
                        return true;
                    }
                }
            }
            tinySunIds.Clear();
            return tinySunIds.Count > 0;
        }

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