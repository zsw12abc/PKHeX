using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PKHeX.Reflection
{
    public static partial class ReflectUtil
    {
        #region General Reflection
        internal static bool GetValueEquals(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var v = pi.GetValue(obj, null);
            var c = ConvertValue(value, pi.PropertyType);
            return v.Equals(c);
        }
        internal static void SetValue(object obj, string propertyName, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            pi.SetValue(obj, ConvertValue(value, pi.PropertyType), null);            
        }
        internal static object GetValue(object obj, string propertyName)
        {
            PropertyInfo pi = obj.GetType().GetProperty(propertyName);
            return pi.GetValue(obj, null);
        }
        internal static IEnumerable<string> getPropertiesStartWithPrefix(Type type, string prefix)
        {
            return type.GetProperties()
                .Where(p => p.Name.StartsWith(prefix))
                .Select(p => p.Name);
        }
        internal static IEnumerable<string> getPropertiesCanWritePublic(Type type)
        {
            return type.GetProperties().Where(p => p.CanWrite && p.GetSetMethod(nonPublic: true).IsPublic).Select(p => p.Name);
        }
        internal static bool HasProperty(this Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) != null;
        }
        internal static bool HasPropertyAll(this Type type, string name)
        {
            return type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        private static object ConvertValue(object value, Type type)
        {
            if (type == typeof(DateTime?)) // Used for PKM.MetDate and other similar properties
            {
                DateTime dateValue;
                return DateTime.TryParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue) 
                    ? new DateTime?(dateValue) 
                    : null;
            }

            // Convert.ChangeType is suitable for most things
            return Convert.ChangeType(value, type);
        }
        internal static bool? getBooleanState(object obj, string prop)
        {
            return obj.GetType().HasProperty(prop) ? GetValue(obj, prop) as bool? : null;
        }
        #endregion

        #region Batch Edit and Filters
        private const string CONST_DATEFORMAT = "yyyyMMdd";
        private const string CONST_RAND = "$rand";
        private const string CONST_SHINY = "$shiny";

        private static void screenStrings(IEnumerable<BatchEditorStringInstruction> il)
        {
            GameInfo.GameStrings strings;
            if (Main.GameStrings == null)
            {
                strings = GameInfo.GameStrings.CreateFromCurrentCulture();
            }
            else
            {
                strings = Main.GameStrings;
            }

            foreach (var i in il.Where(i => !i.PropertyValue.All(char.IsDigit)))
            {
                switch (i.PropertyName)
                {
                    case nameof(PKM.Species): i.setScreenedValue(strings.specieslist); continue;
                    case nameof(PKM.HeldItem): i.setScreenedValue(strings.itemlist); continue;
                    case nameof(PKM.Move1): case nameof(PKM.Move2): case nameof(PKM.Move3): case nameof(PKM.Move4): i.setScreenedValue(strings.movelist); continue;
                    case nameof(PKM.RelearnMove1): case nameof(PKM.RelearnMove2): case nameof(PKM.RelearnMove3): case nameof(PKM.RelearnMove4): i.setScreenedValue(strings.movelist); continue;
                    case nameof(PKM.Ability): i.setScreenedValue(strings.abilitylist); continue;
                    case nameof(PKM.Nature): i.setScreenedValue(strings.natures); continue;
                    case nameof(PKM.Ball): i.setScreenedValue(strings.balllist); continue;
                }
            }
        }

        public static IEnumerable<BatchEditorStringInstruction> getFilters(IEnumerable<string> lines)
        {
            var raw = lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Where(line => new[] { '!', '=' }.Contains(line[0]));

            var filters = (from line in raw
                          let eval = line[0] == '='
                          let split = line.Substring(1).Split('=')
                          where split.Length == 2 && !string.IsNullOrWhiteSpace(split[0])
                          select new BatchEditorStringInstruction { PropertyName = split[0], PropertyValue = split[1], Evaluator = eval }).ToList();
            screenStrings(filters);
            return filters;
        }
        public static IEnumerable<BatchEditorStringInstruction> getFilters(string script)
        {
            return getFilters(script.Split("\n".ToCharArray()).Select(line => line.Trim()));
        }
        public static IEnumerable<BatchEditorStringInstruction> getInstructions(IEnumerable<string> lines)
        {
            var raw = lines
                    .Where(line => !string.IsNullOrEmpty(line))
                    .Where(line => new[] { '.' }.Contains(line[0]))
                    .Select(line => line.Substring(1));

            var instructions = (from line in raw
                               select line.Split('=') into split
                               where split.Length == 2
                               select new BatchEditorStringInstruction { PropertyName = split[0], PropertyValue = split[1] }).ToList();
            screenStrings(instructions);
            return instructions;
        }

        public static IEnumerable<BatchEditorStringInstruction> getInstructions(string script)
        {
            return getInstructions(script.Split("\n".ToCharArray()).Select(line => line.Trim()));
        }

        public static BatchEditorModifyResult ProcessPKM(PKM PKM, IEnumerable<BatchEditorStringInstruction> Filters, IEnumerable<BatchEditorStringInstruction> Instructions)
        {
            if (!PKM.ChecksumValid || PKM.Species == 0)
                return BatchEditorModifyResult.Invalid;

            Type pkm = PKM.GetType();

            foreach (var cmd in Filters)
            {
                try
                {
                    if (!pkm.HasProperty(cmd.PropertyName))
                        return BatchEditorModifyResult.Filtered;
                    if (ReflectUtil.GetValueEquals(PKM, cmd.PropertyName, cmd.PropertyValue) != cmd.Evaluator)
                        return BatchEditorModifyResult.Filtered;
                }
                catch
                {
                    Console.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}.");
                    return BatchEditorModifyResult.Filtered;
                }
            }

            BatchEditorModifyResult result = BatchEditorModifyResult.Error;
            foreach (var cmd in Instructions)
            {
                try
                {
                    if (cmd.PropertyName == nameof(PKM.MetDate))
                        PKM.MetDate = DateTime.ParseExact(cmd.PropertyValue, CONST_DATEFORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    else if (cmd.PropertyName == nameof(PKM.EggMetDate))
                        PKM.EggMetDate = DateTime.ParseExact(cmd.PropertyValue, CONST_DATEFORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == CONST_RAND)
                        ReflectUtil.SetValue(PKM, cmd.PropertyName, Util.rnd32().ToString());
                    else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_RAND)
                        PKM.setPIDGender(PKM.Gender);
                    else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == "PID")
                        PKM.EncryptionConstant = PKM.PID;
                    else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_SHINY)
                        PKM.setShinyPID();
                    else if (cmd.PropertyName == nameof(PKM.Species) && cmd.PropertyValue == "0")
                        PKM.Data = new byte[PKM.Data.Length];
                    else
                        ReflectUtil.SetValue(PKM, cmd.PropertyName, cmd.PropertyValue);

                    result = BatchEditorModifyResult.Modified;
                }
                catch { Console.WriteLine($"Unable to set {cmd.PropertyName} to {cmd.PropertyValue}."); }
            }
            return result;
        }

        public static IEnumerable<T> applyFilters<T>(IEnumerable<T> enumerable, IEnumerable<BatchEditorStringInstruction> filters)
        {
            return enumerable.Where(x => // Compare across all filters
            {
                foreach (var cmd in filters)
                {
                    if (!x.GetType().HasPropertyAll(cmd.PropertyName))
                        return false;
                    try { if (ReflectUtil.GetValueEquals(x, cmd.PropertyName, cmd.PropertyValue) == cmd.Evaluator) continue; }
                    catch { Console.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}."); }
                    return false;
                }
                return true;
            });
        }
        #endregion
    }
}
