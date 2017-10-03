﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Taction {

	partial class Config {

		/// <summary>
		/// Covert JSON string value to enumerator
		/// </summary>
		private class BrushConverter : JsonConverter {

			public override bool CanWrite => false;
			public override bool CanRead => true;

			public override bool CanConvert(Type objectType) {

				return objectType == typeof(Brush);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

				throw new InvalidOperationException("Not supported");
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {

				var o = default(Brush);

				var json = serializer.Deserialize<JObject>(reader);
				var type = json.Value<string>("type");

				switch (type) {

					case "image":

						var stretch = json.Value<string>("stretch");
						var tile = json.Value<string>("tile");

						o = new ImageBrush {
							Stretch = GetStretch(stretch),
							TileMode = GetTileMode(tile)
						};
						break;

					case "solid":

						var value = json.Value<string>("value");

						o = new SolidColorBrush {
							Color = GetColor(value)
						};
						break;

					case "gradient":

						var brush = new LinearGradientBrush {
							EndPoint = new Point(0, 0),
							StartPoint = new Point(0, 1),
						};

						var values = json.Value<List<string>>("values");
						foreach (var v in values) {

							// @TODO compile errors
							if (!TryGetGradientStop(v, out var gs)) {

								continue;
							}

							brush.GradientStops.Add(gs);
						}

						// @TODO compile errors

						o = brush;
						break;
				}

				return o;
			}

			private Stretch GetStretch(string id) {

				var o = Stretch.None;

				switch (id) {
					case "fill":
						o = Stretch.Fill;
						break;

					case "uniform":
						o = Stretch.Uniform;
						break;

					case "uniformFill":
						o = Stretch.UniformToFill;
						break;
				}

				return o;
			}

			private TileMode GetTileMode(string id) {

				var o = TileMode.None;

				switch (id) {
					case "normal":
						o = TileMode.Tile;
						break;

					case "flipX":
						o = TileMode.FlipX;
						break;

					case "flipY":
						o = TileMode.FlipY;
						break;

					case "flipXY":
						o = TileMode.FlipXY;
						break;
				}

				return o;
			}

			private Color GetColor(string id) {

				// var o = default(Color);

				// Named check
				//var candidate = typeof(Colors).GetProperty(id, typeof(Color)).GetValue(null);
				//if (candidate != null)
				//	return (Color)candidate;

				// @TODO Color.Empty?
				return (Color)ColorConverter.ConvertFromString(id);
			}

			private bool TryGetGradientStop(string id, out GradientStop o) {

				o = default(GradientStop);

				// Format check
				var parts = id.Split(';');
				if (parts.Length != 2)
					return false;

				var color = GetColor(parts[0]);

				if (!double.TryParse(parts[1], out var offset))
					return false;

				o = new GradientStop() {
					Color = color,
					Offset = offset
				};

				return true;
			}
		}
	}
}
