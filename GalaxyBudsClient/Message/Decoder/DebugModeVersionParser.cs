﻿using System;
using System.Collections.Generic;
using GalaxyBudsClient.Model.Constants;

namespace GalaxyBudsClient.Message.Decoder
{
    class DebugModeVersionParser : BaseMessageParser
    {
        public override SppMessage.MessageIds HandledType => SppMessage.MessageIds.VERSION_INFO;

        readonly String[] _swMonth = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
        readonly String[] _swRelVer = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        readonly String[] _swVer = { "E", "U" };
        readonly String[] _swYear = { "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        public String? LeftHardwareVersion { set; get; }
        public String? RightHardwareVersion { set; get; }
        public String? LeftSoftwareVersion { set; get; }
        public String? RightSoftwareVersion { set; get; }
        public String? LeftTouchSoftwareVersion { set; get; }
        public String? RightTouchSoftwareVersion { set; get; }

        public override void ParseMessage(SppMessage msg)
        {
            if (msg.Id != HandledType)
                return;

            int l1 = (msg.Payload[0] & 240) >> 4;
            int l2 = (msg.Payload[0] & 15);
            int r1 = (msg.Payload[1] & 240) >> 4;
            int r2 = (msg.Payload[1] & 15);

            LeftHardwareVersion = "rev" + l1.ToString("X") + "." + l2.ToString("X");
            RightHardwareVersion = "rev" + r1.ToString("X") + "." + r2.ToString("X");

            LeftSoftwareVersion = VersionDataToString(msg.Payload, 2, "L");
            RightSoftwareVersion = VersionDataToString(msg.Payload, 5, "R");

            LeftTouchSoftwareVersion = msg.Payload[8].ToString("x");
            RightTouchSoftwareVersion = msg.Payload[9].ToString("x");
        }

        private string VersionDataToString(IReadOnlyList<byte> payload, int startIndex, string side)
        {
            if (ActiveModel == Models.Buds)
            {
                int swVarIndex = payload[startIndex];
                var swYearIndex = (payload[startIndex + 1] & 240) >> 4;
                var swMonthIndex = payload[startIndex + 1] & 15;
                var swRelVerIndex = payload[startIndex + 2];

                string swRelVarString;
                if (swRelVerIndex <= 15)
                {
                    swRelVarString = (swRelVerIndex & 255).ToString("X");
                }
                else
                {
                    swRelVarString = _swRelVer[swRelVerIndex - 16];
                }

                return side + "170XX" + _swVer[swVarIndex] + "0A" + _swYear[swYearIndex] + _swMonth[swMonthIndex] +
                       swRelVarString;
            }
            else
            {
                var swVar = payload[startIndex] == 0 ? "E" : "U";
                var swYearIndex = (payload[startIndex + 1] & 240) >> 4;
                var swMonthIndex = payload[startIndex + 1] & 15;
                var swRelVerIndex = payload[startIndex + 2];

                var pre = ActiveModel switch
                {
                    Models.BudsPlus => "175XX",
                    Models.BudsLive => "180XX",
                    Models.BudsPro => "190XX",
                    _ => "???XX"
                };

                return side + pre + swVar + "0A" + _swYear[swYearIndex] + _swMonth[swMonthIndex] +
                       _swRelVer[swRelVerIndex];
            }
        }
    }
}