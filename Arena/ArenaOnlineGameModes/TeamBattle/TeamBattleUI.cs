﻿using Menu;
using Menu.Remix;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ArenaMode = RainMeadow.ArenaOnlineGameMode;
using Menu.Remix.MixedUI;
using RainMeadow.UI.Components;

namespace RainMeadow.Arena.ArenaOnlineGameModes.TeamBattle
{
    public partial class TeamBattleMode : ExternalArenaGameMode
    {
        OpTinyColorPicker martyrColor;
        OpTinyColorPicker chieftainColor;
        OpTinyColorPicker dragonSlayerColor;
        OpTinyColorPicker outlawColor;

        public int winningTeam;
        public int martyrsSpawn;
        public int outlawsSpawn;
        public int dragonslayersSpawn;
        public int chieftainsSpawn;
        public int roundSpawnPointCycler;
        public ProperlyAlignedMenuLabel teamColorLerpLabel;
        public OpTextBox teamColorBox;

        public string martyrsTeamName = RainMeadow.rainMeadowOptions.MartyrTeamName.Value;
        public string outlawTeamNames = RainMeadow.rainMeadowOptions.OutlawsTeamName.Value;
        public string dragonSlayersTeamNames = RainMeadow.rainMeadowOptions.DragonSlayersTeamName.Value;
        public string chieftainsTeamNames = RainMeadow.rainMeadowOptions.ChieftainTeamName.Value;

        public UIelementWrapper externalModeWrapper;

        public OpComboBox? arenaTeamComboBox;
        public OpTextBox? martyrsTeamNameUpdate;
        public OpTextBox? outlawsTeamNameUpdate;
        public OpTextBox? dragonsSlayersTeamNameUpdate;
        public OpTextBox? chieftainsTeamNameUpdate;

        public bool teamComboBoxLastHeld;
        public Dictionary<int, string> teamNameDictionary = new Dictionary<int, string>
        {
            { 0, RainMeadow.rainMeadowOptions.MartyrTeamName.Value },
            { 1, RainMeadow.rainMeadowOptions.OutlawsTeamName.Value },
            { 2, RainMeadow.rainMeadowOptions.DragonSlayersTeamName.Value },
            { 3, RainMeadow.rainMeadowOptions.ChieftainTeamName.Value }
        };
        public enum TeamMappings
        {
            martyrsTeamName,
            outlawTeamName,
            dragonslayersTeamName,
            chieftainsTeamName
        }

        public Dictionary<int, string> TeamMappingsDictionary = new Dictionary<int, string>
        {
            { 0, "SaintA" },
            { 1, "OutlawA" },
            { 2, "DragonSlayerA" },
            { 3, "ChieftainA" }
    };

        public static Dictionary<int, Color> TeamColors = new Dictionary<int, Color>
        {
    { 0, GetColorFromHex("#FF7F7F") },
    { 1, GetColorFromHex("#FFFF7F") },
    { 2, GetColorFromHex("#7FFF7F") },
    { 3,  GetColorFromHex("#7F7FFF") }
    };


