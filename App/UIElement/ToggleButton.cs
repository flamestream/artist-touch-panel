﻿using System;
using System.Windows;
using System.Windows.Controls;
using static Taction.Config;

namespace Taction.UIElement {

	/// <summary>
	/// A button that executes key command only once.
	/// </summary>
	internal class ToggleButton : System.Windows.Controls.Primitives.ToggleButton {

		private App App => (App)Application.Current;
		internal KeyCommand KeyCommand { set; get; }

		public ToggleButton(IPanelItemSpecs specs) {

			var s = (ToggleButtonSpecs)specs;

			 KeyCommand = InputSimulatorHelper.ParseKeyCommand(s.keyCommand);

			// Event binding
			 Checked += HandleChecked;
			 Unchecked += HandleUnchecked;
		}

		protected void HandleChecked(Object sender, RoutedEventArgs e) {

			 FontWeight = FontWeights.Bold;
			App.InputSimulator.SimulateKeyDown(KeyCommand);
		}

		protected void HandleUnchecked(Object sender, RoutedEventArgs e) {

			 FontWeight = FontWeights.Normal;
			App.InputSimulator.SimulateKeyUp(KeyCommand);
		}
	}
}
