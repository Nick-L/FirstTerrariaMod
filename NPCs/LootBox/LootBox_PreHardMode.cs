using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TestMod.NPCs.LootBox
{
    class LootBox_PreHardMode : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pre Hardmode Loot Box");
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
            npc.lifeMax = 1;
            npc.defense = 0;
            // TODO Change damage once ai etc is done
            npc.damage = 0;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0f;
        }

        public override void NPCLoot()
        {
            Item.NewItem(npc.getRect(), ItemID.MeteorHelmet);
            Item.NewItem(npc.getRect(), ItemID.MeteorSuit);
            Item.NewItem(npc.getRect(), ItemID.MeteorLeggings);
            Item.NewItem(npc.getRect(), ItemID.CloudinaBottle);
            Item.NewItem(npc.getRect(), ItemID.SpaceGun);
        }

        private int gameTicksCount = 0;

        public override void AI()
        {
            this.npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            if(!player.active || player.dead)
            {
                DespawnBoss(this.npc);
            }
            else
            {
                if(gameTicksCount > 120)
                {
                    this.npc.position = new Vector2((player.position.X - (npc.width / 2)), (player.position.Y - 150));
                    gameTicksCount = 0;
                }
                gameTicksCount++;
            }
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
    }
}