        public override void ArenaExternalGameModeSettingsInterface_ctor(ArenaOnlineGameMode arena, OnlineArenaExternalGameModeSettingsInterface extComp, Menu.Menu menu, MenuObject owner, MenuTabWrapper tabWrapper, Vector2 pos, float settingsWidth = 300)
        {
            if (isTeamBattleMode(arena, out var tb))
            {
                tb.winningTeam = -1;
                martyrsSpawn = 0;
                outlawsSpawn = 0;
                dragonslayersSpawn = 0;
                chieftainsSpawn = 0;
                roundSpawnPointCycler = 0;


                ListItem martyrListItem = new ListItem(RainMeadow.rainMeadowOptions.MartyrTeamName.Value);
                ListItem outlawsListItem = new ListItem(RainMeadow.rainMeadowOptions.OutlawsTeamName.Value);
                ListItem dragonSlayersListItem = new ListItem(RainMeadow.rainMeadowOptions.DragonSlayersTeamName.Value);
                ListItem chieftainsListItem = new ListItem(RainMeadow.rainMeadowOptions.ChieftainTeamName.Value);


                List<ListItem> teamNameListItems = new List<ListItem>();
                teamNameListItems.Add(martyrListItem);
                teamNameListItems.Add(outlawsListItem);
                teamNameListItems.Add(dragonSlayersListItem);
                teamNameListItems.Add(chieftainsListItem);

                var arenaGameModeLabel = new ProperlyAlignedMenuLabel(menu, owner, menu.Translate("Team:"), new Vector2(50, 380f), new Vector2(0, 20), false);
                bool defaultTeamNameString = OnlineManager.lobby.clientSettings[OnlineManager.mePlayer].TryGetData<ArenaTeamClientSettings>(out var t);
                arenaTeamComboBox = new OpComboBox2(new Configurable<string>(defaultTeamNameString ? teamNameDictionary[t.team] : ""), new Vector2(arenaGameModeLabel.pos.x + 50, arenaGameModeLabel.pos.y), 175f, teamNameListItems);
                arenaTeamComboBox.OnValueChanged += (config, value, lastValue) =>
                {
                    if (OnlineManager.lobby.clientSettings[OnlineManager.mePlayer].TryGetData<ArenaTeamClientSettings>(out var tb))
                    {
                        var alListItems = arenaTeamComboBox.GetItemList();
                        for (int i = 0; i < alListItems.Length; i++)
                        {
                            if (alListItems[i].name == value)
                            {
                                tb.team = i;
                            }
                        }

                    }
                };

                var martyrTeamLabel = new ProperlyAlignedMenuLabel(menu, owner, menu.Translate("Team 1:"), new Vector2(arenaGameModeLabel.pos.x, arenaTeamComboBox.pos.y - 45), new Vector2(0, 20), false);

                martyrsTeamNameUpdate = new(new Configurable<string>(RainMeadow.rainMeadowOptions.MartyrTeamName.Value), new(martyrTeamLabel.pos.x + 50, martyrTeamLabel.pos.y), 150);

                martyrsTeamNameUpdate.greyedOut = !OnlineManager.lobby.isOwner;
                martyrsTeamNameUpdate.allowSpace = true;
                martyrsTeamNameUpdate.OnValueUpdate += (config, value, lastValue) =>
                {
                    var alListItems = arenaTeamComboBox.GetItemList();
                    RainMeadow.rainMeadowOptions.MartyrTeamName.Value = value;
                    for (int i = 0; i < alListItems.Length; i++)
                    {
                        if (alListItems[i].displayName == lastValue)
                        {
                            alListItems[i].name = value;
                            alListItems[i].desc = value;
                            alListItems[i].displayName = value;
                        }


                    }
                    tb.martyrsTeamName = value;
                };

                var outlawTeamlabel = new ProperlyAlignedMenuLabel(menu, owner, menu.Translate("Team 2:"), new Vector2(arenaGameModeLabel.pos.x, martyrsTeamNameUpdate.pos.y - 45), new Vector2(0, 20), false);

                outlawsTeamNameUpdate = new(new Configurable<string>(RainMeadow.rainMeadowOptions.OutlawsTeamName.Value), new(outlawTeamlabel.pos.x + 50, outlawTeamlabel.pos.y), 150);

                outlawsTeamNameUpdate.greyedOut = !OnlineManager.lobby.isOwner;
                outlawsTeamNameUpdate.allowSpace = true;
                outlawsTeamNameUpdate.OnValueUpdate += (config, value, lastValue) =>
                {
                    var alListItems = arenaTeamComboBox.GetItemList();
                    RainMeadow.rainMeadowOptions.OutlawsTeamName.Value = value;
                    for (int i = 0; i < alListItems.Length; i++)
                    {
                        if (alListItems[i].displayName == lastValue)
                        {
                            alListItems[i].name = value;
                            alListItems[i].desc = value;
                            alListItems[i].displayName = value;
                        }


                    }
                    tb.outlawTeamNames = value;
                };
                ///
                var dragonSlayersLabel = new ProperlyAlignedMenuLabel(menu, owner, menu.Translate("Team 3:"), new Vector2(arenaGameModeLabel.pos.x, outlawsTeamNameUpdate.pos.y - 45), new Vector2(0, 20), false);

                dragonsSlayersTeamNameUpdate = new(new Configurable<string>(RainMeadow.rainMeadowOptions.DragonSlayersTeamName.Value), new(dragonSlayersLabel.pos.x + 50, dragonSlayersLabel.pos.y), 150);


                dragonsSlayersTeamNameUpdate.greyedOut = !OnlineManager.lobby.isOwner;
                dragonsSlayersTeamNameUpdate.allowSpace = true;
                dragonsSlayersTeamNameUpdate.OnValueUpdate += (config, value, lastValue) =>
                {
                    var alListItems = arenaTeamComboBox.GetItemList();
                    RainMeadow.rainMeadowOptions.DragonSlayersTeamName.Value = value;
                    for (int i = 0; i < alListItems.Length; i++)
                    {
                        if (alListItems[i].displayName == lastValue)
                        {
                            alListItems[i].name = value;
                            alListItems[i].desc = value;
                            alListItems[i].displayName = value;
                        }


                    }
                    tb.dragonSlayersTeamNames = value;
                };


                ///
                var chifetainTeamLabel = new ProperlyAlignedMenuLabel(menu, owner, menu.Translate("Team 4:"), new Vector2(arenaGameModeLabel.pos.x, dragonsSlayersTeamNameUpdate.pos.y - 45), new Vector2(0, 20), false);

                chieftainsTeamNameUpdate = new(new Configurable<string>(RainMeadow.rainMeadowOptions.ChieftainTeamName.Value), new(chifetainTeamLabel.pos.x + 50, chifetainTeamLabel.pos.y), 150);


                chieftainsTeamNameUpdate.greyedOut = !OnlineManager.lobby.isOwner;
                chieftainsTeamNameUpdate.allowSpace = true;
                chieftainsTeamNameUpdate.OnValueUpdate += (config, value, lastValue) =>
                {
                    var alListItems = arenaTeamComboBox.GetItemList();
                    RainMeadow.rainMeadowOptions.ChieftainTeamName.Value = value;
                    for (int i = 0; i < alListItems.Length; i++)
                    {
                        if (alListItems[i].displayName == lastValue)
                        {
                            alListItems[i].name = value;
                            alListItems[i].desc = value;
                            alListItems[i].displayName = value;
                        }


                    }
                    tb.chieftainsTeamNames = value;
                };

                externalModeWrapper = new UIelementWrapper(tabWrapper, arenaTeamComboBox);

                martyrColor = new OpTinyColorPicker(menu, externalModeWrapper.tabWrapper, new Vector2(martyrsTeamNameUpdate.pos.x + martyrsTeamNameUpdate.rect.size.x + 50, martyrsTeamNameUpdate.pos.y), TeamColors[0]);
                martyrColor.colorPicker.greyedOut = !OnlineManager.lobby.isOwner;

                UIelementWrapper martyrColorsWrapper = new UIelementWrapper(tabWrapper, martyrColor);


                chieftainColor = new OpTinyColorPicker(menu, externalModeWrapper.tabWrapper, new Vector2(chieftainsTeamNameUpdate.pos.x + chieftainsTeamNameUpdate.rect.size.x + 50, chieftainsTeamNameUpdate.pos.y), TeamColors[1]);
                chieftainColor.colorPicker.greyedOut = !OnlineManager.lobby.isOwner;
                UIelementWrapper chieftainColorWrapper = new UIelementWrapper(tabWrapper, chieftainColor);

                dragonSlayerColor = new OpTinyColorPicker(menu, externalModeWrapper.tabWrapper, new Vector2(dragonsSlayersTeamNameUpdate.pos.x + dragonsSlayersTeamNameUpdate.rect.size.x + 50, dragonsSlayersTeamNameUpdate.pos.y), TeamColors[2]);
                dragonSlayerColor.colorPicker.greyedOut = !OnlineManager.lobby.isOwner;
                UIelementWrapper dragonSlayerColorsWrapper = new UIelementWrapper(tabWrapper, dragonSlayerColor);


                outlawColor = new OpTinyColorPicker(menu, externalModeWrapper.tabWrapper, new Vector2(outlawsTeamNameUpdate.pos.x + outlawsTeamNameUpdate.rect.size.x + 50, outlawsTeamNameUpdate.pos.y), TeamColors[3]);
                outlawColor.colorPicker.greyedOut = !OnlineManager.lobby.isOwner;
                UIelementWrapper outlawColorWrapper = new UIelementWrapper(tabWrapper, outlawColor);

                teamColorLerpLabel = new(menu, tabWrapper, menu.Translate("Team Color Lerp Factor:"), new Vector2(chifetainTeamLabel.pos.x, chifetainTeamLabel.pos.y - 38), new Vector2(0, 20), false);
                teamColorBox = new(new Configurable<float>(RainMeadow.rainMeadowOptions.TeamColorLerp.Value), new(teamColorLerpLabel.pos.x, teamColorLerpLabel.pos.y - 38), 50)
                {
                    alignment = FLabelAlignment.Center,
                    description = menu.Translate("How strongly the team color mixes with your color"),
                    accept = OpTextBox.Accept.Float
                };
                teamColorBox.OnValueUpdate += (config, value, lastValue) =>
                {
                    RainMeadow.rainMeadowOptions.TeamColorLerp.Value = teamColorBox.valueFloat;
                };
                UIelementWrapper martyrWrapper = new UIelementWrapper(tabWrapper, martyrsTeamNameUpdate);
                UIelementWrapper outlawWrapper = new UIelementWrapper(tabWrapper, outlawsTeamNameUpdate);
                UIelementWrapper dragonSlayerWrapper = new UIelementWrapper(tabWrapper, dragonsSlayersTeamNameUpdate);
                UIelementWrapper chiefTainWrapper = new UIelementWrapper(tabWrapper, chieftainsTeamNameUpdate);
                UIelementWrapper teamColorBoxWrapper = new UIelementWrapper(tabWrapper, teamColorBox);

                martyrColor.OnValueChangedEvent += ColorSelector_OnValueChangedEvent;
                outlawColor.OnValueChangedEvent += ColorSelector_OnValueChangedEvent;
                dragonSlayerColor.OnValueChangedEvent += ColorSelector_OnValueChangedEvent;
                chieftainColor.OnValueChangedEvent += ColorSelector_OnValueChangedEvent;


                extComp.SafeAddSubobjects(tabWrapper, externalModeWrapper, arenaGameModeLabel, martyrWrapper, martyrTeamLabel, outlawWrapper, outlawTeamlabel, dragonSlayerWrapper, dragonSlayersLabel, chiefTainWrapper, chifetainTeamLabel, teamColorBoxWrapper, teamColorLerpLabel);
            }
        }

