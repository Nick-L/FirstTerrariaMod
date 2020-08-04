using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace TestMod.Items.Weapons
{
    public class KillStick : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Kill Stick");
			Tooltip.SetDefault("Mod testing tool");
		}

		public override void SetDefaults()
		{
			item.damage = 999999999;
			item.melee = true;
			item.width = 20;
			item.height = 600;
			item.useTime = 10;
			item.useAnimation = 10;
			item.knockBack = 6;

			item.rare = -1;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.crit = 100;

			item.useStyle = ItemUseStyleID.SwingThrow;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CopperCoin, 5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
