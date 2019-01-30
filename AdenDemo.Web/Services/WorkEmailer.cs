using AdenDemo.Web.Controllers.api;
using AdenDemo.Web.Helpers;
using AdenDemo.Web.Models;
using Alsde.Extensions;
using FluentEmail;
using Humanizer;
using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace AdenDemo.Web.Services
{
    public static class WorkEmailer
    {
        public static void Send(WorkItem workItem, Submission submission, HttpPostedFileBase[] files = null)
        {

            Email.DefaultRenderer = new RazorRenderer();

            var sender = Constants.ReplyAddress;
            var templatePath = Constants.WorkItemTemplatePath;
            var taskIcon = Constants.TaskIcon;
            var subject = string.Empty;


            if (workItem.WorkItemAction == 0)
            {
                subject = $"{submission.FileSpecification.FileDisplayName} Submission Successful";
                templatePath = Constants.SubmissionTemplatePath;
                taskIcon = Constants.SuccessIcon;
            }

            if (workItem.WorkItemAction == WorkItemAction.ReviewError)
            {
                taskIcon = Constants.ErrorIcon;
            }

            if (submission.SubmissionState == SubmissionState.NotStarted)
            {
                subject = $"{submission.FileSpecification.FileDisplayName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled";
                templatePath = Constants.CancelTemplatePath;
                taskIcon = Constants.CancelledIcon;
            }

            if (string.IsNullOrWhiteSpace(subject)) subject = $"{submission.FileSpecification.FileDisplayName} {workItem.WorkItemAction.GetDisplayName()} Assignment";

            var model = new EmailModel()
            {
                WorkItemAction = workItem.WorkItemAction != 0 ? workItem.WorkItemAction.GetDescription() : "",
                Notes = workItem.Description ?? string.Empty,
                DueDate = submission.NextDueDate ?? submission.DueDate ?? DateTime.Now,
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
                //.Send();
                ;

            if (files != null)
            {
                foreach (var file in files)
                {
                    file.InputStream.Position = 0;
                    email.Message.Attachments.Add(new Attachment(file.InputStream, file.FileName));
                }
            }

            email.Send();

        }

        public static void SendRequest(UpdateGroupMemberDto model)
        {
            Email.DefaultRenderer = new RazorRenderer();

            var sender = Constants.ReplyAddress;
            var templatePath = Constants.UserRequestTemplatePath;
            var subject = "Add Aden Group members";
            var body =
                $"Please ADD user {model.Email} to group <br />{Regex.Replace(model.GroupName.Humanize().ToTitleCase().TrimEnd('s'), " App ", " ", RegexOptions.ExplicitCapture)}";

            var email = Email
                .From(sender, sender)
                .To("helpdesk@alsde.edu")
                .Subject(subject)
                .BodyAsHtml()
                .Body("")
                //.UsingTemplateEngine(new RazorRenderer())
                .UsingTemplateFromFile(templatePath, model)
            //.Send();
            ;


            email.Send();
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