        public override void ArenaExternalGameModeSettingsInterface_Update(ArenaMode arena, OnlineArenaExternalGameModeSettingsInterface extComp, Menu.Menu menu, Menu.MenuObject owner, MenuTabWrapper tabWrapper, Vector2 pos, float settingsWidth = 300)
        {
            if (arenaTeamComboBox != null)
            {
                if (arenaTeamComboBox.greyedOut = arena.currentGameMode != TeamBattleMode.TeamBattle.value)
                    if (!arenaTeamComboBox.held && !teamComboBoxLastHeld) arenaTeamComboBox.value = OnlineManager.lobby.clientSettings[OnlineManager.mePlayer].GetData<ArenaTeamClientSettings>().team.ToString();
            }

            if (arenaTeamComboBox != null)
            {
                var alListItems = arenaTeamComboBox.GetItemList();

                for (int i = 0; i < alListItems.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            if (alListItems[i] != null && alListItems[i].name != martyrsTeamName)
                            {
                                alListItems[i].name = martyrsTeamName;
                                alListItems[i].desc = martyrsTeamName;
                                alListItems[i].displayName = martyrsTeamName;
                            }
                            break;
                        case 1:
                            if (alListItems[i] != null && alListItems[i].name != chieftainsTeamNames)
                            {
                                alListItems[i].name = chieftainsTeamNames;
                                alListItems[i].desc = chieftainsTeamNames;
                                alListItems[i].displayName = chieftainsTeamNames;
                            }
                            break;
                        case 2:
                            if (alListItems[i] != null && alListItems[i].name != dragonSlayersTeamNames)
                            {
                                alListItems[i].name = dragonSlayersTeamNames;
                                alListItems[i].desc = dragonSlayersTeamNames;
                                alListItems[i].displayName = dragonSlayersTeamNames;
                            }
                            break;
                        case 3:
                            if (alListItems[i] != null && alListItems[i].name != outlawTeamNames)
                            {
                                alListItems[i].name = outlawTeamNames;
                                alListItems[i].desc = outlawTeamNames;
                                alListItems[i].displayName = outlawTeamNames;
                            }
                            break;
                    }

                }

            }
        }

