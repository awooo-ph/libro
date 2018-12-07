using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Libro.Dialogs;

namespace Libro
{
    class TextBoxHelper : DependencyObject
    {
        public static readonly DependencyProperty SelectAllProperty = DependencyProperty.RegisterAttached(
            "SelectAll", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(default(bool), OnSelectAllChanged));

        public static void SetSelectAll(DependencyObject element, bool value)
        {
            element.SetValue(SelectAllProperty, value);
        }

        public static bool GetSelectAll(DependencyObject element)
        {
            return (bool) element.GetValue(SelectAllProperty);
        }

        private static void OnSelectAllChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var tb = dependencyObject as TextBox;
            if (tb == null) return;
            if (GetSelectAll(tb))
            {
                tb.Focus();
                tb.SelectAll();
            }
        }
        
        public static readonly DependencyProperty FocusOnLoadProperty = DependencyProperty.RegisterAttached(
            "FocusOnLoad", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(default(bool), OnFocusLoadChanged));

        private static void OnFocusLoadChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var context = SynchronizationContext.Current;
            var tb = dependencyObject as TextBox;
            if (tb == null) return;
            tb.IsVisibleChanged += (sender, args) =>
            {
                if ((bool) args.NewValue)
                {
                    Task.Delay(147).ContinueWith(d =>
                    {
                        context.Post(dd=>tb.Focus(),null);
                        context.Post(dd => tb.Text = "PEPE",null);
                        context.Post(dd => tb.Text = "",null);
                    });
                }
            };
        }

        public static void SetFocusOnLoad(DependencyObject element, bool value)
        {
            element.SetValue(FocusOnLoadProperty, value);
        }

        public static bool GetFocusOnLoad(DependencyObject element)
        {
            return (bool) element.GetValue(FocusOnLoadProperty);
        }

        public static readonly DependencyProperty FocusOnHiddenProperty = DependencyProperty.RegisterAttached(
            "FocusOnHidden", typeof(FrameworkElement), typeof(TextBoxHelper), new PropertyMetadata(default(FrameworkElement), OnFocusHiddenChanged));

        private static void OnFocusHiddenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var tb = dependencyObject as TextBox;
            if (tb == null) return;
            var el = GetFocusOnHidden(tb);
            el.IsVisibleChanged += (s, e) =>
            {
                if (!(bool) e.NewValue)
                {
                    tb.SelectAll();
                    tb.Focus();
                }
            };
        }
        

        public static void SetFocusOnHidden(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(FocusOnHiddenProperty, value);
        }

        public static FrameworkElement GetFocusOnHidden(DependencyObject element)
        {
            return (FrameworkElement) element.GetValue(FocusOnHiddenProperty);
        }

        public static readonly DependencyProperty OnInputCommandProperty = DependencyProperty.RegisterAttached(
            "OnInputCommand", typeof(ICommand), typeof(TextBoxHelper), new PropertyMetadata(default(ICommand), OnInputChanged));

        private static void OnInputChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var cmd = GetOnInputCommand(dependencyObject);
            if(cmd == null)
                return;
            var tb = dependencyObject as TextBox;
            if(tb == null)
                return;
            tb.TextChanged += TbOnTextChanged;
        }

        private static void TbOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var tb = (TextBox) sender;
            var cmd = GetOnInputCommand(tb);
            if(cmd == null) return;
            var param = GetInputCommandParameter(tb);
            if(cmd.CanExecute(param))
                cmd.Execute(param);
        }

        public static void SetOnInputCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(OnInputCommandProperty, value);
        }

        public static ICommand GetOnInputCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(OnInputCommandProperty);
        }

        public static readonly DependencyProperty InputCommandParameterProperty = DependencyProperty.RegisterAttached(
            "InputCommandParameter", typeof(object), typeof(TextBoxHelper), new PropertyMetadata(default(object), OnInputChanged));

        public static void SetInputCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(InputCommandParameterProperty, value);
        }

        public static object GetInputCommandParameter(DependencyObject element)
        {
            return element.GetValue(InputCommandParameterProperty);
        }

        public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.RegisterAttached(
            "EnterCommand", typeof(ICommand), typeof(TextBoxHelper), new PropertyMetadata(default(ICommand), OnEnterCommandChanged));

        private static void OnEnterCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var cmd = GetEnterCommand(dependencyObject);
            if (cmd == null) return;
            var tb = dependencyObject as TextBox;
            if (tb == null) return;
            tb.PreviewKeyDown += (sender, args) =>
            {
                if (args.Key != Key.Enter) return;

                if (!string.IsNullOrEmpty(tb.Text))
                {
                    cmd.Execute(tb.Text);
                    tb.Focus();
                    tb.SelectAll();
                    
                }
                args.Handled = true;
            };
        }

        public static readonly DependencyProperty EscapeCommandProperty = DependencyProperty.RegisterAttached(
            "EscapeCommand", typeof(ICommand), typeof(TextBoxHelper), new PropertyMetadata(default(ICommand), OnEscapeCommandChanged));

        private static void OnEscapeCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var el = dependencyObject as FrameworkElement;
            if(el == null) return;
            el.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    var tb = el as TextBox;
                    if (tb != null)
                        if (tb.Text.Length > 0)
                            tb.Text = "";
                        else
                            ExecuteEscapeCommand(el);
                    else
                        ExecuteEscapeCommand(el);
                    e.Handled = true;
                }
                
            };
        }

        private static void ExecuteEscapeCommand(DependencyObject dep)
        {
            var cmd = GetEscapeCommand(dep);
            if(cmd == null)
                return;
            if(cmd.CanExecute(null))
                cmd.Execute(null);
        }

        public static void SetEscapeCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(EscapeCommandProperty, value);
        }

        public static ICommand GetEscapeCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(EscapeCommandProperty);
        }

        public static readonly DependencyProperty HideCaretProperty = DependencyProperty.RegisterAttached(
            "HideCaret", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(default(bool), OnHideCaretChanged));

        private static void OnHideCaretChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var tb = dependencyObject as TextBox;
            if (tb == null) return;
            tb.CaretBrush = Brushes.Transparent;
            tb.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Left)
                {
                    if(tb.Text.Length>0)
                        tb.Text = tb.Text.Substring(0, tb.Text.Length - 1);
                    e.Handled = true;
                    tb.SelectionStart = tb.Text.Length;
                } else if (e.Key == Key.Home)
                {
                    tb.SelectionStart = tb.Text.Length;
                    e.Handled = true;
                }

                Debug.Print(e.Key+" "+e.SystemKey);

            };
        }
        
        public static void SetHideCaret(DependencyObject element, bool value)
        {
            element.SetValue(HideCaretProperty, value);
        }

        public static bool GetHideCaret(DependencyObject element)
        {
            return (bool) element.GetValue(HideCaretProperty);
        }

        public static void SetEnterCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(EnterCommandProperty, value);
        }

        public static ICommand GetEnterCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(EnterCommandProperty);
        }
    }
}
