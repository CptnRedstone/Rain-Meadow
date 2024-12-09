﻿
using UnityEngine;

namespace RainMeadow
{
    public class ArenaPrepTimer : HUD.HudPart
    {
        public enum TimerMode
        {
            Countdown,
            Waiting
        }

        private float Readtimer;
        private bool isRunning;
        private TimerMode currentMode = TimerMode.Waiting;  // Track which timer is active
        private TimerMode showMode = TimerMode.Waiting;  // Track which timer is being displayed
        private TimerMode matchMode = TimerMode.Waiting; // mode at start of match
        private FLabel timerLabel;
        private FLabel modeLabel;
        private Vector2 pos, lastPos;
        private float fade, lastFade;
        public ArenaOnlineGameMode arena;
        public ArenaGameSession session;
        public bool cancelTimer;
        private Player? player;
        private bool countdownInitiated;
        private int safetyCatchTimer;
        public ArenaPrepTimer(HUD.HUD hud, FContainer fContainer, ArenaOnlineGameMode arena, ArenaGameSession arenaGameSession) : base(hud)
        {

            session = arenaGameSession;
            arena.trackSetupTime = arena.onlineArenaGameMode.SetTimer(arena);
            matchMode = TimerMode.Waiting;

            timerLabel = new FLabel("font", FormatTime(0))
            {
                scale = 2.4f,
                alignment = FLabelAlignment.Left
            };

            modeLabel = new FLabel("font", currentMode.ToString())
            {
                scale = 1.6f,
                alignment = FLabelAlignment.Left
            };

            pos = new Vector2(80f, hud.rainWorld.options.ScreenSize.y - 60f);
            lastPos = pos;
            timerLabel.SetPosition(DrawPos(1f));
            modeLabel.SetPosition(DrawPos(1f) + new Vector2(135f, 0f));

            fContainer.AddChild(timerLabel);
            fContainer.AddChild(modeLabel);
            this.arena = arena;
            countdownInitiated = false;
            arena.arenaPrepTimer = this;
            safetyCatchTimer = 0;
        }

        public Vector2 DrawPos(float timeStacker)
        {
            return Vector2.Lerp(lastPos, pos, timeStacker);
        }

        public override void Draw(float timeStacker)
        {
            base.Draw(timeStacker);
            if (RainMeadow.isArenaMode(out var arena))
            {
                if (showMode == TimerMode.Waiting)
                {
                    arena.onlineArenaGameMode.TimerDirection(arena, safetyCatchTimer);
                }

                if (arena.playerEnteredGame < arena.arenaSittingOnlineOrder.Count)
                {
                    showMode = TimerMode.Waiting;
                    matchMode = TimerMode.Waiting;
                    modeLabel.text = showMode.ToString();
                    arena.countdownInitiatedHoldFire = true;


                }
                else if (arena.setupTime > 0)
                {
                    arena.setupTime = arena.onlineArenaGameMode.TimerDirection(arena, arena.setupTime);
                    showMode = TimerMode.Countdown;
                    matchMode = TimerMode.Countdown;
                    modeLabel.text = arena.onlineArenaGameMode.TimerText();
                    arena.countdownInitiatedHoldFire = arena.onlineArenaGameMode.HoldFireWhileTimerIsActive(arena);
                }

                else if (arena.setupTime <= 0 && !countdownInitiated)
                {
                    countdownInitiated = true;
                    arena.countdownInitiatedHoldFire = false;

                    hud.PlaySound(SoundID.MENU_Start_New_Game);
                    ClearSprites();
                }

                if ((safetyCatchTimer > arena.trackSetupTime + 60 && arena.setupTime != 0)) // Something went wrong with the timer. Clear it.
                {
                    ClearSprites();
                    arena.countdownInitiatedHoldFire = false;

                };



            }

            timerLabel.text = FormatTime(arena.setupTime);
        }

        // Format time to MM:SS:MMM
        public static string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time % 1) * 1000);

            return $"{minutes:D2}:{seconds:D2}:{milliseconds:D3}";
        }

        public override void ClearSprites()
        {
            base.ClearSprites();
            timerLabel.RemoveFromContainer();
            modeLabel.RemoveFromContainer();

            arena.arenaPrepTimer = null;
        }
    }
}