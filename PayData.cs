using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketEcommerce.Componants;
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
        public PayData(string siteGuid)
        {
            var portalid = PortalUtils.GetPortalIdBySiteKey(siteGuid);
            PortalShop = new PortalShopLimpet(portalid, DNNrocketUtils.GetCurrentCulture());
            _guidKey = siteGuid + "_" + _systemKey + "_" + _entityTypeCode;
            _objCtrl = new DNNrocketController();
            Info = _objCtrl.GetData(_guidKey, _entityTypeCode, DNNrocketUtils.GetCurrentCulture(), -1, true, _tableName);
            if (Info == null)
            {
                var portalId = PortalUtils.GetPortalIdBySiteKey(siteGuid);
                Info = new SimplisityInfo();
                Info.TypeCode = _entityTypeCode;
                Info.GUIDKey = _guidKey;
                Info.PortalId = portalId;
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


        public PortalShopLimpet PortalShop { get; set; }
        public SimplisityInfo Info { get; set; }


    }
}
