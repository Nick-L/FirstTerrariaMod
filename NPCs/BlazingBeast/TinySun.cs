using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


/*
 * Reference for public members of ModNPC class
 * https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_mod_n_p_c.html
 */

namespace TestMod.NPCs.BlazingBeast
{
    public class TinySun : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tiny Sun");
        }

        public override void SetDefaults()
        {
            // Set width/height to match image
            npc.width = 50;
            npc.height = 50;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 1;
            // -1 means custom ai
            npc.aiStyle = -1;
            // TODO change health/defense once balancing starts
            npc.lifeMax = 500;
            npc.defense = 8;
            // TODO Change damage once ai etc is done
            npc.damage = 1;
            npc.boss = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            // TODO these are placeholders change once balancing starts
            npc.damage = 1;
            npc.lifeMax = (int)(500 + numPlayers * 50);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void NPCLoot()
        {

        }

        #region AI variables
        private const int maxSpeed = 5;
        private int gameTicksCount = 0;
        private bool startSpin = true;
        private Phase bossPhase = Phase.spin;
        readonly Phase[] phasesList = new Phase[] { Phase.spin };

        private enum Phase
        {
            changePhase,
            spin,
            attack,
            heal
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
                switch (bossPhase)
                {
                    case Phase.changePhase:
                        this.npc.velocity = new Vector2(0, 0);
                        bossPhase = (Phase)phasesList.GetValue(Main.rand.Next(phasesList.Length));
                        break;
                    case Phase.attack:
                        AttackAI(player);
                        break;
                    case Phase.spin:
                        SpinAI();
                        break;
                    case Phase.heal:
                        break;
                    default:
                        bossPhase = Phase.changePhase;
                        break;
                }
                gameTicksCount++;
            }
        }

        private void AttackAI(Player player)
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

        private void SpinAI()
        {
            Vector2 masterCenter = Main.npc[(int)npc.ai[0]].Center;
            if (startSpin)
            {
                npc.velocity = new Vector2(0,0);
                switch (npc.ai[1])
                {
                    case 0: // Left
                        npc.ai[2] = 0;
                        break;
                    case 1: // Top
                        npc.ai[2] = 90;
                        break;
                    case 2: // Right
                        npc.ai[2] = 180;
                        break;
                    case 3: // Bottom
                        npc.ai[2] = 270;
                        break;
                    default:
                        throw new Exception($"TinySun - outside of expected case npc.ai[1] was {npc.ai[1]} expected 0 - 3");
                }
                GetPositionFromAngle(100, npc.ai[2], masterCenter);
                startSpin = false;
            }
            else
            {
                if(npc.ai[3] != 1)
                {
                    npc.position = GetPositionFromAngle(100, npc.ai[2], masterCenter);
                    npc.ai[2] += 2;
                    if(npc.ai[2] > 360)
                    {
                        npc.ai[2] -= 360;
                    }
                }
                else
                {
                    npc.ai[3] = 0;
                    startSpin = true;
                    bossPhase = Phase.changePhase;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {

        }

        #region Utility functions
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

        private Vector2 GetPositionFromAngle(int radius, float angleDegrees, Vector2 circleCenter)
        {
            float yOffset = (float)(radius * Math.Sin(ConvertDegreesToRadians(angleDegrees)));
            float xOffset = (float)(Math.Sqrt((radius * radius) - (yOffset * yOffset)));

            if(angleDegrees < 90 || angleDegrees > 270)
            {
                xOffset *= -1;
            }

            return new Vector2(circleCenter.X + (xOffset - (this.npc.width/2)), circleCenter.Y + (yOffset - (this.npc.height/2)));
        }

        private double ConvertDegreesToRadians(float angleDegrees)
        {
            return (Math.PI / 180) * angleDegrees; 
        }
        #endregion
    }
}