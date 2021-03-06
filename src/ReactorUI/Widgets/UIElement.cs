﻿using ReactorUI.Animation;
using ReactorUI.Contracts;
using ReactorUI.Primitives;
using ReactorUI.Styles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReactorUI.Widgets
{
    public class UIElement<T, TS> : Widget<T>, IUIElement, IStyledWidget<TS> where T : class, IUIElement where TS : UIElementStyle<T>
    {
        public bool IsEnabled { get; set; } = false;
        public bool IsHitTestVisible { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public double Opacity { get; set; } = 1.0;
        public Transform Transform { get; set; } = new Transform();

        public UIElement()
        {
            Application.Current?.WidgetRegistry.ApplyDefaultStyle<T, TS>(this);
        }

        public void SetStyle(TS styleToApply)
        {
            OnStyleApplied(styleToApply);
        }

        protected virtual void OnStyleApplied(TS styleToApply)
        {
            Opacity = styleToApply.Opacity;
            Transform = styleToApply.Transform;

            if (styleToApply.OnMouseEnterAction != null)
                OnMouseEnterAction = new Action<IUIElement>(_ => styleToApply.OnMouseEnterAction?.Invoke((T)_));
            if (styleToApply.OnMouseLeaveAction != null)
                OnMouseLeaveAction = new Action<IUIElement>(_ => styleToApply.OnMouseLeaveAction?.Invoke((T)_));
            if (styleToApply.OnMouseDownAction != null)
                OnMouseDownAction = new Action<IUIElement>(_ => styleToApply.OnMouseDownAction?.Invoke((T)_));
            if (styleToApply.OnMouseUpAction != null)
                OnMouseUpAction = new Action<IUIElement>(_ => styleToApply.OnMouseUpAction?.Invoke((T)_));
        }

        private Dictionary<Type, IAnimationActuator> _animations = new Dictionary<Type, IAnimationActuator>();
        
        public void Animate(IAnimationActuator animationActuator)
        {
            if (animationActuator == null)
            {
                throw new ArgumentNullException(nameof(animationActuator));
            }

            _animations[animationActuator.GetType()] = animationActuator;
        }

        protected override void OnAnimate()
        {
            List<Type> actuatorsToRemove = null;
            foreach (var animationActuator in _animations)
            {
                if (!animationActuator.Value.Tick(this))
                {
                    actuatorsToRemove ??= new List<Type>();
                    actuatorsToRemove.Add(animationActuator.Key);
                }
                else
                    _stateChanged = true;
            }

            if (actuatorsToRemove != null)
            {
                foreach (var animationTypeToRemove in actuatorsToRemove)
                    _animations.Remove(animationTypeToRemove);
            }

            base.OnAnimate();
        }

        internal override void MergeWith(VisualNode newNode)
        {
            base.MergeWith(newNode);

            if (newNode.GetType() == GetType())
            {
                ((UIElement<T, TS>)newNode)._animations = _animations;
            }
        }

        protected override IEnumerable<VisualNode> RenderChildren()
        {
            yield break;
        }

        public Action<IUIElement> OnMouseEnterAction { get; set; }
        public Action<IUIElement> OnMouseLeaveAction { get; set; }
        public Action<IUIElement> OnMouseDownAction { get; set; }
        public Action<IUIElement> OnMouseUpAction { get; set; }
    }

    public static class UIElementExtensions
    {
        public static T HitTestVisible<T>(this T element, bool isHitTestVisible) where T : class, IUIElement
        {
            element.IsHitTestVisible = isHitTestVisible;
            return element;
        }

        public static T IsEnabled<T>(this T element, bool isEnabled) where T : class, IUIElement
        {
            element.IsEnabled = isEnabled;
            return element;
        }

        public static T IsHitTestVisible<T>(this T element, bool isHitTestVisible) where T : class, IUIElement
        {
            element.IsHitTestVisible = isHitTestVisible;
            return element;
        }

        public static T IsVisible<T>(this T element, bool isVisible) where T : class, IUIElement
        {
            element.IsVisible = isVisible;
            return element;
        }

        public static T Opacity<T>(this T element, double opacity) where T : class, IUIElement
        {
            element.Opacity = opacity;
            return element;
        }

        public static T Transform<T>(this T element, Transform transform) where T : class, IUIElement
        {
            element.Transform = transform;
            return element;
        }

        public static T Style<T, TS>(this T element, TS style) where T : UIElement<T, TS> where TS : UIElementStyle<T>
        {
            element.SetStyle(style);
            return element;
        }

        public static T OnMouseEnter<T>(this T element, Action<T> action) where T : class, IUIElement
        {
            element.OnMouseEnterAction = (_) => action((T)_);
            return element;
        }

        public static T OnMouseLeave<T>(this T element, Action<T> action) where T : class, IUIElement
        {
            element.OnMouseLeaveAction = (_) => action((T)_);
            return element;
        }
    }

    public static class UIElementAnimationExtensions
    {
        public static T AnimateOpacity<T>(this T element, double from, double to, int duration, Func<double, double> easingFunction) where T : class, IUIElement
        {
            element.Animate(new OpacityAnimationActuator(new DoubleAnimation(from, to, duration, easingFunction)));
            return element;
        }

        public static T AnimateOpacity<T>(this T element, double to, int duration, Func<double, double> easingFunction) where T : class, IUIElement
        {
            element.Animate(new OpacityAnimationActuator(new DoubleAnimation(to, duration, easingFunction)));
            return element;
        }
    }
}
