using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Generic;

namespace RocketEcommerce.RE_ManualPay
{
    public class StartConnect : APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private CommandSecurity _commandSecurity;
        private RocketInterface _rocketInterface;
        private string _currentLang;
        private Dictionary<string, string> _passSettings;
        private SystemLimpet _systemData;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return ERROR if not matching commands.
            var rtnDic = new Dictionary<string, object>();

            paramCmd = paramCmd.ToLower();

            _systemData = new SystemLimpet(systemInfo);
            _rocketInterface = new RocketInterface(interfaceInfo);

            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _currentLang = langRequired;
            if (_currentLang == "") _currentLang = DNNrocketUtils.GetCurrentCulture();

            _commandSecurity = new CommandSecurity(-1, -1, _rocketInterface);
            _commandSecurity.AddCommand("manualpay_edit", true);
            _commandSecurity.AddCommand("manualpay_save", true);
            _commandSecurity.AddCommand("manualpay_delete", true);

            if (!_commandSecurity.HasSecurityAccess(paramCmd))
            {
                strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
            }
            else
            {
                switch (paramCmd)
                {
                    case "manualpay_edit":
                        strOut = EditData();
                        break;
                    case "manualpay_save":
                        SaveData();
                        strOut = EditData();
                        break;
                    case "manualpay_delete":
                        DeleteData();
                        strOut = EditData();
                        break;
                }
            }

            if (!rtnDic.ContainsKey("outputjson")) rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public String EditData()
        {
            var payData = new PayData(PortalUtils.SiteGuid());

            var razorTempl = RenderRazorUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, _currentLang, _rocketInterface.ThemeVersion, true);
            var strOut = RenderRazorUtils.RazorDetail(razorTempl, payData.Info, _passSettings, new SessionParams(_paramInfo), true);
            return strOut;
        }
        public void SaveData()
        {
            var payData = new PayData(PortalUtils.SiteGuid());
            payData.Save(_postInfo);
        }
        public void DeleteData()
        {
            var payData = new PayData(PortalUtils.SiteGuid());
            payData.Delete();
        }



    }
}
