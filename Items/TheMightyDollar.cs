using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using static Terraria.ModLoader.ModContent;
using TestMod.Players;

/*
 * Reference for functions included in the ModItem class
 * https://docs.tmodloader.net/html/class_terraria_1_1_mod_loader_1_1_mod_item.html
 */
namespace TestMod.Items
{
    public class TheMightyDollar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Mighty Dollar");
            Tooltip.SetDefault("The mighty dollar can protect a person from harm");
        }

        public override void SetDefaults()
        {
            item.width = 25;
            item.height = 35;
            item.consumable = true;
            item.maxStack = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;

            item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldCoin, 10);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool CanUseItem(Player player)
        {
            if (!player.HasBuff(BuffType<Buffs.Buff_MoneyForLife>()))
            {
                player.AddBuff(BuffType<Buffs.Buff_MoneyForLife>(), 600);
                Main.PlaySound(SoundID.CoinPickup, player.position, 0);
                item.stack--;
                return true;
            }
            return false;
        }
    }
}
