using System;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

namespace Bakezori.UI
{
    public abstract class ModalController : MonoBehaviour
    {
        // [Tooltip("Delay to give transition a chance to complete.")]
        // [SerializeField] protected float DisplayNoneDelay = 0.3f;

        public Action<string> OnShow;
        public Action<string> OnHide;

        [Tooltip("String ID from the UXML for this menu panel/screen.")]
        [ReadOnly, SerializeField] private string id;
        [ReadOnly, SerializeField] private bool isBusy;

        [BoxGroup("USS"), SerializeField] private string ussFadeShow = "fade-show";
        [BoxGroup("USS"), SerializeField] private string ussFadeHide = "fade-hide";
        [BoxGroup("USS"), SerializeField] private string ussScaleShow = "scale-show";
        [BoxGroup("USS"), SerializeField] private string ussScaleHide = "scale-hide";

        private UiController uiController;
        private VisualElement backgroundVisualElement;
        private VisualElement dialogVisualElement;

        public string Id => this.id;
        public bool IsVisible => this.isBusy;
        public bool IsFree => !this.isBusy;
        public bool IsBusy => this.isBusy;
        public bool IsHideDialogOnClicked { get; set; }

        protected VisualElement RootVisualElement { get; set; }
        protected Action OnHideCallback { get; set; }

        public abstract string RootVisualElementId { get; }

        public virtual void Initialize(UiController uiController)
        {
            this.uiController = uiController;

            RootVisualElement = this.uiController.RootVisualElement.Q(RootVisualElementId);
            RootVisualElement.style.display = DisplayStyle.Flex;
            this.backgroundVisualElement = RootVisualElement.Q("Background");
            this.dialogVisualElement = RootVisualElement.Q("Dialog");
            this.id = RootVisualElement.name;

            this.dialogVisualElement.RegisterCallback<ClickEvent>(e => 
            {
                e.StopPropagation();

                if (IsHideDialogOnClicked)
                {
                    TryHide();
                }
            });

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
                OnHideCallback?.Invoke();
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
                    // this.rootVisualElement.style.display = DisplayStyle.Flex;
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
                    // StartCoroutine(CoroutineUtility.ExecuteAfterCoroutine(DisplayNoneDelay, () =>
                    // {
                    // this.rootVisualElement.style.display = DisplayStyle.None;
                    this.isBusy = false;
                    // }));
                }
            }
        }
    }
}