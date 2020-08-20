using DNNrocketAPI;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RocketEcommerce;
using DNNrocketAPI.Componants;
using RocketEcommerce.Interfaces;
using RocketEcommerce.Componants;

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
                rPost.Url = payData.ReturnUrl + "?key=" + paymentData.PaymentKey;
                paymentData.Paid(true); // manual payment always true.
                paymentData.Update("ManualPay");
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
            // NOT REQUIRED
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
