using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace ECellDive.Interfaces
{
    public interface INamed
    {
        TextMeshProUGUI nameField { get; }

        string GetName();
        void SetName(string _name);
    }
}

