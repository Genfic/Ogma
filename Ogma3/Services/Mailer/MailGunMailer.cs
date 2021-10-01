using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace Ogma3.Services.Mailer;

public class MailGunMailer : IEmailSender
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly MailGunOptions _options;
        
    public MailGunMailer(IHttpClientFactory httpClientFactory, IOptions<MailGunOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = _httpClientFactory.CreateClient();
        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_options.MailGunKey}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
            
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["from"] = $"Genfic <mailgun@{_options.MailGunDomain}>",
            ["to"] = email,
            ["subject"] = subject,
            ["html"] = htmlMessage,
        });

        var response = await client.PostAsync($"https://api.mailgun.net/v3/{_options.MailGunDomain}/messages", content);

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Error trying to send email to [{To}], with subject [{Subject}] and body [{Body}]. Status: [{Status}]", email, subject, htmlMessage, response.StatusCode);
        }
    }
}