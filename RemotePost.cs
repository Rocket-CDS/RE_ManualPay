﻿using Simplisity;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace RocketEcommerceAPI.RE_ManualPay
{
    public class RemotePost
    {
        private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
        public string Url = "";
        public string Method = "POST";
        public string FormName = "form";
        public void Add(string name, string value)
        {
            Inputs.Add(name, value);
        }

        public string GetPostHtml()
        {
            string sipsHtml = "";

            sipsHtml += "<html><head>";
            sipsHtml += "<link rel='stylesheet' href='/DesktopModules/DNNrocket/css/w3.css'>";
            sipsHtml += "<link rel='stylesheet' href='/DesktopModules/DNNrocket/fa/css/all.min.css'>";
            //sipsHtml += "</head><body onload=\"document." + FormName + ".submit()\">";
            sipsHtml += "</head><body>";
            sipsHtml += "  <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" height=\"100%\">";
            sipsHtml += "<tr><td width=\"100%\" height=\"100%\" valign=\"middle\" align=\"center\">";
            sipsHtml += "<font style=\"font-family: Trebuchet MS, Verdana, Helvetica;font-size: 14px;letter-spacing: 1px;font-weight: bold;\">";
            sipsHtml += "Processing...";
            sipsHtml += "</font><br /><br /><i class='fa fa-spinner fa-spin' style='font-size:48px'></i>     ";
            sipsHtml += "</td></tr>";
            sipsHtml += "</table>";

            sipsHtml += "<script>";
            sipsHtml += "$(document).ready(function () {";
            sipsHtml += "window.location.replace('" + Url + "');; ";
            sipsHtml += "});";
            sipsHtml += "</script>";


            sipsHtml += "</body></html>";

            return sipsHtml;

        }

    }


}
