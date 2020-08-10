using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TestMod.Players;

namespace TestMod.Utilities
{
    public static class AnimationHelper
    {
        public static readonly PlayerLayer blindingEffect = new PlayerLayer("TestMod", "BlindingEffect", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.drawPlayer.whoAmI != Main.myPlayer) 
                return;

            Main.playerDrawData.Add(BlindingDrawData(drawInfo, "UI/BlindedUI", drawInfo.drawPlayer.Center));
        });

        public static DrawData BlindingDrawData(PlayerDrawInfo drawInfo, string sprite, Vector2 playerCenter)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = TestMod.Instance;
            Texture2D texture = mod.GetTexture(sprite);
            return new DrawData(texture, new Vector2(0,0), new Rectangle(0,0, Main.screenWidth, Main.screenHeight), Color.White);
        }
    }
}
