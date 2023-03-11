using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ITVisions.RegEx
{

 /// <summary>
 /// aus ITVAppUtil 12.2.2019
 /// </summary>
 public class RegExUtil
 {

  public static bool IsEMail(string email)
  {
   const string MAILREGEX = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
   // Regulärer Ausdruck
   Regex re = new Regex(MAILREGEX);
   return re.Match(email).Success;
  }

  public static string MaskField(string str, string field, string mask = "***")
  {
   if (String.IsNullOrEmpty(str)) return str;
   var separators = ",;";
   var sb = new StringBuilder();

   foreach (var keyValue in Regex.Split(str, $"(?<=[{separators}])"))
   {
    var temp = keyValue;
    var index = keyValue.IndexOf("=");
    if (index > 0)
    {
     var key = keyValue.Substring(0, index);
     if (string.Compare(key.Trim(), field.Trim(), true) == 0)
     {
      var end = separators.Contains(keyValue.Last()) ? keyValue.Last().ToString() : "";
      temp = key + "=" + mask + end;
     }
    }
    sb.Append(temp);
   }

   return sb.ToString();
  }

  public static string ReplacePasswordInConnectionString(string cs)
  {
   cs = MaskField(cs, "PWD");
   cs = MaskField(cs, "PASSWORD");
   cs = MaskField(cs, "InstrumentationKey");
   return cs;
  }
 }
}
