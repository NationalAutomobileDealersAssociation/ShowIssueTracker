using ShowIssueTracker.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ShowIssueTracker.Repositories
{
    public class NadaRepository: INadaRepository
    {
        private readonly IHostEnvironment _env; 
        private readonly SendGridProperties _sendGrid;
        private readonly InternalProperties _internalProp;
        public NadaRepository(IHostEnvironment env,  IOptions<SendGridProperties> sendGrid, IOptions<InternalProperties> internalProp)
        {
            _env = env; 
            _sendGrid = sendGrid.Value;
            _internalProp = internalProp.Value;
        }
        public async Task<string> SendGridEmailAsync(string toAddress, string subject, string body, string ccAddress)
        {
            var apiKey = _sendGrid.ApiKey;//"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"; //
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(_sendGrid.FromEmailAddress);

            var plainTextContent = Regex.Replace(body, "<[^>]*>", "");

            var fromData = new EmailAddress(_sendGrid.FromEmailAddress, _sendGrid.FromName);

            var toEmails = new List<EmailAddress>();
            string[] toAddresses = toAddress.Split(';');
            foreach (string add in toAddresses)
            {
                if (add != "")
                {
                    toEmails.Add(new EmailAddress(add));
                }
            }
            string[] toccAddress = ccAddress.Split(';');
            foreach (string bccadd in toccAddress)
            {
                if (bccadd != "")
                {
                    toEmails.Add(new EmailAddress(bccadd));
                }
            }
            var msgNew = MailHelper.CreateSingleEmailToMultipleRecipients(from, toEmails, subject, plainTextContent, body);
            var responseNew = await client.SendEmailAsync(msgNew);
            var d =  SendEmailFromNADAAPI(ccAddress, toAddress, subject, body);
            return responseNew.StatusCode.ToString();
        }


        public async Task SendEmailFromNADAAPI(string ccAddress, string toAddress, string subject, string body)
        {

            var emailapi = "noreply@nada.org";
            var apipass = "Password1!";

            var client = new RestClientNew(
                   "https://api.nada.org/api/account/token",
                   HttpVerb.POST,
                   "{\"Email\":\"" + emailapi + "\",\"Password\":\"" +
                   apipass + "\"}");
            var json = await client.MakeRequest();
            dynamic tokenDetails = JsonConvert.DeserializeObject(json);

            var apiToken = tokenDetails.token;
            if (apiToken != "")
            {
                var emailTobeSent = "{\"ToEmail\":\"" + toAddress + "\",\"subject\":\"" + subject + "\",\"body\":\"" + body + "\",\"CCEmail\":\"" +
                                    ccAddress + "\"}";


                var headerDictionary = new Dictionary<string, string>
                {
                    { "Authorization", "Bearer "+ apiToken  }
                };
                client = new RestClientNew(
                "https://api.nada.org/api/nada/ComposeEmail",
                HttpVerb.POST,
                emailTobeSent,
                headerDictionary);


                var jsonEmailSent = await client.MakeRequest();
                dynamic emailReponse = JsonConvert.DeserializeObject(jsonEmailSent); 

            }





        }


    }
}
