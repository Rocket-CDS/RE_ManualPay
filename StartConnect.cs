using DNNrocketAPI;
using DNNrocketAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;

namespace RocketEcommerce.RE_ManualPay
{
    public class StartConnect : APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private SecurityLimpet _securityData;
        private RocketInterface _rocketInterface;
        private string _currentLang;
        private Dictionary<string, string> _passSettings;
        private SystemLimpet _systemData;
        private const string _systemkey = "rocketecommerce";
        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return ERROR if not matching commands.
            var rtnDic = new Dictionary<string, object>();

            paramCmd = paramCmd.ToLower();

            _systemData = new SystemLimpet(_systemkey);
            _rocketInterface = new RocketInterface(interfaceInfo);

            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _currentLang = langRequired;
            if (_currentLang == "") _currentLang = DNNrocketUtils.GetCurrentCulture();

            _securityData = new SecurityLimpet(PortalUtils.GetCurrentPortalId(), _systemData.SystemKey, _rocketInterface, -1, -1);
            _securityData.AddCommand("manualpay_edit", true);
            _securityData.AddCommand("manualpay_save", true);
            _securityData.AddCommand("manualpay_delete", true);

            paramCmd = _securityData.HasSecurityAccess(paramCmd, "manualpay_login");

            switch (paramCmd)
            {
                case "manualpay_login":
                    strOut = UserUtils.LoginForm(_systemkey, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                    break;
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

            if (!rtnDic.ContainsKey("outputjson")) rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public String EditData()
        {
            var payData = new PayData(PortalUtils.SiteGuid());
            var appThemeSystem = new AppThemeSystemLimpet("RE_ManualPay");
            var razorTempl = appThemeSystem.GetTemplate("settings.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, payData.Info, null, _passSettings, new SessionParams(_paramInfo), true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
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
