using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECellDive
{
    namespace Utility
    {
        /// <summary>
        /// Utility class to solve frequent GOs positioning problems in the scene.
        /// </summary>
        public static class Positioning
        {
            /// <summary>
            /// Used to place an object in front of a target at a certain 
            /// distance and relative height.
            /// </summary>
            /// <param name="_target">The transform from which we make the 
            /// computation.</param>
            /// <param name="_distance">The distance from <paramref name="_target"/>.</param>
            /// <param name="_relative_height">The height relatively to <paramref name="_target"/>'s,
            /// height.</param>
            /// <returns>The desired position in front of the target at a certain 
            /// distance and relative height.</returns>
            public static Vector3 PlaceInFrontOfTarget(Transform _target, float _distance, float _relative_height)
            {
                Vector3 fromTargetPos = _target.position + _distance * _target.forward;
                return new Vector3(fromTargetPos.x,
                                   _relative_height * _target.position.y,
                                   fromTargetPos.z);
            }

            /// <summary>
            /// Maily useful for UI.
            /// Makes it face the user.
            /// </summary>
            /// <param name="_UIContainer">The parent gameobject of the UI
            /// (usually has the canvas components)</param>
            /// <param name="_target">The transform the UI should face (i.e.
            /// be readable from)</param>
            public static void UIFaceTarget(GameObject _UIContainer, Transform _target)
            {
                _UIContainer.transform.LookAt(_target);
                _UIContainer.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
            }

            /// <summary>
            /// The planar XY positions of a point on a circle
            /// </summary>
            /// <param name="_radius">The radius of the circle.</param>
            /// <param name="_angle">The angle in degrees.</param>
            /// <returns> _radius * (cos(_rad_angle), sin(_rad_angle))</returns>
            public static Vector2 RadialPosition(float _radius, float _angle)
            {
                return _radius * new Vector2(Mathf.Cos(Mathf.Deg2Rad * _angle), Mathf.Sin(Mathf.Deg2Rad * _angle));
            }


        }
    }
}

