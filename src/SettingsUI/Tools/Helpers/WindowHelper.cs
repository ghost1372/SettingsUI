﻿using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace SettingsUI.Helpers;

public static class WindowHelper
{
    /// <summary>
    /// allow the app to find the Window that contains an
    /// arbitrary UIElement (GetWindowForElement).  To do this, we keep track
    /// of all active Windows.  The app code must call WindowHelper.CreateWindow
    /// rather than "new Window" so we can keep track of all the relevant windows.
    /// </summary>
    public static List<Window> ActiveWindows { get { return _activeWindows; } }

    private static List<Window> _activeWindows = new List<Window>();

    /// <summary>
    /// Get AppWindow For a Window
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static AppWindow GetAppWindowForCurrentWindow(object target)
    {
        return AppWindow.GetFromWindowId(GetWindowIdFromCurrentWindow(target));
    }

    /// <summary>
    /// Get WindowHandle for a Window
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static IntPtr GetWindowHandleForCurrentWindow(object target)
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(target);
        return hWnd;
    }

    /// <summary>
    /// Get WindowId from Window
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static WindowId GetWindowIdFromCurrentWindow(object target)
    {
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(GetWindowHandleForCurrentWindow(target));
        return wndId;
    }

    /// <summary>
    /// Set Window Width and Height
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public static void SetWindowSize(IntPtr hwnd, int width, int height)
    {
        // Win32 uses pixels and WinUI 3 uses effective pixels, so you should apply the DPI scale factor
        var dpi = NativeMethods.GetDpiForWindow(hwnd);
        float scalingFactor = (float)dpi / 96;
        width = (int)(width * scalingFactor);
        height = (int)(height * scalingFactor);

        NativeMethods.SetWindowPos(hwnd, NativeMethods.HWND_TOP, 0, 0, width, height, NativeMethods.SetWindowPosFlags.SWP_NOMOVE);
    }

    /// <summary>
    /// allow the app to find the Window that contains an
    /// arbitrary UIElement (GetWindowForElement).  To do this, we keep track
    /// of all active Windows.  The app code must call WindowHelper.CreateWindow
    /// rather than "new Window" so we can keep track of all the relevant windows.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Window GetWindowForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (Window window in _activeWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return window;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Create a new Window
    /// </summary>
    /// <returns></returns>
    public static Window CreateWindow()
    {
        Window newWindow = new Window();
        TrackWindow(newWindow);
        return newWindow;
    }

    /// <summary>
    /// track of all active Windows.  The app code must call WindowHelper.CreateWindow
    /// rather than "new Window" so we can keep track of all the relevant windows.
    /// </summary>
    /// <param name="window"></param>
    public static void TrackWindow(Window window)
    {
        window.Closed += (sender, args) => {
            _activeWindows.Remove(window);
        };
        _activeWindows.Add(window);
    }
}