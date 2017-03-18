﻿using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_3 = 386;
        internal const int MaxMoveID_3 = 354;
        internal const int MaxItemID_3 = 374;
        internal const int MaxAbilityID_3 = 77;
        internal const int MaxBallID_3 = 0xC;
        
        #region RS
        internal static readonly ushort[] Pouch_Items_RS = {
            13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 63, 64, 65, 66, 67, 68, 69, 70, 71, 73, 74, 75, 76, 77, 78, 79, 80, 81, 83, 84, 85, 86, 93, 94, 95, 96, 97, 98, 103, 104, 106, 107, 108, 109, 110, 111, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 254, 255, 256, 257, 258
        };
        internal static readonly ushort[] Pouch_Key_RS = {
            259, 260, 261, 262, 263, 264, 265, 266, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288
        };
        internal static readonly ushort[] Pouch_TM_RS = {
            289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338,
        };

        public static readonly ushort[] Pouch_HM_RS = {
            339, 340, 341, 342, 343, 344, 345, 346
        };
        internal static readonly ushort[] Pouch_Berries_RS = {
            133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175
        };
        internal static readonly ushort[] Pouch_Ball_RS = {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        };
        internal static readonly ushort[] Pouch_Key_FRLG = Pouch_Key_RS.Concat(new ushort[] { 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374 }).ToArray();
        internal static readonly ushort[] Pouch_Key_E = Pouch_Key_FRLG.Concat(new ushort[] { 375, 376 }).ToArray();

        internal static readonly ushort[] Pouch_TMHM_RS = Pouch_TM_RS.Concat(Pouch_HM_RS).ToArray();
        internal static readonly ushort[] HeldItems_RS = new ushort[1].Concat(Pouch_Items_RS).Concat(Pouch_Ball_RS).Concat(Pouch_Berries_RS).Concat(Pouch_TM_RS).ToArray();
        #endregion

        internal static readonly int[] MovePP_RS =
        {
            00, 
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 10, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20, 
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 20, 10, 10, 40, 25, 10, 35, 30, 15, 20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20, 
            15, 10, 40, 15, 20, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40, 
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 10, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 15, 
            10, 05, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 15, 15, 15, 
            10, 10, 10, 10, 10, 10, 10, 10, 15, 15, 15, 10, 20, 20, 10, 20, 20, 20, 20, 20, 10, 10, 10, 20, 20, 05, 15, 10, 10, 15, 10, 20, 05, 05, 10, 10, 20, 05, 10, 20, 10, 20, 20, 20, 05, 05, 15, 20, 10, 15, 
            20, 15, 10, 10, 15, 10, 05, 05, 10, 15, 10, 05, 20, 25, 05, 40, 10, 05, 40, 15, 20, 20, 05, 15, 20, 30, 15, 15, 05, 10, 30, 20, 30, 15, 05, 40, 15, 05, 20, 05, 15, 25, 40, 15, 20, 15, 20, 15, 20, 10, 
            20, 20, 05, 05, 
        };

        internal static readonly ushort[] Pouch_Cologne_CXD = {543, 544, 545};
        internal static readonly ushort[] Pouch_Items_COLO = Pouch_Items_RS.Concat(new ushort[] {537}).ToArray(); // Time Flute
        internal static readonly ushort[] HeldItems_COLO = new ushort[1].Concat(Pouch_Items_COLO).Concat(Pouch_Ball_RS).Concat(Pouch_Berries_RS).Concat(Pouch_TM_RS).ToArray();
        internal static readonly ushort[] Pouch_Key_COLO =
        {
            500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
            510, 511, 512, 513, 514, 515, 516, 517, 518, 519,
            520, 521, 522, 523, 524, 525, 526, 527, 528, 529,
            530, 531, 532, 533, 534, 535, 536,      538, 539,
            540, 541, 542,                546, 547,
        };

        internal static readonly ushort[] Pouch_Items_XD = Pouch_Items_RS.Concat(new ushort[] {511}).ToArray(); // Poké Snack
        internal static readonly ushort[] HeldItems_XD = new ushort[1].Concat(Pouch_Items_XD).Concat(Pouch_Ball_RS).Concat(Pouch_Berries_RS).Concat(Pouch_TM_RS).ToArray();
        internal static readonly ushort[] Pouch_Key_XD =
        {
            500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
            510,      512,                516, 517, 518, 519,
                           523, 524, 525, 526, 527, 528, 529,
            530, 531, 532, 533
        };
        internal static readonly ushort[] Pouch_Disc_XD =
        {
            534, 535, 536, 537, 538, 539,
            540, 541, 542, 543, 544, 545, 546, 547, 548, 549,
            550, 551, 552, 553, 554, 555, 556, 557, 558, 559,
            560, 561, 562, 563, 564, 565, 566, 567, 568, 569,
            570, 571, 572, 573, 574, 575, 576, 577, 578, 579,
            580, 581, 582, 583, 584, 585, 586, 587, 588, 589,
            590, 591, 592, 593
        };
        internal static readonly int[] WildPokeBalls3 = {1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12};

        internal static readonly int[] FutureEvolutionsGen3 =
        {
            407,424,429,430,461,462,463,464,465,466,467,468,469,470,471,472,473,474,475,476,477,700
        };

        internal static readonly int[] FutureEvolutionsGen3_LevelUp = 
        {
            424, 461, 462, 463, 465, 469, 470, 471, 472, 473, 476
        };
        // Ambipom Weavile Magnezone Lickilicky Tangrowth
        // Yanmega Leafeon Glaceon Mamoswine Gliscor Probopass
        internal static readonly int[] UnreleasedItems_3 =
        {
            // todo
        };
        internal static readonly bool[] ReleasedHeldItems_3 = Enumerable.Range(0, MaxItemID_3+1).Select(i => HeldItems_RS.Contains((ushort)i) && !UnreleasedItems_3.Contains(i)).ToArray();
        internal static readonly int[] TM_3 =
        {
            264, 337, 352, 347, 046, 092, 258, 339, 331, 237,
            241, 269, 058, 059, 063, 113, 182, 240, 202, 219,
            218, 076, 231, 085, 087, 089, 216, 091, 094, 247,
            280, 104, 115, 351, 053, 188, 201, 126, 317, 332,
            259, 263, 290, 156, 213, 168, 211, 285, 289, 315,
        };
        internal static readonly int[] HM_3 = {15, 19, 57, 70, 148, 249, 127, 291};
        internal static readonly int[] TypeTutor3 = {338, 307, 308};
        internal static readonly int[] Tutor_3Mew =
        {
            185, // Feint Attack
            252, // Fake Out
            095, // Hypnosis
            101, // Night Shade
            272, // Role Play
            192, // Zap Cannon
        };
        internal static readonly int[][] Tutor_Frontier =
        {
            new[] {135, 069, 138, 005, 025, 034, 157, 068, 086, 014},
            new[] {111, 173, 189, 129, 196, 203, 244, 008, 009, 007},
        };
        internal static readonly int[] Tutor_E = {038, 223, 153, 210, 118, 102, 205, 214, 164, 207};
        internal static readonly int[] Tutor_FRLG = {034, 068, 038, 138, 153, 025, 005, 118, 102, 157, 069, 135, 164, 014, 086};

        internal static readonly EncounterStatic[] Encounter_RSE =
        {
            //todo
        };
        internal static readonly EncounterStatic[] Encounter_FRLG =
        {
            //todo
        };
        internal static readonly EncounterTrade[] TradeGift_RSE =
        {
            //todo
        };
        internal static readonly EncounterTrade[] TradeGift_FRLG =
        {
            //todo
        };

    }
}
