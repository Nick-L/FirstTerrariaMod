using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.ModBrowser;
using TestMod.Players;


/*
 * Reference for 
 * https://docs.tmodloader.net/html/class_terraria_1_1_mod_loader_1_1_mod_buff.html
 */
namespace TestMod.Buffs
{
    public class Buff_MoneyForLife : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Money for Life");
            Description.SetDefault("Sacrifice your money to avoid damage");
            canBeCleared = true;

            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<TerrariaPlayer>().buff_MoneyForLife = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            base.Update(npc, ref buffIndex);
        }
    }
}
