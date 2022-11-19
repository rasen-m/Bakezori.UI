using System;
using UnityEngine.UIElements;
using Bakezori.Essentials.Managers;
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

        private bool isShowOkayButton = true;
        private float defaultGrowlTimeToLive = 3f;

        public override string RootVisualElementId => "NotificationModal";

        public override void Initialize(UiController uiController)
        {
            base.Initialize(uiController);

            this.titleLabel = this.rootVisualElement.Q<Label>("TitleLabel");
            this.bodyLabel = this.rootVisualElement.Q<Label>("BodyLabel");

            this.closeButton = this.rootVisualElement.Q<Button>("CloseButton");

            this.yesButton = this.rootVisualElement.Q<Button>("YesButton");
            this.noButton = this.rootVisualElement.Q<Button>("NoButton");
            this.okayButton = this.rootVisualElement.Q<Button>("OkayButton");

            /// Self subscription.
            GameManager.NotificationManager.SetNotificationController(this);
        }

        public void SetDefaultSettings(NotificationSettings notificationSettings)
        {
            // this.backgroundVisualElement..display = notificationSettings.IsBackgroundDismissible ? DisplayStyle.Flex : DisplayStyle.None;
            this.closeButton.style.display = notificationSettings.IsShowCloseButton ? DisplayStyle.Flex : DisplayStyle.None;

            this.yesButton.text = notificationSettings.AlertYesButtonLabel;
            this.noButton.text = notificationSettings.AlertNoButtonLabel;
            this.okayButton.text = notificationSettings.AlertOkayButtonLabel;

            this.isShowOkayButton = notificationSettings.IsShowOkayButton;
            this.defaultGrowlTimeToLive = notificationSettings.GrowlTimeToLive;
        }

        public bool TryShow(AlertData data)
        {
            if (!this.isBusy)
            {
                this.titleLabel.text = data.Title;
                this.titleLabel.style.display = string.IsNullOrWhiteSpace(data.Title) ? DisplayStyle.None : DisplayStyle.Flex;
                this.bodyLabel.text = data.Body;

                this.yesButton.style.display = DisplayStyle.Flex;
                this.noButton.style.display = DisplayStyle.Flex;
                this.okayButton.style.display = DisplayStyle.None;

                if (TryShow())
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryShow(PopupData data)
        {
            if (!this.isBusy)
            {
                this.titleLabel.text = data.Title;
                this.titleLabel.style.display = string.IsNullOrWhiteSpace(data.Title) ? DisplayStyle.None : DisplayStyle.Flex;
                this.bodyLabel.text = data.Body;
                this.yesButton.style.display = DisplayStyle.None;
                this.noButton.style.display = DisplayStyle.None;
                this.okayButton.style.display = this.isShowOkayButton ? DisplayStyle.Flex : DisplayStyle.None;

                if (TryShow())
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool TryShow(GrowlData data)
        {
            if (!this.isBusy)
            {
                this.titleLabel.text = data.Title;
                this.titleLabel.style.display = string.IsNullOrWhiteSpace(data.Title) ? DisplayStyle.None : DisplayStyle.Flex;
                this.bodyLabel.text = data.Body;
                this.yesButton.style.display = DisplayStyle.None;
                this.noButton.style.display = DisplayStyle.None;
                this.okayButton.style.display = DisplayStyle.None;

                float timeToLive = data.TimeToLive > 0 ? data.TimeToLive : this.defaultGrowlTimeToLive;

                if (TryShow(() => HideAfter(timeToLive, data.Callback)))
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
