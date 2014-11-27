using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OpenXC.Web.Utilities
{
    public static class HexFileUtility
    {
        public struct HexLine
        {
            public byte RecDataLen;
            public UInt32 Address;
            public byte RecType;
            public Byte[] Data;
            public byte Checksum;
        }
        public class FlashCRCData
        {
            public ushort CRC;
            public UInt32 StartAddress;
            public UInt32 Length;
            public string flashMD5;
            public byte[] bytesToAppend;
        }

        // private embedded class HEXMOD
        public static FlashCRCData ProcessHEX(byte[] hexfile)
        {
            FlashCRCData flashCRC = VerifyHexFile(hexfile);

            byte start_address_checksum = (byte)(~(0x04 + 0xA1 + (byte)((flashCRC.StartAddress & 0xFF000000) >> 24) + (byte)((flashCRC.StartAddress & 0x00FF0000) >> 16) +
                (byte)((flashCRC.StartAddress & 0x0000FF00) >> 8) + (byte)((flashCRC.StartAddress & 0x000000FF))) + 1);
            byte length_checksum = (byte)(~(0x03 + 0xA2 + (byte)((flashCRC.Length & 0xFF0000) >> 16) + (byte)((flashCRC.Length & 0x00FF00) >> 8) + (byte)((flashCRC.Length & 0x0000FF))) + 1);
            byte crc_checksum = (byte)(~(0x02 + 0xA3 + (byte)((flashCRC.CRC & 0xFF00) >> 8) + (byte)((flashCRC.CRC & 0x00FF))) + 1);
            string appendToHex = ";" + "04" + "0000" + "A1" + flashCRC.StartAddress.ToString("X8") + start_address_checksum.ToString("X2") + "\r\n";
            appendToHex += ";" + "03" + "0000" + "A2" + flashCRC.Length.ToString("X6") + length_checksum.ToString("X2") + "\r\n";
            appendToHex += ";" + "02" + "0000" + "A3" + flashCRC.CRC.ToString("X4") + crc_checksum.ToString("X2") + "\r\n";

            flashCRC.bytesToAppend = Encoding.ASCII.GetBytes(appendToHex);
            return flashCRC;
        }

        private static FlashCRCData VerifyHexFile(byte[] hexfile)
        {
            // Here we want to find the right hex file.
            //TextReader tr = new StreamReader(FileName);
            // Read entire file into a buffer, and remove all the colons.
            string HexFile = Encoding.UTF8.GetString(hexfile, 0, hexfile.Length);/*tr.ReadToEnd();*/
            //tr.Close();
            string[] HexFileLines = HexFile.Split(':');
            HexLine[] hexLines = new HexLine[HexFileLines.Length];
            int i = 0;
            foreach (string HexLineString in HexFileLines)
            {
                if (HexLineString.Length > 5 && !HexLineString.Equals("00000001FF\r\n"))
                {
                    //Convert into numeric
                    hexLines[i].RecDataLen = Convert.ToByte(HexLineString.Substring(0, 2), 16);
                    hexLines[i].Address = Convert.ToUInt32(HexLineString.Substring(2, 4), 16);
                    hexLines[i].RecType = Convert.ToByte(HexLineString.Substring(6, 2), 16);
                    hexLines[i].Data = StringToByteArray(HexLineString.Substring(8, hexLines[i].RecDataLen * 2));
                    hexLines[i].Checksum = Convert.ToByte(HexLineString.Substring(8 + hexLines[i].RecDataLen * 2, 2), 16);
                    //Calculate the virtual Flash Space.
                    i++;
                }
            }
            hexLines = hexLines.Take(i).ToArray();
            return VerifyFlash(hexLines);
        }

        private static ushort CalculateCrc(Byte[] data, int len)
        {

            uint i;
            uint inc = 0;
            ushort crc = 0xFFFF;
            //ushort crc = 0;
            const uint L_Mask = 0x0F;
            byte reflectedByte;
            ushort[] crc_table = new ushort[16]
            {
                0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
                0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef
            };
            byte[] BitReverseTable256 = new byte[256] {
              0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0,
              0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8,
              0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4,
              0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC,
              0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2,
              0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA,
              0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6,
              0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE,
              0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
              0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9,
              0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5,
              0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD,
              0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3,
              0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB,
              0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7,
              0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF
            };

            while (len-- != 0)
            {
                reflectedByte = BitReverseTable256[data[inc]];
                i = (uint)(crc >> 12) ^ (uint)(reflectedByte >> 4);
                crc = (ushort)((ushort)crc_table[i & L_Mask] ^ (ushort)(crc << 4));
                i = (uint)(crc >> 12) ^ (uint)(reflectedByte >> 0);
                crc = (ushort)((ushort)crc_table[i & L_Mask] ^ (ushort)(crc << 4));
                inc++;
            }

            //return ((ushort)(crc & 0xFFFF));
            return (ushort)(BitReverseTable256[(byte)((crc & 0xFF00) >> 8)] + (BitReverseTable256[(byte)(crc & 0x00FF)] << 8));
        }

        private static FlashCRCData VerifyFlash(HexLine[] hexLines)
        {
            uint ProgLen;
            uint VirtualFlashAdrs;
            uint ProgAddress;
            // 5 MB Virtual Flash.
            byte[] VirtualFlash = new byte[5 * 1024 * 1024];
            for (int i = 0; i < 5 * 1024 * 1024; i++) // initialize virtual flash to all FFs
            {
                VirtualFlash[i] = 0xFF;
            }
            uint ExtLinAddress = 0;
            uint ExtSegAddress = 0;
            uint MaxAddress = 0;
            uint MinAddress = 0xFFFFFFFF;
            // Reset max address and min address.

            for (int i = 0; i < hexLines.Length; i++)
            {
                HexLine HexRecordSt = hexLines[i];
                switch (HexRecordSt.RecType)
                {

                    case 0:  //Record Type 00, data record.\
                        //Dont need the following line since we have extacted this data elsewhere.
                        //HexRecordSt.Address = (((HexRec[1] << 8) & 0x0000FF00) | (HexRec[2] & 0x000000FF)) & (0x0000FFFF);
                        HexRecordSt.Address = HexRecordSt.Address + ExtLinAddress + ExtSegAddress;

                        ProgAddress = (HexRecordSt.Address | 0x80000000);

                        if (ProgAddress < 0x9FC00000) // Make sure we are not writing boot sector.
                        {
                            if (MaxAddress < (ProgAddress + HexRecordSt.RecDataLen))
                            {
                                MaxAddress = ProgAddress + HexRecordSt.RecDataLen;
                            }

                            if (MinAddress > ProgAddress)
                            {
                                MinAddress = ProgAddress;
                            }

                            VirtualFlashAdrs = (ProgAddress - 0x9D000000); // Program address to local virtual flash address

                            foreach (byte CurrData in HexRecordSt.Data)
                            {
                                VirtualFlash[VirtualFlashAdrs++] = CurrData;
                            }
                            //memcpy((void *)&VirtualFlash[VirtualFlashAdrs], HexRecordSt.Data, HexRecordSt.RecDataLen);
                        }
                        break;

                    case 02:  // Record Type 02, defines 4 to 19 of the data address.
                        ExtSegAddress = (((uint)HexRecordSt.Data[0] << 16) & 0x00FF0000u) | (((uint)HexRecordSt.Data[1] << 8) & 0x0000FF00u);
                        ExtLinAddress = 0;
                        break;

                    case 04:
                        ExtLinAddress = (((uint)HexRecordSt.Data[0] << 24) & 0xFF000000) | (((uint)HexRecordSt.Data[1] << 16) & 0x00FF0000);
                        ExtSegAddress = 0;
                        break;


                    case 01:  //Record Type 01
                    default:
                        ExtSegAddress = 0;
                        ExtLinAddress = 0;
                        break;
                }
            }

            MinAddress -= MinAddress % 4;
            MaxAddress += MaxAddress % 4;

            ProgLen = MaxAddress - MinAddress;
            //StartAdress = HexRecordSt.MinAddress;
            VirtualFlashAdrs = (MinAddress - 0x9D000000);
            //*crc = CalculateCrc((char*)&VirtualFlash[VirtualFlashAdrs], *);	
            byte[] Temp = VirtualFlash.Skip((int)VirtualFlashAdrs).Take((int)ProgLen).ToArray(); // array of flash memory to be programmed
            byte[] Temp2 = VirtualFlash.Skip((int)0x1000).Take((int)0x2FC).Concat(VirtualFlash.Skip((int)0x1348).Take((int)0x7DCB8)).ToArray();

            return new FlashCRCData
            {
                CRC = CalculateCrc(Temp, (int)ProgLen),
                Length = ProgLen,
                StartAddress = MinAddress,
                flashMD5 = GetMd5Hash(Temp2)
            };
        }

        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Get the MD5 hash of a byte array.
        /// </summary>
        /// <param name="bytes">The bytes to hash.</param>
        /// <returns>MD5 hash as a hex string.</returns>
        public static string GetMd5Hash(byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}