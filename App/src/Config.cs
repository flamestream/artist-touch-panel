﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Taction {

	internal partial class Config {

		public List<string> LoadLayoutErrors { get; private set; }
		public ConfigLayout Layout { get; private set; }
		public ConfigState State { get; private set; }

		public ZipArchive LoadedLayoutZip { get; private set; }
		public List<MemoryStream> LoadingImageStreams { get; private set; }
		public List<MemoryStream> LoadedImageStreams { get; private set; }
		public List<string> LoadedFonts { get; private set; }

		public Config() {

			LoadLayoutErrors = new List<string>();
			Layout = new ConfigLayout();
			State = new ConfigState();

			LoadedImageStreams = new List<MemoryStream>();
			LoadingImageStreams = new List<MemoryStream>();
			LoadedFonts = new List<string>();
		}

		public void Save() {

			using (StreamWriter file = File.CreateText(App.FileStatePath)) {

				JsonSerializer serializer = new JsonSerializer {
					Formatting = Formatting.Indented
				};
				serializer.Serialize(file, State);
			}
		}

		public void LoadLayout(string path) {

			if (path == null)
				throw new ArgumentNullException("LoadLayout may not receive null path");

			JObject json;
			LoadedLayoutZip = null;

			var ext = Path.GetExtension(path);
			if (ext == Properties.Resources.ConfigBundleFileExtension) {

				using (var zip = ZipFile.Open(path, ZipArchiveMode.Read)) {

					LoadedFonts.Clear();

					// Layout existence check
					var entry = zip.Entries.FirstOrDefault(e => e.Name == Properties.Resources.ConfigLayoutFileName);
					if (entry == null)
						throw new FileFormatException("Malformed config bundle: No layout found");

					using (var streamReader = new StreamReader(entry.Open()))
					using (var jsonReader = new JsonTextReader(streamReader)) {

						// Make zip file available to converters
						LoadedLayoutZip = zip;

						json = JObject.Load(jsonReader);
						LoadLayout(json);
					}

					// Remove fonts unrelated to layout
					var fontFiles = Directory.GetFiles(App.FontDir);
					var extraFontFiles = fontFiles.Except(LoadedFonts);
					foreach (var f in extraFontFiles) {
						try {
							File.Delete(f);
						} catch (Exception ex) {
							// Silently log error
							App.Instance.LogError(ex.ToString());
						}
					}
				}

				LoadedLayoutZip = null;

			} else if (ext == ".json") {

				using (var reader = File.OpenText(path))
				using (var jsonReader = new JsonTextReader(reader)) {

					json = JObject.Load(jsonReader);
					LoadLayout(json);
				}

			} else {

				throw new FileFormatException(string.Format("Unsupported format {0}", ext));
			}
		}

		public void LoadLayout(JObject json) {

			// Version check
			if (!json.TryGetValue("version", out var versionJson))
				throw new FormatException("Missing layout version number");

			int version;
			try {
				version = versionJson.Value<int>();
			} catch (Exception) {
				throw new FormatException("Invalid layout version number");
			}

			var appMajorVersion = App.Instance.GetVersion().Major;
			if (version != appMajorVersion)
				throw new FormatException(string.Format("Layout is outdated. Expected version {0}, but detected version {1}", appMajorVersion, version));

			// Validation check
			if (!json.IsValid(App.LayoutJsonSchema, out IList<ValidationError> errors)) {

				var errMsgs = new List<string>();
				foreach (var error in errors)
					errMsgs.Add(ParseError(error));

				var errMsg = string.Join(Environment.NewLine, errMsgs);

				throw new FormatException(errMsg);
			}

			// Set up
			LoadingImageStreams.Clear();
			LoadLayoutErrors.Clear();

			// Value check
			var layoutCandidate = JsonConvert.DeserializeObject<ConfigLayout>(JsonConvert.SerializeObject(json));
			if (LoadLayoutErrors.Count > 0) {

				var errMsg = string.Join(Environment.NewLine, LoadLayoutErrors);
				throw new FormatException(errMsg);
			}

			// Populate
			Layout = layoutCandidate;

			// Clean up image streams
			foreach (var stream in LoadedImageStreams) stream.Dispose();
			LoadedImageStreams = new List<MemoryStream>(LoadingImageStreams);

			// Clean up loaded fonts
			//foreach (var font in LoadedFonts) font.Dispose();
		}

		public void LoadState() {

			// Existence check
			// @NOTE Load default one here if there are any
			if (!File.Exists(App.FileStatePath))
				return;

			try {

				JObject json;
				using (var reader = File.OpenText(App.FileStatePath))
				using (var jsonReader = new JsonTextReader(reader)) {

					json = JObject.Load(jsonReader);
				}

				// Populate
				State = JsonConvert.DeserializeObject<ConfigState>(JsonConvert.SerializeObject(json));

			} catch (Exception e) {

				// Load error, but continue with default state
				App.Instance.LogError(string.Format("Failed to load state: {0}\n{1}", e.Message, e.StackTrace));
				ResetState();
			}
		}

		public void ResetState() {

			File.Delete(App.FileStatePath);
		}

		private static string ParseError(ValidationError error) {

			if (error.ChildErrors.Count == 0)
				return string.Format("Line {1}:{2}: {0}", error.Message, error.LineNumber, error.LinePosition);

			var errMsgs = new List<string>();
			foreach (var e in error.ChildErrors)
				errMsgs.Add(ParseError(e));

			return string.Join(Environment.NewLine, errMsgs);
		}
	}
}
