#r "Microsoft.WindowsAzure.Storage"
#r "System.Runtime"

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Text;
using Twilio.TwiML;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, CloudTable table, TraceWriter log)
{
    var data = await req.Content.ReadAsStringAsync();

    var formValues = data.Split('&')
        .Select(value => value.Split('='))
      .ToDictionary(pair => System.Uri.UnescapeDataString(pair[0]),
             pair => System.Uri.UnescapeDataString(pair[1]));

    var body = formValues["Body"];
    var phoneNumber = formValues["From"];
    var response = new TwilioResponse();

    if (body.Trim('+').ToLower() == "subscribe")
    {
        // User is subscribing
        TableOperation operation = TableOperation.Retrieve<Reminder>("Reminders", phoneNumber);
        TableResult result = table.Execute(operation);

        if (result.Result != null)
        {
            response.Message("You already subscribed!");
        }
        else
        {
            response.Message("Thank you, you are now subscribed. Reply 'STOP' to stop receiving updates.");
            var insertOperation = TableOperation.Insert(
              new Reminder()
              {
                  PartitionKey = "Reminders",
                  RowKey = phoneNumber
              }
            );

            table.Execute(insertOperation);
        }
    }
    else
    {
        response.Message("Welcome to Daily Updates. Text 'Subscribe' receive updates.");
    }

    return new HttpResponseMessage
    {
        Content = new StringContent(response.ToString(), System.Text.Encoding.UTF8, "application/xml")
    };
}

public class Reminder : TableEntity
{
}