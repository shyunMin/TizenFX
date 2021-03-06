/*
 * Copyright(c) 2020 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
extern alias TizenSystemInformation;
using TizenSystemInformation.Tizen.System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Tizen.NUI.Xaml;
using Tizen.NUI.BaseComponents;

namespace Tizen.NUI
{
    /// <summary></summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ThemeManager
    {
        private enum Profile
        {
            Common = 0,
            Mobile = 1,
            TV = 2,
            Wearable = 3
        }

        private static readonly string[] nuiThemeProjects =
        {
            "Tizen.NUI",
            "Tizen.NUI.Components",
            "Tizen.NUI.Wearable"
        };

        /// <summary>
        /// Table that indicates default theme id by device profile.
        /// Note that, the fallback of null value is Common value.
        /// </summary>
        private static readonly string[] profileDefaultTheme =
        {
            /* Common   */ "Tizen.NUI.Theme.Common",
            /* Mobile   */ "Tizen.NUI.Theme.Common",
            /* TV       */ null,
            /* Wearable */ "Tizen.NUI.Theme.Wearable",
        };

        private static Theme currentTheme;
        private static Theme defaultTheme;
        private static bool isLoadingDefault = false;
        private static Profile? currentProfile;
        private static List<Theme> builtinThemes = new List<Theme>(); // Themes provided by framework.
        internal static List<Theme> customThemes = new List<Theme>(); // Themes registered by user.

        static ThemeManager() {}

        /// <summary>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        /// <summary>
        /// Internal one should be called before calling public ThemeChanged
        /// </summary>
        internal static event EventHandler<ThemeChangedEventArgs> ThemeChangedInternal;

        internal static Theme CurrentTheme
        {
            get
            {
                if (currentTheme == null)
                {
                    currentTheme = DefaultTheme;
                }
                return currentTheme;
            }
            set
            {
                currentTheme = value;
                NotifyThemeChanged();
            }
        }

        internal static Theme DefaultTheme
        {
            get
            {
                if (defaultTheme == null && !isLoadingDefault)
                {
                    isLoadingDefault = true;
                    defaultTheme = LoadBuiltinTheme(profileDefaultTheme[(int)CurrentProfile]);
                    isLoadingDefault = false;
                }
                return defaultTheme;
            }
            set => defaultTheme = (Theme)value?.Clone();
        }

        internal static bool ThemeApplied => (CurrentTheme.Count > 0 || DefaultTheme.Count > 0);

        private static Profile CurrentProfile
        {
            get
            {
                if (currentProfile == null)
                {
                    currentProfile = Profile.Common;
                    string profileString = "";

                    try
                    {
                        Information.TryGetValue<string>("tizen.org/feature/profile", out profileString);
                        Tizen.Log.Info("NUI", "Profile for initial theme found : " + profileString);
                    }
                    catch
                    {
                        Tizen.Log.Info("NUI", "Unknown device profile\n");
                    }
                    finally
                    {
                        if (string.Equals(profileString, "mobile"))
                        {
                            currentProfile = Profile.Mobile;
                        }
                        else if (string.Equals(profileString, "tv"))
                        {
                            currentProfile = Profile.TV;
                        }
                        else if (string.Equals(profileString, "wearable"))
                        {
                            currentProfile = Profile.Wearable;
                        }
                    }
                }
                return (Profile)currentProfile;
            }
        }

        /// <summary>
        /// Set a theme to be used as fallback.
        /// The fallback theme is set to profile specified theme by default.
        /// </summary>
        /// <param name="fallbackTheme">The theme instance to be applied as a fallback.</param>
        /// <exception cref="ArgumentNullException">The given theme is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ApplyFallbackTheme(Theme fallbackTheme)
        {
            DefaultTheme = fallbackTheme ?? throw new ArgumentNullException("Invalid theme.");
        }

        /// <summary>
        /// Apply theme to the NUI.
        /// This will change the appreance of the existing components with property <seealso cref="View.ThemeChangeSensitive"/> on.
        /// This also affects all components created afterwards.
        /// </summary>
        /// <param name="theme">The theme instance to be applied.</param>
        /// <exception cref="ArgumentNullException">Thrown when the given theme is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ApplyTheme(Theme theme)
        {
            var newTheme = (Theme)theme?.Clone() ?? throw new ArgumentNullException("Invalid theme.");

            if (string.IsNullOrEmpty(newTheme.Id))
            {
                newTheme.Id = "NONAME";
            }

            CurrentTheme = newTheme;
        }

        /// <summary>
        /// Note that this API is to support legacy Tizen.NUI.Components.StyleManager.
        /// Please use <seealso cref="ApplyTheme(Theme)"/> instead.
        ///
        /// Apply theme to the NUI using theme id.
        /// The id of theme should be either a registered custom theme or a built-in theme.
        /// You can register custom theme using <seealso cref="RegisterTheme(Theme)"/>.
        /// This will change the appreance of the existing components with property <seealso cref="View.ThemeChangeSensitive"/> on.
        /// This also affects all components created afterwards.
        /// </summary>
        /// <param name="themeId">The theme Id.</param>
        /// <exception cref="ArgumentNullException">Thrown when the given themeId is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ApplyTheme(string themeId)
        {
            if (themeId == null) throw new ArgumentNullException("Invalid themeId");

            int index = customThemes.FindIndex(x => x.Id.Equals(themeId, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                CurrentTheme = customThemes[index];
                return;
            }
            
            index = builtinThemes.FindIndex(x => string.Equals(x.Id, themeId, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                CurrentTheme = builtinThemes[index];
            }
            else
            {
                Tizen.Log.Info("NUI", $"No Theme found with given id : {themeId}");
            }
        }

        /// <summary>
        /// Note that this API is to support legacy Tizen.NUI.Components.StyleManager.
        ///
        /// Register a custom theme that can be used as an id when calling <seealso cref="ApplyTheme(string)"/>.
        /// </summary>
        /// <param name="theme">The theme instance.</param>
        /// <exception cref="ArgumentException">Thrown when the given theme is null or invalid.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RegisterTheme(Theme theme)
        {
            if (theme == null || string.IsNullOrEmpty(theme.Id)) throw new ArgumentException("Invalid theme.");

            int index = customThemes.FindIndex(x => x.Id.Equals(theme.Id, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                customThemes[index] = (Theme)theme.Clone();
            }
            else
            {
                customThemes.Add((Theme)theme.Clone());
            }
        }

        /// <summary>
        /// Load a style with style name in the current theme.
        /// For components, the style name is a component name (e.g. Button) in normal case.
        /// </summary>
        /// <param name="styleName">The style name.</param>
        /// <exception cref="ArgumentNullException">Thrown when the given styleName is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ViewStyle GetStyle(string styleName)
        {
            if (styleName == null) throw new ArgumentNullException("Invalid style name");

            if (!ThemeApplied) return null;

            return (CurrentTheme.GetStyle(styleName) ?? DefaultTheme.GetStyle(styleName))?.Clone();
        }

        /// <summary>
        /// Load a style with View type in the current theme.
        /// </summary>
        /// <param name="viewType">The type of View.</param>
        /// <exception cref="ArgumentNullException">Thrown when the given viewType is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ViewStyle GetStyle(Type viewType)
        {
            if (viewType == null) throw new ArgumentNullException("Invalid viewType");

            if (!ThemeApplied) return null;

            return (CurrentTheme.GetStyle(viewType) ?? DefaultTheme.GetStyle(viewType))?.Clone();
        }

        /// <summary>
        /// Get a cloned built-in theme.
        /// </summary>
        /// <param name="themeId">The built-in theme id.</param>
        /// <exception cref="ArgumentNullException">Thrown when the given themeId is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Theme GetBuiltinTheme(string themeId)
        {
            if (themeId == null) throw new ArgumentNullException("Invalid themeId");

            Theme result = null;
            int index = builtinThemes.FindIndex(x => string.Equals(x.Id, themeId, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                result = builtinThemes[index];
            }
            else
            {
                var theme = LoadBuiltinTheme(themeId);
                builtinThemes.Add(theme);
                result = theme;
            }
            return (Theme)result?.Clone();
        }

        private static Theme LoadBuiltinTheme(string id)
        {
            var loaded = new Theme()
            {
                Id = id,
            };

            if (string.IsNullOrEmpty(id)) return loaded;

            foreach (var project in nuiThemeProjects)
            {
                string path = StyleManager.FrameworkResourcePath + "/Theme/" + project + "_" + id + ".xaml";

                try
                {
                    loaded.Merge(path);
                    loaded.Id = id;
                    Tizen.Log.Info("NUI", $"Done to load \"{path}\".\n");
                }
                catch (Exception e)
                {
                    Tizen.Log.Debug("NUI", $"Could not load \"{path}\"\n");
                    Tizen.Log.Debug("NUI", "Message: " + e + "\n");
                }
            }

            return loaded;
        }

        private static void NotifyThemeChanged()
        {
            ThemeChangedInternal?.Invoke(null, new ThemeChangedEventArgs(CurrentTheme?.Id));
            ThemeChanged?.Invoke(null, new ThemeChangedEventArgs(CurrentTheme?.Id));
        }
    }
}
