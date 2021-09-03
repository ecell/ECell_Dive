using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace ECellDive
{
    namespace IInteractions
    {
        public interface IHighlightable
        {
            bool highlighted { get; }
            void SetHighlight();
            void UnsetHighlight();
        }

        public interface IFloatingDisplayable
        {
            GameObject refFloatingPlanel { get;}
            InputActionReference refTriggerFloatingPlanel { get; set; }
            void ActivateFloatingDisplay();
            void DeactivateFloatingDisplay();
        }

        public interface IFixedDisplayable
        {
            GameObject refFixedPlanel { get; }
            void ActivateFixedDisplay();
        }
    }
}
