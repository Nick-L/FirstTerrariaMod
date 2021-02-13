using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TestMod.Items.LootBox_Summons
{
    public class LootBox_PreHardMode : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LootBox PreHardMode Summon");
            Tooltip.SetDefault("Summons LootBox_PreHardMode");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.consumable = true;
            item.maxStack = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock, 1);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool CanUseItem(Player player)
        {
            if (!NPC.AnyNPCs(mod.NPCType("LootBox_PreHardMode")))
            {
                NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("LootBox_PreHardMode"));
                item.stack--;
                return true;
            }
            return false;
        }
    }
}
