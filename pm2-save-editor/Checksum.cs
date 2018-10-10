﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace pm2_save_editor
{

    enum ChecksumVariants { EnglishRefine = 0, JapaneseRefine = 1 };

    /// <summary>
    /// Provides functionality for calcuating checksums of save files
    /// </summary>
    static class Checksum
    {

        public const int ENGLISH_REFINE_CHECKSUM_OFFSET = 0x1B4C;
        public const int JAPANESE_REFINE_CHECKSUM_OFFSET = 0x1114; // will most likely require a new checksum function to handle, based on observed differences so far

        [DllImport("pm2-checksum.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CalculateChecksum(IntPtr save_file, int version);

        public static int CalculateChecksum(byte[] file, PrincessMakerFileBuffer.Version workingVersion)
        {
            if (workingVersion == PrincessMakerFileBuffer.Version.EnglishRefine)
            {
                return FullCheckSum(file, workingVersion);
            }
            
            return PartialChecksum(file);

        }

        private static int PartialChecksum(byte[] file)
        {
            return 0x00;
        }

        /// <summary>
        /// Calcuate a full file checksum using the unported C++ code
        /// </summary>
        /// <param name="file">A byte array represening the file</param>
        /// <param name="workingVersion">The version of the game that the file was creaed by</param>
        /// <returns></returns>
        private static int FullCheckSum(byte[] file, PrincessMakerFileBuffer.Version workingVersion)
        {

            // A warning for the programmer lead astray - will most likely enable partial checksum for all files at some point
            if (workingVersion != PrincessMakerFileBuffer.Version.EnglishRefine)
            {
                MessageBox.Show("Full Checksum is only compatible with English Refine files");
                Environment.Exit(0);
            }

            // May want to wrap this in a try/catch/finally block to cover edge cases
            IntPtr fileBuffer = Marshal.AllocHGlobal(file.Length);
            Marshal.Copy(file, 0, fileBuffer, file.Length);

            int checksum = CalculateChecksum(fileBuffer, (int)workingVersion);

            Marshal.FreeHGlobal(fileBuffer);

            return checksum;
        }

    }
}
