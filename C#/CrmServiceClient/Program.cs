using System;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace CrmServiceClientExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Connect to CDS
            string connectionString = ConfigurationManager.AppSettings["organizationUrl"].ToString(); ;
            CrmServiceClient conn = new CrmServiceClient(connectionString);
            IOrganizationService orgService = (IOrganizationService)conn.OrganizationWebProxyClient;

            //Get Id of current user
            WhoAmIResponse whoami = (WhoAmIResponse)orgService.Execute(new WhoAmIRequest());

            //Get current system user
            Entity user = orgService.Retrieve("systemuser", whoami.UserId, new ColumnSet(true));

            //Write out all fields and values
            foreach (KeyValuePair<string, object> kp in user.Attributes)
            {
                if (kp.Value.GetType() == typeof(EntityReference))
                    Console.WriteLine($"{kp.Key} : {((EntityReference)kp.Value).LogicalName}({((EntityReference)kp.Value).Id})");
                else if (kp.Value.GetType() == typeof(OptionSetValue))
                    Console.WriteLine($"{kp.Key} : {user.FormattedValues[kp.Key]}");
                else
                    Console.WriteLine($"{kp.Key} : {kp.Value}");
            }

            Console.ReadLine();
        }
    }
}
