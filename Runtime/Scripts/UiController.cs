using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Bakezori.Essentials.Core;

namespace Bakezori.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UiController : MonoBehaviour
    {
        [BoxGroup("Modals"), SerializeField] private NotificationModalController notificationModalController;
        [Tooltip("Only one screen can appear at a time.")]
        [BoxGroup("Modals"), SerializeField] List<ScreenController> screenControllers = new List<ScreenController>();

        [BoxGroup("References"), SerializeField] private UIDocument uiDocument;
        [BoxGroup("References"), SerializeField] private DebugOverlayController debugOverlayController;

        private VisualElement rootVisualElement;
        private string activeScreenId = string.Empty;
        private Dictionary<string, ScreenController> idToScreenControllers = new Dictionary<string, ScreenController>();

        public VisualElement RootVisualElement { get => this.rootVisualElement; }
        public bool HasActiveScreen { get => !string.IsNullOrEmpty(this.activeScreenId); }

        private void Awake()
        {
            this.rootVisualElement = this.uiDocument.rootVisualElement;

            this.notificationModalController.Initialize(this);
            this.debugOverlayController.Initialize(this);

            foreach (var screenController in this.screenControllers)
            {
                screenController.Initialize(this);
                screenController.OnShow += OnScreenShow;
                screenController.OnHide += OnScreenHide;

                this.idToScreenControllers[screenController.Id] = screenController;
            }
        }

        private void OnDestroy()
        {
            foreach (var screenController in this.screenControllers)
            {
                screenController.OnShow -= OnScreenShow;
                screenController.OnHide -= OnScreenHide;
            }
        }

        private void ShowScreen(string id)
        {
            Assert.IsTrue(this.notificationModalController.IsFree, $"Trying to show screen {id} when there's an active notification.");
            Assert.IsFalse(HasActiveScreen, $"Trying to show screen {id} when {this.activeScreenId} is active.");
            idToScreenControllers[id].TryShow();
            this.activeScreenId = id;
        }

        private void HideScreen(string id)
        {
            Assert.IsTrue(this.activeScreenId == id, $"Trying to hide screen {id} when it is not active.");
            idToScreenControllers[id].TryHide();
            this.activeScreenId = string.Empty;
        }

        #region EventHandlers
        private void OnScreenShow(string id)
        {
#if UNITY_EDITOR
            if (this.activeScreenId != id)
            {
                GameManager.LogWarning($"Showing screen {id} when it is not shown. Screen show/hide must be handled from the UIController.");
                this.activeScreenId = id;
            }
#else
            Assert.IsTrue(this.activeScreenId == id, $"Showing screen {id} when is it is not shown. Screen show/hide must be handled from the UIController.");
#endif
        }

        private void OnScreenHide(string id)
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(this.activeScreenId))
            {
                GameManager.LogWarning($"Hiding screen {id} when it is not hidden. Screen show/hide must be handled from the UIController");
                this.activeScreenId = string.Empty;
            }
#else
            Assert.IsTrue(string.IsNullOrEmpty(this.activeScreenId), $"Hiding screen {id} when it is not hidden. Screen show/hide must be handled from the UIController");
#endif
        }
        #endregion
    }
}