using DNNrocketAPI;
using DNNrocketAPI.Components;
using RocketEcommerce.Components;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketEcommerce.RE_ManualPay
{
    public class PayData
    {
        private const string _entityTypeCode = "MANUALPAY";
        private const string _tableName = "RocketEcommerce";
        private const string _systemKey = "rocketecommerce";
        private string _guidKey;
        private DNNrocketController _objCtrl;
        public PayData(int portalId, string cultureCode)
        {
            PortalShop = new PortalShopLimpet(portalId, cultureCode);
            _guidKey = portalId + "_" + _systemKey + "_" + _entityTypeCode;
            _objCtrl = new DNNrocketController();
            Info = _objCtrl.GetData(_guidKey, _entityTypeCode, cultureCode, -1, true, _tableName);
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.TypeCode = _entityTypeCode;
                Info.GUIDKey = _guidKey;
                Info.PortalId = portalId;
                Info.Lang = cultureCode;
            }
        }
        public void Save(SimplisityInfo postInfo)
        {
            Info.XMLData = postInfo.XMLData;
            _objCtrl.SaveData(Info, _tableName);
            LogUtils.LogTracking("Save - UserId: " + UserUtils.GetCurrentUserId() + " " + postInfo.XMLData, "RocketSystemPay");
        }
        public void Delete()
        {
            if (Info.ItemID > 0) _objCtrl.Delete(Info.ItemID);
        }
        public string ReturnUrl
        {
            get { return Info.GetXmlProperty("genxml/textbox/returnurl"); }
            set { Info.SetXmlProperty("genxml/textbox/returnurl", value); }
        }
        public string ReturnCommand
        {
            get { return Info.GetXmlProperty("genxml/textbox/returncommand"); }
            set { Info.SetXmlProperty("genxml/textbox/returncommand", value); }
        }
        public bool DebugMode
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/debugmode"); }
            set { Info.SetXmlProperty("genxml/checkbox/debugmode", value.ToString()); }
        }
        public bool Active
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/active"); }
            set { Info.SetXmlProperty("genxml/checkbox/active", value.ToString()); }
        }
        public bool ValidPayment
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/validpayment"); }
            set { Info.SetXmlProperty("genxml/checkbox/validpayment", value.ToString()); }
        }
        public bool PaymentFail
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/paymentfail"); }
            set { Info.SetXmlProperty("genxml/checkbox/paymentfail", value.ToString()); }
        }


        public PortalShopLimpet PortalShop { get; set; }
        public SimplisityInfo Info { get; set; }

        public string PayButtonText
        {
            get
            {
                var rtn = Info.GetXmlProperty("genxml/lang/genxml/textbox/paybuttontext");
                if (rtn == "") rtn = DNNrocketUtils.GetResourceString("/DesktopModules/DNNrocketModules/RE_ManualPay/App_LocalResources/", "provider.paybutton", "Text", "");
                return rtn;
            }
            set { Info.SetXmlProperty("genxml/lang/genxml/textbox/paybuttontext", value); }
        }
        public string PayMsg
        {
            get { return Info.GetXmlProperty("genxml/lang/genxml/textbox/paymsg"); }
            set { Info.SetXmlProperty("genxml/lang/genxml/textbox/paymsg", value); }
        }
        public string PaymentKey { get { return "manualpay"; } }

    }
}
