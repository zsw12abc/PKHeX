﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Extension methods for <see cref="SaveFile"/> syntax sugar.
    /// </summary>
    public static class SaveExtensions
    {
        /// <summary>
        /// Checks a <see cref="PKM"/> file for compatibility to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="sav"><see cref="SaveFile"/> that is being checked.</param>
        /// <param name="pkm"><see cref="PKM"/> that is being tested for compatibility.</param>
        public static IReadOnlyList<string> IsPKMCompatible(this SaveFile sav, PKM pkm)
        {
            return sav.GetSaveFileErrata(pkm, GameInfo.Strings);
        }

        private static IReadOnlyList<string> GetSaveFileErrata(this SaveFile sav, PKM pkm, IBasicStrings strings)
        {
            var errata = new List<string>();
            ushort held = (ushort)pkm.HeldItem;
            if (sav.Generation > 1 && held != 0)
            {
                string? msg = null;
                if (held > sav.MaxItemID)
                    msg = MsgIndexItemGame;
                else if (!pkm.CanHoldItem(sav.HeldItems))
                    msg = MsgIndexItemHeld;
                if (msg != null)
                {
                    var itemstr = GameInfo.Strings.GetItemStrings(pkm.Format, (GameVersion)pkm.Version);
                    errata.Add($"{msg} {(held >= itemstr.Length ? held.ToString() : itemstr[held])}");
                }
            }

            if (pkm.Species > strings.Species.Count)
                errata.Add($"{MsgIndexSpeciesRange} {pkm.Species}");
            else if (sav.MaxSpeciesID < pkm.Species)
                errata.Add($"{MsgIndexSpeciesGame} {strings.Species[pkm.Species]}");

            if (!sav.Personal[pkm.Species].IsFormWithinRange(pkm.Form) && !FormInfo.IsValidOutOfBoundsForm(pkm.Species, pkm.Form, pkm.Generation))
                errata.Add(string.Format(LegalityCheckStrings.LFormInvalidRange, Math.Max(0, sav.Personal[pkm.Species].FormCount - 1), pkm.Form));

            if (pkm.Moves.Any(m => m > strings.Move.Count))
                errata.Add($"{MsgIndexMoveRange} {string.Join(", ", pkm.Moves.Where(m => m > strings.Move.Count).Select(m => m.ToString()))}");
            else if (pkm.Moves.Any(m => m > sav.MaxMoveID))
                errata.Add($"{MsgIndexMoveGame} {string.Join(", ", pkm.Moves.Where(m => m > sav.MaxMoveID).Select(m => strings.Move[m]))}");

            if (pkm.Ability > strings.Ability.Count)
                errata.Add($"{MsgIndexAbilityRange} {pkm.Ability}");
            else if (pkm.Ability > sav.MaxAbilityID)
                errata.Add($"{MsgIndexAbilityGame} {strings.Ability[pkm.Ability]}");

            return errata;
        }

        /// <summary>
        /// Imports compatible <see cref="PKM"/> data to the <see cref="sav"/>, starting at the provided box.
        /// </summary>
        /// <param name="sav">Save File that will receive the <see cref="compat"/> data.</param>
        /// <param name="compat">Compatible <see cref="PKM"/> data that can be set to the <see cref="sav"/> without conversion.</param>
        /// <param name="overwrite">Overwrite existing full slots. If true, will only overwrite empty slots.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <returns>Count of injected <see cref="PKM"/>.</returns>
        public static int ImportPKMs(this SaveFile sav, IEnumerable<PKM> compat, bool overwrite = false, int boxStart = 0, PKMImportSetting noSetb = PKMImportSetting.UseDefault)
        {
            int startCount = boxStart * sav.BoxSlotCount;
            int maxCount = sav.SlotCount;
            int index = startCount;
            int nonOverwriteImport = 0;

            foreach (var pk in compat)
            {
                if (overwrite)
                {
                    while (sav.IsSlotOverwriteProtected(index))
                        ++index;

                    // The above will return false if out of range. We need to double-check.
                    if (index >= maxCount) // Boxes full!
                        break;

                    sav.SetBoxSlotAtIndex(pk, index, noSetb);
                }
                else
                {
                    index = sav.NextOpenBoxSlot(index-1);
                    if (index < 0) // Boxes full!
                        break;

                    sav.SetBoxSlotAtIndex(pk, index, noSetb);
                    nonOverwriteImport++;
                }

                if (++index == maxCount) // Boxes full!
                    break;
            }
            return overwrite ? index - startCount : nonOverwriteImport; // actual imported count
        }

        public static IEnumerable<PKM> GetCompatible(this SaveFile sav, IEnumerable<PKM> pks)
        {
            var savtype = sav.PKMType;

            foreach (var temp in pks)
            {
                var pk = PKMConverter.ConvertToType(temp, savtype, out string c);
                if (pk == null)
                {
                    Debug.WriteLine(c);
                    continue;
                }

                if (sav is ILangDeviantSave il && PKMConverter.IsIncompatibleGB(temp, il.Japanese, pk.Japanese))
                {
                    c = PKMConverter.GetIncompatibleGBMessage(pk, il.Japanese);
                    Debug.WriteLine(c);
                    continue;
                }

                var compat = sav.IsPKMCompatible(pk);
                if (compat.Count > 0)
                    continue;

                yield return pk;
            }
        }

        /// <summary>
        /// Gets a compatible <see cref="PKM"/> for editing with a new <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="sav">SaveFile to receive the compatible <see cref="pk"/></param>
        /// <param name="pk">Current Pokémon being edited</param>
        /// <returns>Current Pokémon, assuming conversion is possible. If conversion is not possible, a blank <see cref="PKM"/> will be obtained from the <see cref="sav"/>.</returns>
        public static PKM GetCompatiblePKM(this SaveFile sav, PKM pk)
        {
            if (pk.Format >= 3 || sav.Generation >= 7)
                return PKMConverter.ConvertToType(pk, sav.PKMType, out _) ?? sav.BlankPKM;
            // gen1-2 compatibility check
            if (pk.Japanese != ((ILangDeviantSave)sav).Japanese)
                return sav.BlankPKM;
            if (sav is SAV2 s2 && s2.Korean != pk.Korean)
                return sav.BlankPKM;
            return PKMConverter.ConvertToType(pk, sav.PKMType, out _) ?? sav.BlankPKM;
        }

        /// <summary>
        /// Gets a blank file for the save file. If the template path exists, a template load will be attempted.
        /// </summary>
        /// <param name="sav">Save File to fetch a template for</param>
        /// <returns>Template if it exists, or a blank <see cref="PKM"/> from the <see cref="sav"/></returns>
        private static PKM LoadTemplateInternal(this SaveFile sav) => sav.BlankPKM;

        /// <summary>
        /// Gets a blank file for the save file. If the template path exists, a template load will be attempted.
        /// </summary>
        /// <param name="sav">Save File to fetch a template for</param>
        /// <param name="templatePath">Path to look for a template in</param>
        /// <returns>Template if it exists, or a blank <see cref="PKM"/> from the <see cref="sav"/></returns>
        public static PKM LoadTemplate(this SaveFile sav, string? templatePath = null)
        {
            if (templatePath == null || !Directory.Exists(templatePath))
                return LoadTemplateInternal(sav);

            var di = new DirectoryInfo(templatePath);
            string path = Path.Combine(templatePath, $"{di.Name}.{sav.PKMType.Name.ToLower()}");

            if (!File.Exists(path) || !PKX.IsPKM(new FileInfo(path).Length))
                return LoadTemplateInternal(sav);

            var pk = PKMConverter.GetPKMfromBytes(File.ReadAllBytes(path), prefer: sav.Generation);
            if (pk == null)
                return LoadTemplateInternal(sav);

            return PKMConverter.ConvertToType(pk, sav.BlankPKM.GetType(), out _) ?? LoadTemplateInternal(sav);
        }
    }
}
