using System;
using UnityEngine.UIElements;
using Bakezori.Essentials.Core;
using Bakezori.Essentials.Notification;
using Bakezori.Essentials.Utility;
using Bakezori.Essentials.Debugs;
using UnityEngine;

namespace Bakezori.UI
{
    public class DebugOverlayController : MonoBehaviour, IDebugOverlayController
    {
        private UiController uiController;
        private VisualElement rootVisualElement;

        private ScrollView debugLog;

        public void Initialize(UiController uiController)
        {
            this.uiController = uiController;
            this.rootVisualElement = this.uiController.RootVisualElement.Q("DebugOverlay");

            this.debugLog = this.rootVisualElement.Q<ScrollView>("DebugLog");

            /// Self subscription.
            GameManager.GetManager<DebugManager>().SetDebugOverlayController(this);
        }

        public void Log(string message)
        {
            var line = new Label();
            line.text = message;

            this.debugLog.contentContainer.Add(line);
        }
    }
}
