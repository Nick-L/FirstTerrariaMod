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

/*
 *  TODO - Implement this NPC entity
 *  
 *  This is the skeleton for a new NPC
 */

namespace TestMod.NPCs.BlazingBeast
{
    public class TinySun : ModNPC
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {

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

    }

    public override void AI()
    {

    }
    public override void FindFrame(int frameHeight)
    {

    }
    public override void SendExtraAI(BinaryWriter writer)
    {

    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }
}
}