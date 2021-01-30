namespace Anarchy.UI.Animation
{
    internal class NoneAnimation : GUIAnimation
    {
        public NoneAnimation(GUIBase @base) : base(@base)
        {
        }

        protected override bool Close()
        {
            return false;
        }

        protected override bool Open()
        {
            myBase.Draw();
            return false;
        }
    }
}