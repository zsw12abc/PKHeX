﻿using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SaveFile"/> format for <see cref="GameVersion.DP"/>
    /// </summary>
    public sealed class SAV4DP : SAV4Sinnoh
    {
        public SAV4DP() : base(GeneralSize, StorageSize)
        {
            Initialize();
            Dex = new Zukan4(this, PokeDex);
        }

        public SAV4DP(byte[] data) : base(data, GeneralSize, StorageSize, GeneralSize)
        {
            Initialize();
            Dex = new Zukan4(this, PokeDex);
        }

        public override Zukan4 Dex { get; }

        protected override SAV4 CloneInternal4() => State.Exportable ? new SAV4DP(Data) : new SAV4DP();
        public override PersonalTable Personal => PersonalTable.DP;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_DP;
        public override int MaxItemID => Legal.MaxItemID_4_DP;

        private const int GeneralSize = 0xC100;
        private const int StorageSize = 0x121E0; // Start 0xC100, +4 starts box data
        protected override int StorageStart => GeneralSize;

        private void Initialize()
        {
            Version = GameVersion.DP;
            GetSAVOffsets();
        }

        private void GetSAVOffsets()
        {
            AdventureInfo = 0;
            Trainer1 = 0x64;
            Party = 0x98;
            PokeDex = 0x12DC;
            WondercardFlags = 0xA6D0;
            WondercardData = 0xA7fC;

            EventConst = 0xD9C;
            EventFlag = 0xFDC;
            DaycareOffset = 0x141C;
            OFS_HONEY = 0x72E4;

            OFS_UG_Stats = 0x3A2C;
            OFS_UG_Items = 0x42B0;

            PoketchStart = 0x114C;
            Seal = 0x6178;

            OFS_PoffinCase = 0x5050;

            Box = 4;
        }

        #region Storage
        public override int GetBoxWallpaper(int box)
        {
            if ((uint)box >= 18)
                return 0;
            return Storage[GetBoxWallpaperOffset(box)];
        }

        public override void SetBoxWallpaper(int box, int value)
        {
            if ((uint)box >= 18)
                return;
            Storage[GetBoxWallpaperOffset(box)] = (byte)value;
        }
        #endregion

        public override IReadOnlyList<InventoryPouch> Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch4(InventoryType.Items, Legal.Pouch_Items_DP, 999, 0x624),
                    new InventoryPouch4(InventoryType.KeyItems, Legal.Pouch_Key_DP, 1, 0x8B8),
                    new InventoryPouch4(InventoryType.TMHMs, Legal.Pouch_TMHM_DP, 99, 0x980),
                    new InventoryPouch4(InventoryType.MailItems, Legal.Pouch_Mail_DP, 999, 0xB10),
                    new InventoryPouch4(InventoryType.Medicine, Legal.Pouch_Medicine_DP, 999, 0xB40),
                    new InventoryPouch4(InventoryType.Berries, Legal.Pouch_Berries_DP, 999, 0xBE0),
                    new InventoryPouch4(InventoryType.Balls, Legal.Pouch_Ball_DP, 999, 0xCE0),
                    new InventoryPouch4(InventoryType.BattleItems, Legal.Pouch_Battle_DP, 999, 0xD1C),
                };
                return pouch.LoadAll(General);
            }
            set => value.SaveAll(General);
        }

        // reverse crc32 polynomial, nice!
        private const uint MysteryGiftDPSlotActive = 0xEDB88320;

        public bool[] GetMysteryGiftDPSlotActiveFlags()
        {
            int ofs = WondercardFlags + 0x100; // skip over flags
            bool[] active = new bool[GiftCountMax]; // 8 PGT, 3 PCD
            for (int i = 0; i < active.Length; i++)
                active[i] = BitConverter.ToUInt32(General, ofs + (4 * i)) == MysteryGiftDPSlotActive;

            return active;
        }

        public void SetMysteryGiftDPSlotActiveFlags(bool[] value)
        {
            if (value.Length != GiftCountMax)
                return;

            int ofs = WondercardFlags + 0x100; // skip over flags
            for (int i = 0; i < value.Length; i++)
            {
                byte[] magic = BitConverter.GetBytes(value[i] ? MysteryGiftDPSlotActive : 0); // 4 bytes
                SetData(General, magic, ofs + (4 * i));
            }
        }

        public override MysteryGiftAlbum GiftAlbum
        {
            get => base.GiftAlbum;
            set
            {
                base.GiftAlbum = value;
                SetActiveGiftFlags(value.Gifts);
            }
        }

        private void SetActiveGiftFlags(IReadOnlyList<MysteryGift> gifts)
        {
            var arr = new bool[gifts.Count];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = !gifts[i].Empty;
            SetMysteryGiftDPSlotActiveFlags(arr);
        }

        public override int M { get => BitConverter.ToUInt16(General, 0x1238); set => BitConverter.GetBytes((ushort)value).CopyTo(General, 0x1238); }
        public override int X { get => BitConverter.ToUInt16(General, 0x1240); set => BitConverter.GetBytes((ushort)(X2 = value)).CopyTo(General, 0x1240); }
        public override int Y { get => BitConverter.ToUInt16(General, 0x1244); set => BitConverter.GetBytes((ushort)(Y2 = value)).CopyTo(General, 0x1244); }
        public override int X2 { get => BitConverter.ToUInt16(General, 0x25FA); set => BitConverter.GetBytes((ushort)value).CopyTo(General, 0x25FA); }
        public override int Y2 { get => BitConverter.ToUInt16(General, 0x25FE); set => BitConverter.GetBytes((ushort)value).CopyTo(General, 0x25FE); }
        public override int Z { get => BitConverter.ToUInt16(General, 0x2602); set => BitConverter.GetBytes((ushort)value).CopyTo(General, 0x2602); }
    }
}
