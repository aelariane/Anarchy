using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    public class ProfilePanel : GUIPanel
    {
        private const int MainPage = 0;
        private const int AdvancedPage = 1;
        private const int PreviewPage = 2;
        private const int MaxProfiles = 15;

        private int checkProfile;
        private int currentProfile;
        private SmartRect left;
        private string newProfile;
        private int previewPage;
        private SmartRect right;

        //sort later

        public ProfilePanel() : base(nameof(ProfilePanel), GUILayers.ProfilePanel)
        {
            animator = new Animation.AngleAnimation(this, Animation.AngleAnimation.StartPoint.TopLeft, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
            if (animator is Animation.AngleAnimation anim)
            {
                anim.Speed = 1.5f;
            }
        }

        [GUIPage(AdvancedPage)]
        private void AdvancedSettings()
        {
            right.Reset();
            LabelCenter(right, locale["advancedHead"], true);
            TextField(right, User.DieName, locale["diename"], Style.LabelOffset, true);
            TextField(right, User.RaceFinish, locale["raceFinish"], Style.LabelOffset, true);
            TextField(right, User.DieNameFormat, locale["dieStyle"], Style.LabelOffset, true);
            TextField(right, User.WaveFormat, locale["wave"], Style.LabelOffset, true);
            right.MoveY();
            LabelCenter(right, locale["chatSettings"], true);
            TextField(right, User.ChatFormat, locale["chatFormat"], Style.LabelOffset, true);
            TextField(right, User.ChatPmFormat, locale["chatPMFormat"], Style.LabelOffset, true);
            LabelCenter(right, locale["chatFormatSend"], true);
            TextField(right, User.ChatFormatSend, string.Empty, 0f, true);
            LabelCenter(right, locale["chatPMFormatSend"], true);
            TextField(right, User.ChatPmFormatSend, string.Empty, 0f, true);
            right.MoveY();
            LabelCenter(right, locale["others"], true);
            LabelCenter(right, locale["restart"], true);
            TextField(right, User.RestartMessage, string.Empty, 0f, true);
            LabelCenter(right, locale["mcswitch"], true);
            TextField(right, User.McSwitch, string.Empty, 0f, true);

            right.MoveToEndY(BoxPosition, Style.Height * 2f + Style.VerticalMargin);
            if (Button(right, locale["preview"], true))
            {
                pageSelection = PreviewPage;
                return;
            }
            right.BeginHorizontal(2);
            if (Button(right, locale["btnToMain"], false))
            {
                pageSelection = MainPage;
                return;
            }
            right.MoveX();
            if (Button(right, locale["btnBack"], false))
            {
                Disable();
                return;
            }
        }

        private void CheckChange()
        {
            if (checkProfile == currentProfile)
            {
                return;
            }
            checkProfile = currentProfile;
            User.LoadProfile(User.AllProfiles[checkProfile]);
        }

        protected override void DrawMainPart()
        {
            left.Reset();
            LabelCenter(left, locale["profileSel"], true);

            newProfile = TextField(left, newProfile, string.Empty, 0f, true);
            left.BeginHorizontal(2);
            if (Button(left, locale["btnAdd"], false))
            {
                if (User.AllProfiles.Length < MaxProfiles)
                {
                    User.LoadProfile(newProfile);
                    newProfile = "Profile" + (User.AllProfiles.Length + 1);
                    currentProfile = FindCurrentProfile();
                }
                else
                {
                    //Put log
                }
            }
            left.MoveX();
            if (Button(left, locale["btnCopy"], true))
            {
                if (User.AllProfiles.Length < MaxProfiles)
                {
                    User.CopyProfile(newProfile, User.ProfileName);
                    User.LoadProfile(newProfile);
                    newProfile = "Profile" + (User.AllProfiles.Length + 1);
                    currentProfile = FindCurrentProfile();
                }
                else
                {
                    //Put log
                }
            }
            left.ResetX();
            if (Button(left, locale["btnRemove"], true))
            {
                if (User.AllProfiles.Length > 1)
                {
                    User.DeleteProfile(User.ProfileName);
                    newProfile = User.ProfileName;
                    currentProfile = FindCurrentProfile();
                }
                else
                {
                    //Put log
                }
            }

            Label(left, locale["note"], true);
            LabelCenter(left, locale["allProfiles"], true);
            currentProfile = SelectionGrid(left, currentProfile, User.AllProfiles, 1, true);
            CheckChange();
        }

        private int FindCurrentProfile()
        {
            for (int i = 0; i < User.AllProfiles.Length; i++)
            {
                if (User.AllProfiles[i] == User.ProfileName)
                {
                    return i;
                }
            }
            return -1;
        }

        [GUIPage(MainPage)]
        private void MainSettings()
        {
            right.Reset();
            LabelCenter(right, locale["mainSettings"], true);
            TextField(right, User.Name, locale["name"], Style.LabelOffset, true);
            TextField(right, User.GuildNames[0], locale["guildName"], Style.LabelOffset, true);
            TextField(right, User.GuildNames[1], locale["guildName2"], Style.LabelOffset, true);
            TextField(right, User.GuildNames[2], locale["guildName3"], Style.LabelOffset, true);
            TextField(right, User.ChatName, locale["chatName"], Style.LabelOffset, true);
            TextField(right, User.MainColor, locale["mainColor"], Style.LabelOffset, true);
            TextField(right, User.SubColor, locale["subColor"], Style.LabelOffset, true);

            LabelCenter(right, locale["killTriggers"], true);
            TextField(right, User.Suicide, locale["suicide"], Style.LabelOffset, true);
            TextField(right, User.AkinaKillTrigger, locale["akinaTrigger"], Style.LabelOffset, true);
            TextField(right, User.ForestLavaKillTrigger, locale["forestTrigger"], Style.LabelOffset, true);
            TextField(right, User.RacingKillTrigger, locale["racingTrigger"], Style.LabelOffset, true);

            LabelCenter(right, locale["titans"], true);
            TextField(right, User.TitanNames[0], locale["titan"], Style.LabelOffset, true);
            TextField(right, User.TitanNames[1], locale["aberrant"], Style.LabelOffset, true);
            TextField(right, User.TitanNames[2], locale["jumper"], Style.LabelOffset, true);
            TextField(right, User.TitanNames[3], locale["crawler"], Style.LabelOffset, true);
            TextField(right, User.TitanNames[4], locale["punk"], Style.LabelOffset, true);

            right.MoveToEndY(BoxPosition, Style.Height * 2f + Style.VerticalMargin);
            if (Button(right, locale["preview"], true))
            {
                pageSelection = PreviewPage;
                return;
            }
            right.BeginHorizontal(2);
            if (Button(right, locale["btnAdvanced"], false))
            {
                pageSelection = AdvancedPage;
                return;
            }
            right.MoveX();
            if (Button(right, locale["btnBack"], false))
            {
                Disable();
                return;
            }
        }

        protected override void OnPanelDisable()
        {
            left = null;
            right = null;
            newProfile = null;
            Localization.Language.UpdateFormats();
            if (PhotonNetwork.inRoom)
            {
                var hash = new ExitGames.Client.Photon.Hashtable();
                hash.Add(PhotonPlayerProperty.name, User.Name.Value);
                hash.Add(PhotonPlayerProperty.guildName, User.AllGuildNames);
                PhotonNetwork.player.SetCustomProperties(hash);
            }
        }

        protected override void OnPanelEnable()
        {
            SmartRect[] rects = Helper.GetSmartRects(BoxPosition, 2);
            left = rects[0];
            right = rects[1];
            newProfile = "Profile" + (User.AllProfiles.Length + 1);
            currentProfile = FindCurrentProfile();
            checkProfile = currentProfile;
            previewPage = 0;
        }

        [GUIPage(PreviewPage)]
        private void Preview()
        {
            right.Reset();
            previewPage = SelectionGrid(right, previewPage, locale.GetArray("previews"), 2, true);

            if (previewPage == 0)
            {
                PreviewOne();
            }
            else
            {
                PreviewSecond();
            }

            right.MoveToEndY(BoxPosition, Style.Height * 2f + Style.VerticalMargin);
            if (Button(right, locale["preview"], true))
            {
                pageSelection = MainPage;
                return;
            }
            right.BeginHorizontal(2);
            if (Button(right, locale["btnAdvanced"], false))
            {
                pageSelection = AdvancedPage;
                return;
            }
            right.MoveX();
            if (Button(right, locale["btnBack"], false))
            {
                Disable();
                return;
            }
        }

        private void PreviewOne()
        {
            LabelCenter(right, locale["mainSettings"], true);
            Label(right, locale["name"] + " " + User.Name.ToValue().ToHTMLFormat(), true);
            Label(right, locale["guildName"] + " " + User.GuildNames[0].ToValue().ToHTMLFormat(), true);
            Label(right, locale["guildName2"] + " " + User.GuildNames[1].ToValue().ToHTMLFormat(), true);
            Label(right, locale["guildName3"] + " " + User.GuildNames[2].ToValue().ToHTMLFormat(), true);
            Label(right, locale["chatName"] + " " + User.ChatName.ToValue(), true);
            Label(right, string.Format(locale["previewColors"], User.MainColor.ToValue(), User.SubColor.ToValue()), true);

            LabelCenter(right, locale["killTriggers"], true);
            Label(right, locale["suicide"] + " " + User.Suicide.ToValue().ToHTMLFormat(), true);
            Label(right, locale["akinaTrigger"] + " " + User.AkinaKillTrigger.ToValue().ToHTMLFormat(), true);
            Label(right, locale["forestTrigger"] + " " + User.ForestLavaKillTrigger.ToValue().ToHTMLFormat(), true);
            Label(right, locale["racingTrigger"] + " " + User.RacingKillTrigger.ToValue().ToHTMLFormat(), true);

            LabelCenter(right, locale["titans"], true);
            Label(right, locale["titan"] + " " + User.TitanNames[0].ToValue().ToHTMLFormat(), true);
            Label(right, locale["aberrant"] + " " + User.TitanNames[1].ToValue().ToHTMLFormat(), true);
            Label(right, locale["jumper"] + " " + User.TitanNames[2].ToValue().ToHTMLFormat(), true);
            Label(right, locale["crawler"] + " " + User.TitanNames[3].ToValue().ToHTMLFormat(), true);
            Label(right, locale["punk"] + " " + User.TitanNames[4].ToValue().ToHTMLFormat(), true);
        }

        private void PreviewSecond()
        {
            LabelCenter(right, locale["advancedHead"], true);
            Label(right, locale["diename"] + " " + User.DeathNameFull.ToHTMLFormat(), true);
            Label(right, locale["raceFinish"] + " " + User.RaceName.ToHTMLFormat(), true);
            Label(right, locale["dieStyle"] + " " + User.DeathFormat(1, "[FF0000]Killer").ToHTMLFormat() + " <b>X</b> " + User.DeathNameFull.ToHTMLFormat(), true);
            Label(right, locale["wave"] + " " + User.Wave(3), true);

            right.MoveY();
            LabelCenter(right, locale["chatSettings"], true);
            Label(right, locale["chatFormat"] + " " + User.Chat(1, "Player: Message").ToHTMLFormat(), true);
            Label(right, locale["chatPMFormat"] + " " + User.ChatPm(1, "Player: Message").ToHTMLFormat(), true);
            Label(right, locale["chatFormatSend"] + ": " + User.ChatSend("message").ToHTMLFormat(), true);
            Label(right, locale["chatPMFormatSend"] + ": " + User.ChatPmSend(1, "<color=white>Message</color>").ToHTMLFormat(), true);

            right.MoveY();
            LabelCenter(right, locale["others"], true);
            LabelCenter(right, locale["restart"], true);
            LabelCenter(right, User.MsgRestart.ToHTMLFormat(), true);
            LabelCenter(right, locale["mcswitch"], true);
            LabelCenter(right, User.MasterClientSwitch.ToHTMLFormat(), true);
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }
        }

        public override void OnUpdateScaling()
        {
            animator = new Animation.AngleAnimation(this, Animation.AngleAnimation.StartPoint.TopLeft, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
            if (animator is Animation.AngleAnimation anim)
            {
                anim.Speed = 1.5f;
            }
        }
    }
}