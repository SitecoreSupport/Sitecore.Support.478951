using Sitecore.Modules.EmailCampaign.Core;
using Sitecore.Modules.EmailCampaign.Core.Pipelines.DispatchNewsletter;
using Sitecore.Modules.EmailCampaign.Messages;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;
using System;

namespace Sitecore.Support.Modules.EmailCampaign.Core.Pipelines.DispatchNewsletter
{
    public class MoveToSent
    {
        public void Process(DispatchNewsletterArgs args)
        {
            if (args.SendingAborted && !string.IsNullOrEmpty(args.ProcessData.Errors))
            {
                return;
            }

            if (!args.IsTestSend && !args.DedicatedInstance && (args.SendingAborted || args.RequireFinalMovement))
            {
                if (args.Message.MessageType != MessageType.Trickle)
                {
                    using (new SecurityDisabler())
                    {
                        args.Message.Source.ReleaseRelatedItems();
                        args.Message.Source.State = MessageState.Sent;
                        args.Message.Source.EndTime = System.DateTime.Now;
                        ExternalLinks.DatabaseToItem(args.Message);
                    }
                }
            }
        }
    }
}