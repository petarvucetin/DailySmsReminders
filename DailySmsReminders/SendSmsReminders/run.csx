#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using Twilio;

public static void Run(TimerInfo myTimer, CloudTable table, TraceWriter log)
{
    var accountSid = Environment.GetEnvironmentVariable("TwilioAccountSid");
    var authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
    var phone = Environment.GetEnvironmentVariable("TwilioPhoneNumber");
    var msg = Environment.GetEnvironmentVariable("ReminderMessage");
    var twilio = new TwilioRestClient(accountSid, authToken);

    var query = new TableQuery<Reminder>().Where(
        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Reminders")
    );

    foreach (Reminder reminder in table.ExecuteQuery(query))
    {
        var message = twilio.SendMessage(phone,reminder.RowKey, msg);
    }
}

public class Reminder : TableEntity
{

}