﻿using System;
using System.Collections.Generic;
using System.Linq;
using Taction.Attribute;

namespace Taction {

	public partial class App {

		#region -- Application Settings/Constants --

		internal const int NotificationToastCloseDelayTime = 5000;
		internal const int NotificationToastSecondaryCloseDelayTime = 500;
		internal const int MaxErrorLogSize = 2 * 1024;
		internal const int ErrorLogTrimLineCount = 500;

		#endregion -- Application Settings/Constants --

		private static string _appDataDir;
		private static string _errorFilePath;
		private static string _fileLayoutPath;
		private static string _fileBundleName;
		private static string _fileBundlePath;
		private static string _fileStatePath;
		private static Dictionary<string, Type> _stringVsPanelItemSpecs;

		public static App Instance => (App)Current;

		public static string AppDataDir {
			get {
				if (_appDataDir == null) {

					_appDataDir = string.Format(@"{0}\{1}",
						Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
						Taction.Properties.Resources.AppName
					);
				}

				return _appDataDir;
			}
		}

		public static string ErrorFilePath {
			get {
				if (_errorFilePath == null) {

					_errorFilePath = string.Format(@"{0}\{1}",
						AppDataDir,
						Taction.Properties.Resources.ConfigErrorFileName
					);
				}

				return _errorFilePath;
			}
		}

		public static string FileLayoutPath {
			get {
				if (_fileLayoutPath == null) {

					_fileLayoutPath = string.Format(@"{0}\{1}",
						AppDataDir,
						Taction.Properties.Resources.ConfigLayoutFileName
					);
				}

				return _fileLayoutPath;
			}
		}

		public static string FileBundlePath {
			get {
				if (_fileBundlePath == null) {

					_fileBundlePath = string.Format(@"{0}\{1}",
						AppDataDir,
						FileBundleName
					);
				}

				return _fileBundlePath;
			}
		}

		public static string FileBundleName {
			get {
				if (_fileBundleName == null) {

					_fileBundleName = Taction.Properties.Resources.ConfigBundleFileBaseName
						+ Taction.Properties.Resources.ConfigBundleFileExtension;
				}

				return _fileBundleName;
			}
		}

		public static Dictionary<string, Type> StringVsPanelItemSpecs {
			get {
				if (_stringVsPanelItemSpecs == null) {

					_stringVsPanelItemSpecs = new Dictionary<string, Type>();

					// List implemented PanelItemSpecs
					var specsTypes = AppDomain.CurrentDomain.GetAssemblies()
						.SelectMany(s => s.GetTypes())
						.Where(p => p.IsClass && typeof(IPanelItemSpecs).IsAssignableFrom(p));

					// Create associations
					foreach (var specsType in specsTypes) {

						var typeAttrs = specsType.GetCustomAttributes(typeof(JsonStringTypeValueAttribute), true);
						if (typeAttrs.Length == 0)
							continue;

						var typeStr = ((JsonStringTypeValueAttribute)typeAttrs[0]).Value;
						_stringVsPanelItemSpecs.Add(typeStr, specsType);
					}
				}

				// Return copy
				return new Dictionary<string, Type>(_stringVsPanelItemSpecs);
			}
		}

		public static string FileStatePath {
			get {
				if (_fileStatePath == null) {

					_fileStatePath = string.Format(@"{0}\{1}",
						AppDataDir,
						Taction.Properties.Resources.ConfigStateFileName
					);
				}

				return _fileStatePath;
			}
		}
	}
}