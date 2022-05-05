﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SettingsUI.Controls;

[TemplatePart(Name = KeyPresenter, Type = typeof(ContentPresenter))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Default", GroupName = "StateStates")]
[TemplateVisualState(Name = "Error", GroupName = "StateStates")]
public sealed class KeyVisual : Control
{
    private const string KeyPresenter = "KeyPresenter";
    private KeyVisual _keyVisual;
    private ContentPresenter _keyPresenter;

    public object Content
    {
        get => (object) GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(KeyVisual), new PropertyMetadata(default(string), OnContentChanged));

    public VisualType VisualType
    {
        get => (VisualType) GetValue(VisualTypeProperty);
        set => SetValue(VisualTypeProperty, value);
    }

    public static readonly DependencyProperty VisualTypeProperty = DependencyProperty.Register("VisualType", typeof(VisualType), typeof(KeyVisual), new PropertyMetadata(default(VisualType), OnSizeChanged));

    public bool IsError
    {
        get => (bool) GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register("IsError", typeof(bool), typeof(KeyVisual), new PropertyMetadata(false, OnIsErrorChanged));

    public KeyVisual()
    {
        this.DefaultStyleKey = typeof(KeyVisual);
        this.Style = GetStyleSize("TextKeyVisualStyle");
    }

    protected override void OnApplyTemplate()
    {
        IsEnabledChanged -= KeyVisual_IsEnabledChanged;
        _keyVisual = (KeyVisual) this;
        _keyPresenter = (ContentPresenter) _keyVisual.GetTemplateChild(KeyPresenter);
        Update();
        SetEnabledState();
        SetErrorState();
        IsEnabledChanged += KeyVisual_IsEnabledChanged;
        base.OnApplyTemplate();
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((KeyVisual) d).Update();
    }

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((KeyVisual) d).Update();
    }

    private static void OnIsErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((KeyVisual) d).SetErrorState();
    }

    private void Update()
    {
        if (_keyVisual == null)
        {
            return;
        }

        if (_keyVisual.Content != null)
        {
            if (_keyVisual.Content.GetType() == typeof(string))
            {
                _keyVisual.Style = GetStyleSize("TextKeyVisualStyle");
                _keyVisual._keyPresenter.Content = _keyVisual.Content;
            }
            else
            {
                _keyVisual.Style = GetStyleSize("IconKeyVisualStyle");

                switch ((int) _keyVisual.Content)
                {
                    /* We can enable other glyphs in the future
                    case 13: // The Enter key or button.
                        _keyVisual._keyPresenter.Content = "\uE751"; break;
                    case 8: // The Back key or button.
                        _keyVisual._keyPresenter.Content = "\uE750"; break;
                    case 16: // The right Shift key or button.
                    case 160: // The left Shift key or button.
                    case 161: // The Shift key or button.
                        _keyVisual._keyPresenter.Content = "\uE752"; break; */

                    case 38: _keyVisual._keyPresenter.Content = "\uE0E4"; break; // The Up Arrow key or button.
                    case 40: _keyVisual._keyPresenter.Content = "\uE0E5"; break; // The Down Arrow key or button.
                    case 37: _keyVisual._keyPresenter.Content = "\uE0E2"; break; // The Left Arrow key or button.
                    case 39: _keyVisual._keyPresenter.Content = "\uE0E3"; break; // The Right Arrow key or button.

                    case 91: // The left Windows key
                    case 92: // The right Windows key
                        var winIcon = XamlReader.Load(@"<PathIcon xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" Data=""M9,17V9h8v8ZM0,17V9H8v8ZM9,8V0h8V8ZM0,8V0H8V8Z"" />") as PathIcon;
                        var winIconContainer = new Viewbox();
                        winIconContainer.Child = winIcon;
                        winIconContainer.HorizontalAlignment = HorizontalAlignment.Center;
                        winIconContainer.VerticalAlignment = VerticalAlignment.Center;

                        var iconDimensions = GetIconSize();
                        winIconContainer.Height = iconDimensions;
                        winIconContainer.Width = iconDimensions;
                        _keyVisual._keyPresenter.Content = winIconContainer;
                        break;
                    default: _keyVisual._keyPresenter.Content = ((VirtualKey) _keyVisual.Content).ToString(); break;
                }
            }
        }
    }

    public Style GetStyleSize(string styleName)
    {
        if (VisualType == VisualType.Small)
        {
            return (Style) Application.Current.Resources["Small" + styleName];
        }
        else if (VisualType == VisualType.SmallOutline)
        {
            return (Style) Application.Current.Resources["SmallOutline" + styleName];
        }
        else
        {
            return (Style) Application.Current.Resources["Default" + styleName];
        }
    }

    public double GetIconSize()
    {
        if (VisualType == VisualType.Small || VisualType == VisualType.SmallOutline)
        {
            return (double) Application.Current.Resources["SmallIconSize"];
        }
        else
        {
            return (double) Application.Current.Resources["DefaultIconSize"];
        }
    }

    private void KeyVisual_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        SetEnabledState();
    }

    private void SetErrorState()
    {
        VisualStateManager.GoToState(this, IsError ? "Error" : "Default", true);
    }

    private void SetEnabledState()
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
    }
}

public enum VisualType
{
    Small,
    SmallOutline,
    Large,
}
