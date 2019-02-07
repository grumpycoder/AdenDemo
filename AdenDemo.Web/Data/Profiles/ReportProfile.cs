using Aden.Web.ViewModels;
using AdenDemo.Web.Models;
using AdenDemo.Web.ViewModels;
using AutoMapper;

namespace AdenDemo.Web.Data.Profiles
{
    public class ReportProfile : Profile
    {

        public ReportProfile()
        {
            CreateMap<Report, ReportViewDto>()
               .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
               .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.Submission.FileSpecification.FileName))
               .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.Submission.FileSpecification.FileNumber))
               .ForMember(d => d.DataYear, opt => opt.MapFrom(s => s.Submission.FileSpecification.DataYear))
               .ForMember(d => d.ReportState, opt => opt.MapFrom(s => s.ReportState))
               .ForMember(d => d.ApprovedDate, opt => opt.MapFrom(s => s.ApprovedDate))
               .ForMember(d => d.DataYear, opt => opt.MapFrom(s => s.DataYear))
               .ForMember(d => d.GeneratedDate, opt => opt.MapFrom(s => s.GeneratedDate))
               .ForMember(d => d.SubmittedDate, opt => opt.MapFrom(s => s.SubmittedDate))
               .ForMember(d => d.Documents, opt => opt.MapFrom(s => s.Documents))
               .ForMember(d => d.SubmissionDueDate, opt => opt.MapFrom(s => s.Submission.NextDueDate ?? s.Submission.DueDate))
               ;

            CreateMap<ReportDocument, DocumentViewDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Filename, opt => opt.MapFrom(s => s.Filename))
                .ForMember(d => d.Version, opt => opt.MapFrom(s => s.Version))
                .ForMember(d => d.FileSize, opt => opt.MapFrom(s => s.FileSize))
                .ForMember(d => d.FileSizeInMb, opt => opt.MapFrom(s => s.FileSize))
                .ForMember(d => d.FileSizeMb, opt => opt.MapFrom(s => s.FileSize))
                ;
        }
    }

    //public class DocumentProfile : Profile
    //{
    //    public DocumentProfile()
    //    {
    //        CreateMap<ReportDocument, DocumentViewDto>()
    //            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
    //            .ForMember(d => d.Filename, opt => opt.MapFrom(s => s.Filename))
    //            .ForMember(d => d.Version, opt => opt.MapFrom(s => s.Version))
    //            .ForMember(d => d.FileSize, opt => opt.MapFrom(s => s.FileSize.ToFileSize()))
    //            .ForMember(d => d.FileSizeInMb, opt => opt.MapFrom(s => s.FileSize.ToFileSize()))
    //            .ForMember(d => d.FileSizeMb, opt => opt.MapFrom(s => s.FileSize.ConvertBytesToMega()))
    //            ;
    //    }

    //}
}
