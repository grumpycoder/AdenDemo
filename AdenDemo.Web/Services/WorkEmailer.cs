using AdenDemo.Web.Helpers;
using AdenDemo.Web.Models;
using FluentEmail;
using System;

namespace AdenDemo.Web.Services
{
    public static class WorkEmailer
    {
        public static void Send(WorkItem workItem, Submission submission)
        {
            Email.DefaultRenderer = new RazorRenderer();
            var templatePath = @"C:\source\AdenDemo\AdenDemo.Web\Templates\WorkItemEmailTemplate.cshtml";
            var sender = "noreplay@mail.com";
            var subject = $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment";

            var taskIcon = Constants.TaskIcon;

            //switch (workItem.WorkItemAction)
            //{

            //    case WorkItemAction.Review:
            //        taskIcon = Constants.AcceptIcon;
            //        break;
            //    case WorkItemAction.Approve:
            //        taskIcon = Constants.ApprovalIcon;
            //        break;
            //    case WorkItemAction.Submit:
            //        taskIcon = Constants.SubmitIcon;
            //        break;
            //    case WorkItemAction.SubmitWithError:
            //        taskIcon = Constants.ErrorIcon;
            //        break;
            //    case WorkItemAction.ReviewError:
            //        taskIcon = Constants.ErrorIcon;
            //        break;
            //    default:
            //        break;
            //}

            var model = new EmailModel()
            {
                WorkItemAction = workItem.WorkItemAction.GetDescription(),
                Notes = workItem.Description,
                DueDate = submission.DueDate ?? DateTime.Now,
                FileName = submission.FileSpecification.FileDisplayName,
                Icon = taskIcon
            };
            var email = Email
                .From(sender, sender)
                .To(workItem.AssignedUser)
                .Subject(subject)
                .BodyAsHtml()
                .Body("")
                //.UsingTemplateEngine(new RazorRenderer())
                .UsingTemplateFromFile(templatePath, model)
                .Send();
        }

    }

    public class EmailModel
    {
        public string WorkItemAction { get; set; }
        public string Notes { get; set; }
        public DateTime DueDate { get; set; }
        public string FileName { get; set; }
        public string Icon { get; set; }
    }

}
