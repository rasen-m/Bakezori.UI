using System;
using UnityEngine.UIElements;
using Bakezori.Essentials.Core;
using Bakezori.Essentials.Notification;
using Bakezori.Essentials.Utility;

namespace Bakezori.UI
{
    public class NotificationModalController : ModalController, INotificationController
    {
        private Label titleLabel;
        private Label bodyLabel;

        private Button closeButton;

        private Button yesButton;
        private Button noButton;
        private Button okayButton;

        private Label yesButtonLabel;
        private Label noButtonLabel;
        private Label okayButtonLabel;

        private bool isShowOkayButton = true;
        private float defaultGrowlTimeToLive = 3f;

        public override string RootVisualElementId => "NotificationModal";

        private event Action onYesCallback = null;
        private event Action onNoCallback = null;
        private event Action onOkayCallback = null;

        public override void Initialize(UiController uiController)
        {
            base.Initialize(uiController);

            this.titleLabel = RootVisualElement.Q<Label>("TitleLabel");
            this.bodyLabel = RootVisualElement.Q<Label>("BodyLabel");

            this.closeButton = RootVisualElement.Q<Button>("CloseButton");

            this.yesButton = RootVisualElement.Q<Button>("YesButton");
            this.noButton = RootVisualElement.Q<Button>("NoButton");
            this.okayButton = RootVisualElement.Q<Button>("OkayButton");

            this.yesButtonLabel = this.yesButton.Q<Label>();
            this.noButtonLabel = this.noButton.Q<Label>();
            this.okayButtonLabel = this.okayButton.Q<Label>();

            this.yesButton.RegisterCallback<ClickEvent>(e => 
            {
                this.onYesCallback?.Invoke();
                e.StopPropagation();
                TryHide();
            });

            this.noButton.RegisterCallback<ClickEvent>(e => 
            {
                this.onNoCallback?.Invoke();
                e.StopPropagation();
                TryHide();
            });

            this.okayButton.RegisterCallback<ClickEvent>(e => 
            {
                this.onOkayCallback?.Invoke();
                e.StopPropagation();
                TryHide();
            });

            /// Self subscription.
            GameManager.GetManager<NotificationManager>().SetNotificationController(this);
        }

        public void SetDefaultSettings(NotificationSettings notificationSettings)
        {
            // this.backgroundVisualElement..display = notificationSettings.IsBackgroundDismissible ? DisplayStyle.Flex : DisplayStyle.None;
            this.closeButton.style.display = notificationSettings.IsShowCloseButton ? DisplayStyle.Flex : DisplayStyle.None;

            this.yesButtonLabel.text = notificationSettings.AlertYesButtonLabel;
            this.noButtonLabel.text = notificationSettings.AlertNoButtonLabel;
            this.okayButtonLabel.text = notificationSettings.AlertOkayButtonLabel;

            this.isShowOkayButton = notificationSettings.IsShowOkayButton;
            this.defaultGrowlTimeToLive = notificationSettings.GrowlTimeToLive;
        }

        public bool TryShow(AlertData data)
        {
            if (!IsBusy)
            {
                this.titleLabel.text = data.Title;
                this.titleLabel.style.display = string.IsNullOrWhiteSpace(data.Title) ? DisplayStyle.None : DisplayStyle.Flex;
                this.bodyLabel.text = data.Body;
                IsHideDialogOnClicked = false;
                OnHideCallback = data.OnHideCallback;

                this.yesButton.style.display = DisplayStyle.Flex;
                this.noButton.style.display = DisplayStyle.Flex;
                this.okayButton.style.display = DisplayStyle.None;

                this.onYesCallback = data.OnYesCallback;
                this.onNoCallback = data.OnNoCallback;
                this.onOkayCallback = null;

                if (TryShow())
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryShow(PopupData data)
        {
            if (!IsBusy)
            {
                this.titleLabel.text = data.Title;
                this.titleLabel.style.display = string.IsNullOrWhiteSpace(data.Title) ? DisplayStyle.None : DisplayStyle.Flex;
                this.bodyLabel.text = data.Body;
                IsHideDialogOnClicked = true;
                OnHideCallback = data.OnHideCallback;

                this.yesButton.style.display = DisplayStyle.None;
                this.noButton.style.display = DisplayStyle.None;
                this.okayButton.style.display = this.isShowOkayButton ? DisplayStyle.Flex : DisplayStyle.None;

                this.onYesCallback = null;
                this.onNoCallback = null;
                this.onOkayCallback = null;

                if (TryShow())
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool TryShow(GrowlData data)
        {
            if (!IsBusy)
            {
                this.titleLabel.text = data.Title;
                this.titleLabel.style.display = string.IsNullOrWhiteSpace(data.Title) ? DisplayStyle.None : DisplayStyle.Flex;
                this.bodyLabel.text = data.Body;
                IsHideDialogOnClicked = true;
                OnHideCallback = data.OnHideCallback;

                this.yesButton.style.display = DisplayStyle.None;
                this.noButton.style.display = DisplayStyle.None;
                this.okayButton.style.display = DisplayStyle.None;

                this.onYesCallback = null;
                this.onNoCallback = null;
                this.onOkayCallback = null;

                float timeToLive = data.TimeToLive > 0 ? data.TimeToLive : this.defaultGrowlTimeToLive;

                if (TryShow(() => HideAfter(timeToLive)))
                {
                    return true;
                }
            }

            return false;
        }

        public void HideAfter(float timeToLive, Action callback = null)
        {
            StartCoroutine(CoroutineUtility.ExecuteAfterCoroutine(timeToLive, () => TryHide(callback)));
        }
    }
}
