﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Taction.Attribute;
using Taction.UIElement;

namespace Taction {

	internal static partial class Designer {

		public static void GenerateLayout(MainPanel window) {

			var config = App.Instance.Config;

			var layoutData = config.Layout;
			var stateData = config.State;

			// Setup window
			window.Opacity = layoutData.Opacity;
			window.Background = layoutData.Color;

			window.BorderThickness = default(Thickness);
			window.BorderBrush = null;
			if (layoutData.Border != null) {

				if (layoutData.Border.Thickness != null)
					window.BorderThickness = layoutData.Border.Thickness.Value;

				if (layoutData.Border.Color != null)
					window.BorderBrush = layoutData.Border.Color;
			}

			// Setup main panel
			var panel = new StackPanel {
				Orientation = config.Layout.Orientation,
			};

			if (layoutData.Margin != null)
				panel.Margin = layoutData.Margin.Value;

			if (layoutData.Orientation == Orientation.Vertical) {
				panel.Width = layoutData.Size;
			} else {
				panel.Height = panel.Height = layoutData.Size;
			}
			window.Container.Children.Add(panel);

			// Compute children items
			ProcessLayout(layoutData, layoutData.Items, panel);

			// Set position
			window.Left = stateData.X;
			window.Top = stateData.Y;
		}

		private static void ProcessLayout(ConfigLayout layout, List<IPanelItemSpecs> specsList, StackPanel panel) {

			if (specsList == null)
				return;

			foreach (var specs in specsList) {

				var specsType = specs.GetType();
				var attr = (AssociatedClassAttribute)specsType.GetCustomAttributes(typeof(AssociatedClassAttribute), true)[0];
				var itemType = attr.Value;

				FrameworkElement item;
				if (itemType == typeof(HoldButton) || itemType == typeof(RadialMenuButton) || itemType == typeof(MoveButton)) {

					// Compute specs
					var computedSpecs = specs as IButtonSpecs;
					var computedStyle = ApplyStyle(layout.DefaultButtonStyle, computedSpecs.Style);
					computedStyle.Size = computedSpecs.Size;
					computedSpecs.Style = computedStyle;
					item = (FrameworkElement)Activator.CreateInstance(itemType, computedSpecs);

				} else {

					item = (FrameworkElement)Activator.CreateInstance(itemType, specs);

					// Set size
					if (panel.Orientation == Orientation.Vertical) {

						item.Height = specs.Size;

					} else {

						item.Width = specs.Size;
					}

					// Set base style
					if (specs is IButtonSpecs)
						ApplyStyle(layout, (ContentControl)item, (IButtonSpecs)specs, panel);
				}

				// Add to tree
				panel.Children.Add(item);

				// Special: Panel handling
				if (item is StackPanel childPanel) {

					childPanel.Orientation = panel.Orientation == Orientation.Horizontal ?
						Orientation.Vertical :
						Orientation.Horizontal;

					// Process Children
					ProcessLayout(layout, ((PivotSpecs)specs).Items, childPanel);
				}
			}
		}

		private static ButtonStyleSetSpecs ApplyStyle(params ButtonStyleSetSpecs[] args) {

			var output = new ButtonStyleSetSpecs();

			foreach (var s in args) {

				ApplyStyle(output.Base, s.Base);
				ApplyStyle(output.Active, s.Active);
				output.Size = s.Size;
			}

			ApplyStyle(output.Active, output.Base, true);

			return output;
		}

		private static void ApplyStyle(ButtonStyleSpecs source, ButtonStyleSpecs apply, bool ignoreIfNotNull = false) {

			if (source.Border.Color == null || !ignoreIfNotNull)
				source.Border.Color = apply.Border.Color ?? source.Border.Color;

			if (source.Border.Radius == null || !ignoreIfNotNull)
				source.Border.Radius = apply.Border.Radius ?? source.Border.Radius;

			if (source.Border.Thickness == null || !ignoreIfNotNull)
				source.Border.Thickness = apply.Border.Thickness ?? source.Border.Thickness;

			if (source.Color == null || !ignoreIfNotNull)
				source.Color = apply.Color ?? source.Color;

			if (source.Content == null || !ignoreIfNotNull)
				source.Content = apply.Content ?? source.Content;

			if (source.Padding == null || !ignoreIfNotNull)
				source.Padding = apply.Padding ?? source.Padding;

			if (source.Margin == null || !ignoreIfNotNull)
				source.Margin = apply.Margin ?? source.Margin;

			if (source.Opacity == null || !ignoreIfNotNull)
				source.Opacity = apply.Opacity ?? source.Opacity;

			if (source.TextStyle.Color == null || !ignoreIfNotNull)
				source.TextStyle.Color = apply.TextStyle.Color ?? source.TextStyle.Color;

			if (source.TextStyle.FontFamily == null || !ignoreIfNotNull)
				source.TextStyle.FontFamily = apply.TextStyle.FontFamily ?? source.TextStyle.FontFamily;

			if (source.TextStyle.FontSize == null || !ignoreIfNotNull)
				source.TextStyle.FontSize = apply.TextStyle.FontSize ?? source.TextStyle.FontSize;

			if (source.TextStyle.FontWeight == null || !ignoreIfNotNull)
				source.TextStyle.FontWeight = apply.TextStyle.FontWeight ?? source.TextStyle.FontWeight;
		}