        private void ColorSelector_OnValueChangedEvent()
        {
            TeamColors[0] = Extensions.SafeColorRange(martyrColor.valuecolor);
            TeamColors[1] = Extensions.SafeColorRange(outlawColor.valuecolor);
            TeamColors[2] = Extensions.SafeColorRange(dragonSlayerColor.valuecolor);
            TeamColors[3] = Extensions.SafeColorRange(chieftainColor.valuecolor);

        }



        public override void ArenaPlayerBox_GrafUpdate(ArenaMode arena, ArenaPlayerBox self, float timestacker, bool showRainbow, FLabel pingLabel, FSprite[] sprites, List<UiLineConnector> lines, MenuLabel selectingStatusLabel, ProperlyAlignedMenuLabel nameLabel, OnlinePlayer profileIdentifier, SlugcatColorableButton slugcatButton)
        {
            base.ArenaPlayerBox_GrafUpdate(arena, self, timestacker, showRainbow, pingLabel, sprites, lines, selectingStatusLabel, nameLabel, profileIdentifier, slugcatButton);
            Color rainbow = ArenaPlayerBox.MyRainbowColor(self.rainbowColor, showRainbow);
            if (TeamBattleMode.isTeamBattleMode(arena, out var tb))
            {
                if (OnlineManager.lobby.clientSettings.TryGetValue(profileIdentifier, out var clientSettings))
                {

                    if (clientSettings.TryGetData<ArenaTeamClientSettings>(out var team))
                    {
                        if (team.team == tb.winningTeam && tb.winningTeam != -1)
                        {
                            slugcatButton.secondaryColor = rainbow;
                        }
                        else
                        {
                            slugcatButton.secondaryColor = TeamBattleMode.TeamColors[team.team];
                        }
                    }

                }
            }

        }

