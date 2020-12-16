using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RocketEcommerce;
using DNNrocketAPI.Components;
using RocketEcommerce.Interfaces;
using RocketEcommerce.Components;

namespace RocketEcommerce.RE_ManualPay
{
    public class BankInterface : PaymentInterface
    {
        public override string GetBankRemotePost(PaymentLimpet paymentData)
        {
            var systemData = new SystemLimpet("rocketecommerce");
            var rocketInterface = systemData.GetInterface(paymentData.PaymentProvider);
            if (rocketInterface != null)
            {
                var rPost = new RemotePost();
                var payData = new PayData(PortalUtils.SiteGuid());
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
        public override string NotifyEvent(RemoteLimpet remoteParam)
        {
            // NOT REQUIRED
            return "NOT REQUIRED";
        }
        public override void ReturnEvent(RemoteLimpet remoteParam)
        {
            try
            {
                var paymentKey = remoteParam.GetUrlParam("key");
                PaymentLimpet paymentData = new PaymentLimpet(PortalUtils.GetPortalId(), paymentKey);
                if (paymentData.Exists)
                {
                    var payData = new PayData(PortalUtils.SiteGuid());
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
            var systemData = new SystemLimpet("rocketecommerce");
            var rocketInterface = systemData.GetInterface("manualpay");
            if (rocketInterface != null)
            {
                var payData = new PayData(PortalUtils.SiteGuid());
                return payData.Active;
            }
            return false;
        }
    }
}
