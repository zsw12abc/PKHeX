using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Reflection
{
    public class BatchEditUtil
    {
        public static void screenStrings(IEnumerable<BatchEditorStringInstruction> il)
        {
            foreach (var i in il.Where(i => !i.PropertyValue.All(char.IsDigit)))
            {
                switch (i.PropertyName)
                {
                    case nameof(PKM.Species): i.setScreenedValue(Main.GameStrings.specieslist); continue;
                    case nameof(PKM.HeldItem): i.setScreenedValue(Main.GameStrings.itemlist); continue;
                    case nameof(PKM.Move1): case nameof(PKM.Move2): case nameof(PKM.Move3): case nameof(PKM.Move4): i.setScreenedValue(Main.GameStrings.movelist); continue;
                    case nameof(PKM.RelearnMove1): case nameof(PKM.RelearnMove2): case nameof(PKM.RelearnMove3): case nameof(PKM.RelearnMove4): i.setScreenedValue(Main.GameStrings.movelist); continue;
                    case nameof(PKM.Ability): i.setScreenedValue(Main.GameStrings.abilitylist); continue;
                    case nameof(PKM.Nature): i.setScreenedValue(Main.GameStrings.natures); continue;
                    case nameof(PKM.Ball): i.setScreenedValue(Main.GameStrings.balllist); continue;
                }
            }
        }
    }
}