        public override void ArenaPlayerBox_Update(ArenaMode arena, ArenaPlayerBox self)
        {
            self.rainbowColor.hue = ArenaPlayerBox.GetLerpedRainbowHue();
            self.slugcatButton.portraitSecondaryLerpFactor = ArenaPlayerBox.GetLerpedRainbowHue(self.showRainbow ? 0.75f : 0f);
            self.realPing = System.Math.Max(1, self.profileIdentifier.ping - 16);
            self.lastTextOverlayFade = self.textOverlayFade;
            self.textOverlayFade = self.enabledTextOverlay ? RWCustom.Custom.LerpAndTick(self.textOverlayFade, 1f, 0.02f, 1f / 60f) : RWCustom.Custom.LerpAndTick(self.textOverlayFade, 0f, 0.12f, 0.1f);
            self.slugcatButton.isBlackPortrait = self.enabledTextOverlay;
        }

        public override string AddGameSettingsTab()
        {
            return "Team Settings";
        }

        public override DialogNotify AddGameModeInfo(Menu.Menu menu)
        {
            return new DialogNotify(menu.LongTranslate("Choose a faction. Last team standing wins."), new Vector2(500f, 400f), menu.manager, () => { menu.PlaySound(SoundID.MENU_Button_Standard_Button_Pressed); });
        }

        public static Color GetColorFromHex(string hexCode)
        {
            Color color;
            // TryParseHtmlString returns true if the conversion was successful
            if (ColorUtility.TryParseHtmlString(hexCode, out color))
            {
                return color;
            }
            else
            {
                Debug.LogError("Invalid hex code: " + hexCode + ". Returning default color.");
                return Color.magenta; // Or any default/error color you prefer
            }
        }

        public float SetTeamLerp()
        {
            return RainMeadow.rainMeadowOptions.TeamColorLerp.Value;
        }
    }
}
