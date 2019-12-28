using System;

namespace Anarchy.UI
{
    [AttributeUsage(AttributeTargets.Method)]   
    public class GUIPageAttribute : Attribute
    {
        public readonly int Page;
        public readonly GUIPageType MethodType;

        public GUIPageAttribute(int page) : this(page, GUIPageType.DrawMethod)
        {
        }

        public GUIPageAttribute(int page, GUIPageType type)
        {
            Page = page;
            MethodType = type;
        }
    }
}
