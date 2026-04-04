using Menu;
namespace RainMeadow
{
    /// <summary>
    /// A simple, self-contained dialog box that blocks other input and destroys itself upon pressing Continue. Automatically translates input text.
    /// </summary>
    public class SimpleDialogBoxNotify : Dialog //In order to temporarily disable other UI elements, we need to have a Dialog to pass into ShowDialog. DialogBox is somehow not a Dialog so we need a wrapper.
    {
        public SimpleDialogBoxNotifyActualDialogBox actualDialog;
        public SimpleDialogBoxNotify(Menu.Menu menu, MenuObject owner, string dialogText, string buttonText = "CONTINUE", bool fullSize = true, bool forceWrapping = true, float canCloseDelay = 0f) : base(menu.manager)
        {
            actualDialog = new(this, dialogPage, dialogText, buttonText, fullSize, forceWrapping, canCloseDelay);
            menu.manager.ShowDialog(this);
        }
        public class SimpleDialogBoxNotifyActualDialogBox : DialogBoxNotify
        {
            private bool continueEverClicked;
            private bool escEverClicked;
            private bool pauseEverClicked;
            public SimpleDialogBoxNotifyActualDialogBox(Menu.Menu menu, MenuObject owner, string dialogText, string buttonText = "CONTINUE", bool fullSize = true, bool forceWrapping = true, float canCloseDelay = 0f)
                : base(menu, owner, dialogText, "", new(menu.manager.rainWorld.options.ScreenSize.x / 2f - 240f + (1366f - menu.manager.rainWorld.options.ScreenSize.x) / 2f, 224f), new(fullSize ? 480f : 240f, 320f), forceWrapping)
            {
                owner.subObjects.Add(this);
                (continueEverClicked, escEverClicked, pauseEverClicked) = (false, false, false);
                descriptionLabel.text = menu.LongTranslate(descriptionLabel.text);
                continueButton.menuLabel.text = menu.Translate(buttonText);
                timeOut = canCloseDelay;
            }
            public override void Update()
            {
                base.Update();
                continueEverClicked |= continueButton.buttonBehav.clicked;
                escEverClicked |= UnityEngine.Input.GetKey(UnityEngine.KeyCode.Escape); //NEVER use GetKeyDown outside of a GrafUpdate. It WILL drop 90% of your inputs.
                pauseEverClicked |= RWInput.CheckPauseButton(0); //Why in the world is this not part of the input package?

                if (timeOut <= 0 && (
                       (continueEverClicked && !continueButton.buttonBehav.clicked)
                    || (escEverClicked      && !UnityEngine.Input.GetKey(UnityEngine.KeyCode.Escape)) //Esc *is* the keyboard pause button, but, keyboard might be rebound or disabled.
                    || (pauseEverClicked    && !RWInput.CheckPauseButton(0))
                    || (menu.lastInput.thrw && !menu.input.thrw)))
                {
                    RemoveSprites();
                }
            }
            public override void RemoveSprites()
            {
                base.RemoveSprites();
                owner.subObjects.Remove(this);
                owner.menu.manager.dialog = null; //This seems dangeorus but it's how basegame does it.
            }
        }
    }
}
