using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TestMod.Players;

namespace TestMod.Buffs
{
    public class Debuff_Blinded : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Blinded");
            Description.SetDefault("Didn't yo momma tell you not to look at the sun");
            canBeCleared = true;

            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<TerrariaPlayer>().debuff_Blinded = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
        }
    }
}
