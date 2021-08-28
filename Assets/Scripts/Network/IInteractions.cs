using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            GameObject refFloatingPlanel { get; }
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
