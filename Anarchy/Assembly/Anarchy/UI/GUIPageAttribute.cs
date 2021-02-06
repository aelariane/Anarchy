using System;

namespace Anarchy.UI
{
    /// <summary>
    /// Indicates that method is special pagination method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GUIPageAttribute : Attribute
    {
        public readonly int Page;
        public readonly GUIPageType MethodType;

        /// <summary>
        /// Sets <seealso cref="GUIPageType.DrawMethod"/> to applied method
        /// </summary>
        /// <param name="page">Page index</param>
        public GUIPageAttribute(int page) : this(page, GUIPageType.DrawMethod)
        {
        }

        /// <summary>
        /// Specifies method as pagiation method
        /// </summary>
        /// <param name="page">Page index</param>
        /// <param name="type">Type of method</param>
        public GUIPageAttribute(int page, GUIPageType type)
        {
            Page = page;
            MethodType = type;
        }
    }
}