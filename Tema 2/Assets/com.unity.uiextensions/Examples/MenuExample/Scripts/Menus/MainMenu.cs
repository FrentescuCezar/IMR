using UnityEngine.InputSystem;

namespace UnityEngine.UI.Extensions.Examples
{
    public class MainMenu : SimpleMenu<MainMenu>
    {
        public InputActionProperty showButton;


        private void Update()
        {
            if (showButton.action.WasPerformedThisFrame())
            {
                OnPlayPressed();
            }
        }

        public void OnPlayPressed()
        {
            PlayMenu.Show();
        }

        public void OnOptionsPressed()
        {
            OptionsMenu.Show();
        }

        public override void OnBackPressed()
        {
            Application.Quit();
        }
    }
}