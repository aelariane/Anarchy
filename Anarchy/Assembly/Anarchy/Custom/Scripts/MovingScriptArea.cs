using System;
using UnityEngine;

namespace Anarchy.Custom.Scripts
{
    /// <summary>
    /// Represents segment for <seealso cref="MovingScript"/>
    /// </summary>
    /// <remarks>
    /// Area is segment, that contains start and end position, and optionally rotations.
    /// Object will smoothly travel from <seealso cref="StartPosition"/>  to <seealso cref="TargetPosition"/>
    /// If rotations are also used, object will smotthly rotate from <seealso cref="StartRotation"/> to <seealso cref="TargetRotation"/>, then back, as object moves
    /// If <seealso cref="Delay"/> is more then 0, then object will wait <seealso cref="Delay"/> seconds before it starts moving. It does pause when reached one of the points
    /// </remarks>
    public class MovingScriptArea
    {

        /// <summary>
        /// Time to wait time before object starts moving again, once it reached one of points
        /// </summary>
        public float Delay { get; set; } = 0f;

        /// <summary>
        /// Speed of object while it is moving between <seealso cref="StartPosition"/> and <seealso cref="TargetPosition"/> points
        /// </summary>
        public float Speed { get; set; } = 10f;

        /// <summary>
        /// Start point of object
        /// </summary>
        public Vector3 StartPosition { get; set; }
        /// <summary>
        /// Base rotation of object
        /// </summary>
        public Quaternion StartRotation { get; set; }

        /// <summary>
        /// Target point for object to move to
        /// </summary>
        public Vector3 TargetPosition { get; set; }

        /// <summary>
        /// Rotation to rotate to
        /// </summary>
        public Quaternion TargetRotation { get; set; }

        private MovingScriptArea()
        {
        }

        /// <summary>
        /// Parses area from given string
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="includeRotations">If rotations are used</param>
        /// <param name="includeSpeed">If custom speed is applied</param>
        /// <param name="includeDelay">If object has delay before it moves</param>
        /// <returns>Parsed <seealso cref="MovingScriptArea"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if string had incorrect format</exception>
        public static MovingScriptArea FromString(string source, bool includeRotations, bool includeSpeed, bool includeDelay)
        {
            string[] sourceArray = source.Split(',');
            int index = 0;
            MovingScriptArea result = new MovingScriptArea();

            //Parsing StartPosition and TargetPosition
            result.StartPosition = new Vector3(
                Convert.ToSingle(sourceArray[index++]),
                Convert.ToSingle(sourceArray[index++]), 
                Convert.ToSingle(sourceArray[index++]));


            result.TargetPosition = new Vector3(
                Convert.ToSingle(sourceArray[index++]),
                Convert.ToSingle(sourceArray[index++]),
                Convert.ToSingle(sourceArray[index++]));

            //Parsing StartRotation and TargetRotation
            if (includeRotations)
            {
                result.StartRotation = new Quaternion(
                    Convert.ToSingle(sourceArray[index++]),
                    Convert.ToSingle(sourceArray[index++]),
                    Convert.ToSingle(sourceArray[index++]), 
                    Convert.ToSingle(sourceArray[index++]));

                result.TargetRotation = new Quaternion(
                    Convert.ToSingle(sourceArray[index++]),
                    Convert.ToSingle(sourceArray[index++]),
                    Convert.ToSingle(sourceArray[index++]),
                    Convert.ToSingle(sourceArray[index++]));
            }
            //Parsing speed
            if (includeSpeed)
            {
                result.Speed = Convert.ToSingle(sourceArray[index++]);
            }
            //Parsing delay
            if (includeDelay)
            {
                result.Delay = Convert.ToSingle(sourceArray[index]);
            }

            return result;
        }
    }
}