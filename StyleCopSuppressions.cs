// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1062", Justification = "User responsibility.")]

[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1000", Justification = "new()")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009", Justification = "Preference.")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010", Justification = "Erroneous.")] // var array = []
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011", Justification = "Preference.")]

[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1127", Justification = "Preference.")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1128", Justification = "Preference.")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1134", Justification = "Reviewed.")]

[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1308", Justification = "Coding preferences.")] // Blame source.dot.net for this.
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309", Justification = "Coding preferences.")] // Blame source.dot.net for this.
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311", Justification = "Coding preferences.")]

[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502", Justification = "Reviewed.")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503", Justification = "Reviewed.")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513", Justification = "Reviewed.")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1520", Justification = "Reviewed.")]

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633", Justification = "LICENSE file included.")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1636", Justification = "LICENSE file included.")]

// SonarLint
[assembly: SuppressMessage("Major Code Smell", "S3358", Justification = "Reviewed.")]