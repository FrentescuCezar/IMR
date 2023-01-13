using UnityEngine.InputSystem;

namespace UnityEngine.UI.Extensions.Examples
{
    public class OptionsMenu : SimpleMenu<OptionsMenu>
    {
        public InputActionProperty showButton;

        private void Update()
        {
            if (showButton.action.WasPerformedThisFrame())
            {
                OnPlayPressed();
            }
        }

        public Slider Slider;

        public void OnMagicButtonPressed()
        {
            AwesomeMenu.Show(Slider.value);
        }

        public void OnPlayPressed()
        {
            PlayMenu.Show();
        }
    }
}