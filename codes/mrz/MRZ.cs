using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace eVerification.codes.mrz
{
    public class MRZ
    {
        private string line1, line2, line3, m_MRZ_info;

        public string Line1
        {
            get { return line1; }
            set { line1 = value; }
        }

        public string Line2
        {
            get { return line2; }
            set { line2 = value; }
        }

        public string Line3
        {
            get { return line3; }
            set { line3 = value; }
        }
        public bool checksum;

        public string MRZ_info
        {
            get { return m_MRZ_info; }
            set { m_MRZ_info = value; }
        }

        public MRZ(string mrz)
        {
            //string[] lines = mrz.TrimEnd('\0').TrimEnd('\r').Split('\r');
            //string[] lines = mrz.Replace(Environment.NewLine, "").Split('\r');
            String oneLine = Regex.Replace(mrz, "([\r\n]+)$", "");
            string[] lines = oneLine.Split('\r');
            this.line1 = lines[0].Trim('\r').Trim('\n');
            if (lines.Length > 1) {
                this.line2 = lines[1].Trim('\r').Trim('\n');
                if (this.line2 == null)
                {
                    this.line2 = string.Empty;
                }
            } else
            {
                this.line2 = string.Empty;
            }
           
            this.checksum = false;
            if (lines.Length > 2 && lines[1].Length < 34)
            {
                this.line3 = lines[2].Trim('\r').Trim('\n');
                extractMRZInfo(3);
            }
            else if (lines.Length > 1)
            {
                this.line3 = string.Empty;
                extractMRZInfo(2);
            }
            else
            {
                this.line2 = string.Empty;
                this.line3 = string.Empty;
                extractMRZInfo(1);
            }
        }

        private void extractMRZInfo(int nblines)
        {
            if (nblines == 3)
            {
                if (this.line1.Length > 14 && this.line2.Length > 14)
                {
                    MRZ_info = this.line1.Substring(5, 10) + this.line2.Substring(0, 7) + this.line2.Substring(8, 7);
                    if (!ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.line1.Substring(5, 10)) ||
                        !ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.line2.Substring(0, 7)) ||
                        !ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.line2.Substring(8, 7)))

                        checksum = false;
                    else
                        checksum = true;
                }
            }
            else if (nblines == 2)
            {
                if (this.line2.Length > 27)
                {
                    MRZ_info = this.line2.Substring(0, 10) + this.line2.Substring(13, 7) + this.line2.Substring(21, 7);
                    if (!ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.line2.Substring(0, 10)) ||
                        !ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.line2.Substring(13, 7)) ||
                        !ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.line2.Substring(21, 7)))

                        checksum = false;
                    else
                        checksum = true;
                }
            }
            else
            {
                if (this.line1.Length > 29)
                {
                    MRZ_info = this.line1.Substring(1, 28);
                    checksum = true;
                }
            }
        }
    }
}
