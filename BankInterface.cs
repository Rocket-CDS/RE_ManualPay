using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RocketEcommerceAPI;
using DNNrocketAPI.Components;
using RocketEcommerceAPI.Interfaces;
using RocketEcommerceAPI.Components;

namespace RocketEcommerceAPI.RE_ManualPay
{
    public class BankInterface : PaymentInterface
    {
        public override string GetBankRemotePost(PaymentLimpet paymentData)
        {
            var systemData = new SystemLimpet("rocketecommerceapi");
            var rocketInterface = systemData.GetInterface(paymentData.PaymentProvider);
            if (rocketInterface != null)
            {
                var rPost = new RemotePost();
                var payData = new PayData(PortalUtils.GetCurrentPortalId(), DNNrocketUtils.GetCurrentCulture());
                var paramPrefix = "?";
                if (payData.ReturnUrl.Contains("?")) paramPrefix = "&";
                rPost.Url = payData.ReturnUrl + paramPrefix + "key=" + paymentData.PaymentKey + "&cmd=" + payData.ReturnCommand;
                paymentData.BankAction = PaymentAction.BankPost;  // flag it is action. (payment made by "ReturnEvent") 
                paymentData.Update("ManualPay");
                LogUtils.LogTracking("ManualPay PaymentId: " + paymentData.PaymentId + "  Key:" + paymentData.PaymentKey + " Name:" + paymentData.Name, systemData.SystemKey);

                //Build the re-direct html 
                //We redirect so the architechture of the internal payment provider matches external payment methods.
                return rPost.GetPostHtml();
            }
            return "";
        }
        public override string NotifyEvent(SimplisityInfo paramInfo)
        {
            // NOT REQUIRED
            return "NOT REQUIRED";
        }
        public override void ReturnEvent(SimplisityInfo paramInfo)
        {
            try
            {
                var paymentKey = paramInfo.GetXmlProperty("genxml/remote/urlparams/key");
                if (paymentKey == "") paymentKey = paramInfo.GetXmlProperty("genxml/urlparams/key");
                if (paymentKey == "") paymentKey = paramInfo.GetXmlProperty("genxml/hidden/key");
                PaymentLimpet paymentData = new PaymentLimpet(PortalUtils.GetPortalId(), paymentKey);
                if (paymentData.Exists)
                {
                    var payData = new PayData(PortalUtils.GetCurrentPortalId(), DNNrocketUtils.GetCurrentCulture());
                    if (payData.PaymentFail && payData.DebugMode)
                    {
                        paymentData.PaymentFailed();
                    }
                    else
                    {
                        if (payData.ValidPayment)
                        {
                            paymentData.Paid(true);
                        }
                        else
                        {
                            paymentData.Paid(false);
                            paymentData.ChangeStaus(PaymentStatus.WaitingForPayment);
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                LogUtils.LogException(exc);
            }
        }

        public override bool Active()
        {
            var systemData = new SystemLimpet("rocketecommerceapi");
            var rocketInterface = systemData.GetInterface("manualpay");
            if (rocketInterface != null)
            {
                var payData = new PayData(PortalUtils.GetCurrentPortalId(), DNNrocketUtils.GetCurrentCulture());
                return payData.Active;
            }
            return false;
        }
        public override string PayButtonText()
        {
            var portalShop = new PortalShopLimpet(PortalUtils.GetCurrentPortalId(), DNNrocketUtils.GetCurrentCulture());
            var payData = new PayData(PortalUtils.GetCurrentPortalId(), DNNrocketUtils.GetCurrentCulture());
            return payData.PayButtonText;
        }

        public override string PayMsg()
        {
            var portalShop = new PortalShopLimpet(PortalUtils.GetCurrentPortalId(), DNNrocketUtils.GetCurrentCulture());
            var payData = new PayData(PortalUtils.GetCurrentPortalId(), DNNrocketUtils.GetCurrentCulture());
            return payData.PayMsg;
        }

        public override string PaymentProvKey()
        {
            return "manualpay";
        }
    }
}
