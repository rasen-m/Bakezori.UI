using System;
using UnityEngine;
using UnityEngine.UIElements;
using NaughtyAttributes;
using Bakezori.Essentials.Utility;
using Bakezori.Essentials.Notification;

namespace Bakezori.UI
{
    public abstract class ModalController : MonoBehaviour
    {
        [Tooltip("Delay to give transition a chance to complete.")]
        [SerializeField] protected float DisplayNoneDelay = 0.3f;

        public Action<string> OnShow;
        public Action<string> OnHide;

        [Tooltip("String ID from the UXML for this menu panel/screen.")]
        [ReadOnly, SerializeField] protected string id;
        [ReadOnly, SerializeField] protected bool isBusy;

        [BoxGroup("USS"), SerializeField] protected string ussFadeShow = "fade-show";
        [BoxGroup("USS"), SerializeField] protected string ussFadeHide = "fade-hide";
        [BoxGroup("USS"), SerializeField] protected string ussScaleShow = "scale-show";
        [BoxGroup("USS"), SerializeField] protected string ussScaleHide = "scale-hide";

        protected UiController uiController;
        protected VisualElement rootVisualElement;
        protected VisualElement backgroundVisualElement;
        protected VisualElement dialogVisualElement;

        public string Id { get => this.id; }
        public bool IsVisible { get => this.isBusy; }
        public bool IsFree { get => !this.isBusy; }
        public bool IsBusy { get => this.isBusy; }

        public abstract string RootVisualElementId { get; }

        public virtual void Initialize(UiController uiController)
        {
            this.uiController = uiController;

            this.rootVisualElement = this.uiController.RootVisualElement.Q(RootVisualElementId);
            this.backgroundVisualElement = this.rootVisualElement.Q("Background");
            this.dialogVisualElement = this.rootVisualElement.Q("Dialog");
            this.id = this.rootVisualElement.name;

            SetIsBusy(false, true);
        }

        [Button]
        public virtual bool TryShow(Action callback = null)
        {
            if (!this.isBusy)
            {
                SetIsBusy(true);
                OnShow?.Invoke(this.id);
                callback?.Invoke();

                return true;
            }

            return false;
        }

        [Button]
        public virtual bool TryHide(Action callback = null)
        {
            if (this.isBusy)
            {
                SetIsBusy(false);
                OnHide?.Invoke(this.id);
                callback?.Invoke();

                return true;
            }

            return false;
        }

        protected void SetIsBusy(bool isBusy, bool isForce = false)
        {
            if (this.isBusy != isBusy || isForce)
            {
                if (isBusy)
                {
                    this.isBusy = true;
                    this.rootVisualElement.style.display = DisplayStyle.Flex;
                    this.backgroundVisualElement.RemoveFromClassList(this.ussFadeHide);
                    this.backgroundVisualElement.AddToClassList(this.ussFadeShow);
                    this.dialogVisualElement.RemoveFromClassList(this.ussScaleHide);
                    this.dialogVisualElement.AddToClassList(this.ussScaleShow);
                }
                else
                {
                    this.backgroundVisualElement.RemoveFromClassList(this.ussFadeShow);
                    this.backgroundVisualElement.AddToClassList(this.ussFadeHide);
                    this.dialogVisualElement.RemoveFromClassList(this.ussScaleShow);
                    this.dialogVisualElement.AddToClassList(this.ussScaleHide);
                    StartCoroutine(CoroutineUtility.ExecuteAfterCoroutine(DisplayNoneDelay, () =>
                    {
                        this.rootVisualElement.style.display = DisplayStyle.None;
                        this.isBusy = false;
                    }));
                }
            }
        }
    }
}