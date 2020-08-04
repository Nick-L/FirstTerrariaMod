using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace TestMod.Players
{
    public class TerrariaPlayer : ModPlayer
    {
        // Buff references
        public bool buff_MoneyForLife = true;
		public override void ResetEffects()
		{
			buff_MoneyForLife = false;

			base.ResetEffects();
		}

        public override void OnEnterWorld(Player player)
        {
            base.OnEnterWorld(player);
        }

        // In MP, other clients need accurate information about your player or else bugs happen.
        // clientClone, SyncPlayer, and SendClientChanges, ensure that information is correct.
        // We only need to do this for data that is changed by code not executed by all clients, 
        // or data that needs to be shared while joining a world.
        // For example, examplePet doesn't need to be synced because all clients know that the player is wearing the ExamplePet item in an equipment slot. 
        // The examplePet bool is set for that player on every clients computer independently (via the Buff.Update), keeping that data in sync.
        // ExampleLifeFruits, however might be out of sync. For example, when joining a server, we need to share the exampleLifeFruits variable with all other clients.
        // In addition, in ExampleUI we have a button that toggles "Non-Stop Party". We need to sync this whenever it changes.
        public override void clientClone(ModPlayer clientClone)
		{
			base.clientClone(clientClone);
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			base.SyncPlayer(toWho, fromWho, newPlayer);
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			base.SendClientChanges(clientPlayer);
		}

		public override void UpdateDead()
		{
			base.UpdateDead();
		}

		public override void LoadLegacy(BinaryReader reader)
		{
			base.LoadLegacy(reader);
		}

		public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
        {
			base.SetupStartInventory(items, mediumcoreDeath);
        }

		public override void UpdateBiomes()
		{
			base.UpdateBiomes();
		}

		public override bool CustomBiomesMatch(Player other)
		{
			return base.CustomBiomesMatch(other);
		}

		public override void CopyCustomBiomesTo(Player other)
		{
			base.CopyCustomBiomesTo(other);
		}

		public override void SendCustomBiomes(BinaryWriter writer)
		{
			base.SendCustomBiomes(writer);
		}

		public override void ReceiveCustomBiomes(BinaryReader reader)
		{
			base.ReceiveCustomBiomes(reader);
		}

		public override void UpdateBiomeVisuals()
		{
			base.UpdateBiomeVisuals();
		}

		public override Texture2D GetMapBackgroundImage()
		{
			base.GetMapBackgroundImage();
			return null;
		}

		public override void UpdateBadLifeRegen()
		{
			base.UpdateBadLifeRegen();
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			base.ProcessTriggers(triggersSet);
		}

		public override void PreUpdateBuffs()
		{
			base.PreUpdateBuffs();
		}

		public override void PostUpdateBuffs()
		{
			base.PostUpdateBuffs();
		}

		public override void UpdateVanityAccessories()
		{
			base.UpdateVanityAccessories();
		}

		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			base.UpdateEquips(ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
		}

		public override void PostUpdateEquips()
		{
			base.PostUpdateEquips();
		}

		public override void PostUpdateMiscEffects()
		{
			base.PostUpdateMiscEffects();
		}

		public override void FrameEffects()
		{
			base.FrameEffects();
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (buff_MoneyForLife)
			{
				ReduceDamageWithCoins(ref damage);
				if (damage == 0)
				{
					playSound = false;
					crit = false;
				}
			}
			return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
		}

		public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			base.Hurt(pvp, quiet, damage, hitDirection, crit);
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
		}

		public override float UseTimeMultiplier(Item item)
		{
			return base.UseTimeMultiplier(item);
		}

		public override void OnConsumeMana(Item item, int manaConsumed)
		{
			base.OnConsumeMana(item, manaConsumed);
		}

		public override void AnglerQuestReward(float quality, List<Item> rewardItems)
		{
			base.AnglerQuestReward(quality, rewardItems);
		}

		public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
		{
			base.CatchFish(fishingRod, bait, power, liquidType, poolSize, worldLayer, questFish, ref caughtType, ref junk);
		}

		public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
		{
			base.GetFishingLevel(fishingRod, bait, ref fishingLevel);
		}

		public override void GetDyeTraderReward(List<int> dyeItemIDsPool)
		{
			base.GetDyeTraderReward(dyeItemIDsPool);
		}

		public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
		{
			base.ModifyDrawInfo(ref drawInfo);
		}

		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
		}

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			base.ModifyDrawLayers(layers);
		}

		public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
		{
			return base.ModifyNurseHeal(nurse, ref health, ref removeDebuffs, ref chatText);
		}

		public override void PostBuyItem(NPC vendor, Item[] shop, Item item)
		{
			base.PostBuyItem(vendor, shop, item);
		}

		public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
		{
			base.PostSellItem(vendor, shopInventory, item);
		}

		private void ReduceDamageWithCoins(ref int damage)
        {
			int count = 0;

            while (player.ConsumeItem(ItemID.SilverCoin))
            {
                if (damage > 0 || damage > player.statLife)
                {
					if (count > 3)
					{
						count = 0;
						damage -= 1;
					}
                    else
                    {
						count++;
                    }
                }
                else
                {
					damage = 0;
					return;
                }
            }
            while (player.ConsumeItem(ItemID.GoldCoin))
            {
				if (damage > 5 || damage > player.statLife)
				{
					damage -= 1;
				}
				else
				{
					return;
				}
			}
            while (player.ConsumeItem(ItemID.PlatinumCoin))
            {
				if (damage > 20 || damage > player.statLife)
				{
					damage = 0;
				}
            }
        }
	}
}