using System;
using System.ComponentModel.DataAnnotations;
using ITVisions;

namespace Samples.TippsUndTricks.MetadataUI;

/// <summary>
/// Anrede einer Person
/// </summary>
public enum AnredeEnum
{
 [Display(Name = "Männlich")]
 Maennlich,

 [Display(Name = "Weiblich")]
 Weiblich,

 [Display(Name = "Divers")]
 Divers
}

/// <summary>
/// Akademischer Titel
/// </summary>
public enum TitelEnum
{
 [Display(Name = "")]
 Kein,

 [Display(Name = "Dr.")]
 Dr,

 [Display(Name = "Prof.")]
 Prof
}

/// <summary>
/// Familienstand
/// </summary>
public enum FamilienstandEnum
{
 [Display(Name = "ledig")]
 Ledig,

 [Display(Name = "verheiratet")]
 Verheiratet,

 [Display(Name = "geschieden")]
 Geschieden,

 [Display(Name = "verwitwet")]
 Verwitwet
}

/// <summary>
/// Hobbys zur Auswahl
/// </summary>
[Flags]
public enum HobbyEnum
{
 [Label("Fußball")]
 Fussball = 1,

 [Label("MTB")]
 MTB = 2,

 [Label("Tennis")]
 Tennis = 4,

 [Label("Schwimmen")]
 Schwimmen = 8,

 [Label("Lesen")]
 Lesen = 16,

 [Label("Reisen")]
 Reisen = 32
}


/// <summary>
/// Liste der Sprachen von PYPL PopularitY of Programming Language
/// https://pypl.github.io/PYPL.html
/// </summary>
public enum Programmiersprache
{
 Python,
 CCpp,
 Java,
 R,
 JavaScript,
 Rust,
 Swift,
 [Label("C#")]
 CSharp,
 ObjectiveC,
 PHP,
 Ada,
 TypeScript,
 Matlab,
 PowerShell,
 Ruby,
 Dart,
 Kotlin,
 Lua,
 VBA,
 Go,
 Julia,
 Scala,
 Abap,
 Zig,
 DelphiPascal,
 VisualBasic,
 Perl,
 Haskell,
 Cobol,
 Groovy
}
