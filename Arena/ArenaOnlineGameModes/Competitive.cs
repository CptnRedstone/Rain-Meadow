﻿using Menu;
using RainMeadow;
using RainMeadow.Arena.ArenaOnlineGameModes.TeamBattle;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace RainMeadow
{
    public class FFA : ExternalArenaGameMode
    {

        public static ArenaSetup.GameTypeID FFAMode = new ArenaSetup.GameTypeID("Free For All", register: false);

        private int _timerDuration;
        //public override ArenaSetup.GameTypeID GetGameModeId
        //{
        //    get
        //    {
        //        return FFAMode;
        //    }
        //}
        public static bool isFFA(ArenaOnlineGameMode arena, out FFA ffa)
        {

            ffa = null;
            if (arena.currentGameMode == FFAMode.value)
            {
                ffa = (arena.registeredGameModes.FirstOrDefault(x => x.Key == FFAMode.value).Value as FFA);
                return true;
            }
            return false;

        }

        public override bool IsExitsOpen(ArenaOnlineGameMode arena, On.ArenaBehaviors.ExitManager.orig_ExitsOpen orig, ArenaBehaviors.ExitManager self)
        {
            int playersStillStanding = self.gameSession.Players?.Count(player =>
                player.realizedCreature != null &&
                (player.realizedCreature.State.alive)) ?? 0;

            if (playersStillStanding == 1 && arena.arenaSittingOnlineOrder.Count > 1)
            {
                return true;
            }

            if (self.world.rainCycle.TimeUntilRain <= 100)
            {
                return true;
            }

            return orig(self);
        }

        public override bool SpawnBatflies(FliesWorldAI self, int spawnRoom)
        {
            return false;
        }
        public override string TimerText()
        {
            var client_settings = OnlineManager.lobby.clientSettings[OnlineManager.mePlayer].GetData<ArenaClientSettings>();

            SlugcatStats.Name playingAs;
            if (client_settings.playingAs != RainMeadow.Ext_SlugcatStatsName.OnlineRandomSlugcat)
            {
                playingAs = client_settings.playingAs;
            }
            else
            {
                playingAs = client_settings.randomPlayingAs ?? SlugcatStats.Name.White;
            }

            if (ModManager.MSC && playingAs == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel)
            {
                return Utils.Translate($"Prepare for combat,") + " " + Utils.Translate((OnlineManager.lobby.gameMode as ArenaOnlineGameMode)?.paincatName ?? "");
            }

            return Utils.Translate("Prepare for combat,") + " " + Utils.Translate(SlugcatStats.getSlugcatName(playingAs));
        }
        public override int SetTimer(ArenaOnlineGameMode arena)
        {
            return arena.setupTime = RainMeadow.rainMeadowOptions.ArenaCountDownTimer.Value;
        }
        public override int TimerDuration
        {
            get { return _timerDuration; }
            set { _timerDuration = value; }
        }
        public override int TimerDirection(ArenaOnlineGameMode arena, int timer)
        {
            return --arena.setupTime;
        }
        public override bool HoldFireWhileTimerIsActive(ArenaOnlineGameMode arena)
        {
            if (arena.setupTime > 0)
            {
                return arena.countdownInitiatedHoldFire = true;
            }
            else
            {
                return arena.countdownInitiatedHoldFire = false;
            }
        }

        public override void LandSpear(ArenaOnlineGameMode arena, ArenaGameSession self, Player player, Creature target, ArenaSitting.ArenaPlayer aPlayer)
        {
            aPlayer.AddSandboxScore(self.GameTypeSetup.spearHitScore);

        }

        public override string AddIcon(ArenaOnlineGameMode arena, PlayerSpecificOnlineHud owner, SlugcatCustomization customization, OnlinePlayer player)
        {

            if (owner.clientSettings.owner == OnlineManager.lobby.owner)
            {
                return "ChieftainA";
            }
            return base.AddIcon(arena, owner, customization, player);

        }

        public override Color IconColor(ArenaOnlineGameMode arena, PlayerSpecificOnlineHud owner, SlugcatCustomization customization, OnlinePlayer player)
        {
            if (owner.PlayerConsideredDead)
            {
                return Color.grey;
            }
            if (arena.reigningChamps != null && arena.reigningChamps.list != null && arena.reigningChamps.list.Contains(player.id))
            {
                return Color.yellow;
            }

            return base.IconColor(arena, owner, customization, player);
        }

    }
}