		private static void ApplyBaseStyle(ContentControl item, ButtonStyleSetSpecs style) {

			if (style == null)
				return;

			var baseStyle = style.Base;
			if (baseStyle == null)
				return;

			if (baseStyle.Color != null)
				item.Background = baseStyle.Color;

			if (baseStyle.Content != null)
				item.Content = baseStyle.Content;

			if (baseStyle.Margin != null)
				item.Margin = baseStyle.Margin.Value;

			if (baseStyle.Padding != null)
				item.Padding = baseStyle.Padding.Value;

			if (baseStyle.Border != null) {

				if (baseStyle.Border.Color != null)
					item.BorderBrush = baseStyle.Border.Color;

				if (baseStyle.Border.Thickness != null)
					item.BorderThickness = baseStyle.Border.Thickness.Value;
			}

			if (baseStyle.TextStyle != null) {

				if (baseStyle.TextStyle.Color != null)
					item.Foreground = baseStyle.TextStyle.Color;

				if (baseStyle.TextStyle.FontFamily != null)
					item.FontFamily = baseStyle.TextStyle.FontFamily;

				if (baseStyle.TextStyle.FontSize != null)
					item.FontSize = baseStyle.TextStyle.FontSize.Value;

				if (baseStyle.TextStyle.FontWeight != null)
					item.FontWeight = baseStyle.TextStyle.FontWeight.Value;
			}
		}

		private static void ApplyActiveStyle(ICustomStylizable item, ButtonStyleSetSpecs style) {

			if (style == null)
				return;

			var activeStyle = style.Active;
			if (activeStyle == null)
				return;

			if (activeStyle.Color != null)
				item.Active_Background = activeStyle.Color;

			if (activeStyle.Content != null)
				item.Active_Content = activeStyle.Content;

			if (activeStyle.Margin != null)
				item.Active_Margin = activeStyle.Margin.Value;

			if (activeStyle.Border != null) {

				if (activeStyle.Border.Color != null)
					item.Active_BorderBrush = activeStyle.Border.Color;

				if (activeStyle.Border.Thickness != null)
					item.Active_BorderThickness = activeStyle.Border.Thickness.Value;
			}

			if (activeStyle.TextStyle != null) {

				if (activeStyle.TextStyle.Color != null)
					item.Active_Foreground = activeStyle.TextStyle.Color;

				if (activeStyle.TextStyle.FontFamily != null)
					item.Active_FontFamily = activeStyle.TextStyle.FontFamily;

				if (activeStyle.TextStyle.FontSize != null)
					item.Active_FontSize = activeStyle.TextStyle.FontSize.Value;

				if (activeStyle.TextStyle.FontWeight != null)
					item.Active_FontWeight = activeStyle.TextStyle.FontWeight.Value;
			}
		}

		private static void ApplyStyle(ConfigLayout layout, ContentControl item, IButtonSpecs specs, StackPanel panel) {

			// Set base style
			ApplyBaseStyle(item, layout.DefaultButtonStyle);
			ApplyBaseStyle(item, specs.Style);

			if (item is ICustomStylizable i) {

				// Copy base
				i.Active_Background = item.Background;
				i.Active_BorderBrush = item.BorderBrush;
				i.Active_BorderThickness = item.BorderThickness;
				i.Active_Content = item.Content;
				i.Active_FontFamily = item.FontFamily;
				i.Active_FontSize = item.FontSize;
				i.Active_Foreground = item.Foreground;
				i.Active_Margin = item.Margin;

				// Set active style
				ApplyActiveStyle(i, layout.DefaultButtonStyle);
				ApplyActiveStyle(i, specs.Style);
			}
		}

		public static void AdjustGradientColor(ContentControl item, Brush brush) {

			// Type heck
			if (!(brush is LinearGradientBrush))
				return;

			var gradientBrush = (LinearGradientBrush)brush;

			// Angle check
			if (Point.Subtract(gradientBrush.StartPoint, gradientBrush.EndPoint).Length == 1)
				return;

			var ratio = item.ActualWidth / item.ActualHeight;

			var displacement = (ratio - 1) * 0.5;

			gradientBrush.StartPoint = new Point(gradientBrush.StartPoint.X, ratio * gradientBrush.StartPoint.Y - displacement);
			gradientBrush.EndPoint = new Point(gradientBrush.EndPoint.X, ratio * gradientBrush.EndPoint.Y - displacement);
		}
	}
}
