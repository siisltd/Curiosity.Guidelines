﻿namespace RazorHtmlEmails.AspNetCore.Views.Emails.ConfirmAccount
{
    public class ConfirmAccountEmailViewModel
    {
        public ConfirmAccountEmailViewModel(string confirmEmailUrl)
        {
            ConfirmEmailUrl = confirmEmailUrl;
        }

        public string ConfirmEmailUrl { get; set; }
    }
}
 