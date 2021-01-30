using System;

namespace Anarchy.UI
{
    internal class CustomizationPanel : GUIPanel
    {
        private const int Intro = 0;
        private const int Text = 1;
        private const int Textures = 2;
        private const int Colors = 3;

        public CustomizationPanel() : base(nameof(CustomizationPanel), GUILayers.Customization)
        {
        }

        protected override void DrawMainPart()
        {
            throw new NotImplementedException();
        }

        protected override void OnPanelDisable()
        {
            throw new NotImplementedException();
        }

        protected override void OnPanelEnable()
        {
            throw new NotImplementedException();
        }
    }
}