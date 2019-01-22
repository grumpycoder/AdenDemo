﻿using AdenDemo.Web.Models;
using AdenDemo.Web.ViewModels;
using AutoMapper;
using Humanizer;

namespace AdenDemo.Web.Data.Profiles
{
    public class FileSpecificationProfile : Profile
    {
        public FileSpecificationProfile()
        {
            CreateMap<FileSpecification, FileSpecificationViewDto>();

            CreateMap<FileSpecification, UpdateFileSpecificationDto>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.FileName))
                .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.FileNumber))
                .ForMember(d => d.Section, opt => opt.MapFrom(s => s.Section))
                .ForMember(d => d.Application, opt => opt.MapFrom(s => s.Application))
                .ForMember(d => d.DataGroups, opt => opt.MapFrom(s => s.DataGroups))
                .ForMember(d => d.Collection, opt => opt.MapFrom(s => s.Collection))
                .ForMember(d => d.SupportGroup, opt => opt.MapFrom(s => s.SupportGroup))
                .ForMember(d => d.GenerationUserGroup, opt => opt.MapFrom(s => s.GenerationUserGroup))
                .ForMember(d => d.ApprovalUserGroup, opt => opt.MapFrom(s => s.ApprovalUserGroup))
                .ForMember(d => d.SubmissionUserGroup, opt => opt.MapFrom(s => s.SubmissionUserGroup))
                .ForMember(d => d.FileNameFormat, opt => opt.MapFrom(s => s.FileNameFormat))
                .ForMember(d => d.ReportAction, opt => opt.MapFrom(s => s.ReportAction)).ReverseMap()
                .ForAllOtherMembers(d => d.Ignore()); ;

            //CreateMap<FileSpecification, FileSpecificationViewDto>()
            //    .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            //    .ForMember(d => d.FileName, opt => opt.MapFrom(src => src.FileName))
            //    .ForMember(d => d.FileNumber, opt => opt.MapFrom(src => src.FileNumber))
            //    .ForMember(d => d.IsRetired, opt => opt.MapFrom(src => src.IsRetired))
            //    .ForMember(d => d.Section, opt => opt.MapFrom(src => src.Section))
            //    .ForMember(d => d.SupportGroup, opt => opt.MapFrom(src => src.SupportGroup))
            //    .ForMember(d => d.Application, opt => opt.MapFrom(src => src.Application))
            //    .ForMember(d => d.Collection, opt => opt.MapFrom(src => src.Collection))
            //    .ForMember(d => d.GenerationUserGroup, opt => opt.MapFrom(src => src.GenerationUserGroup))
            //    .ForMember(d => d.ApprovalUserGroup, opt => opt.MapFrom(src => src.ApprovalUserGroup))
            //    .ForMember(d => d.SubmissionUserGroup, opt => opt.MapFrom(src => src.SubmissionUserGroup))
            //    .ForMember(d => d.ReportAction, opt => opt.MapFrom(src => src.ReportAction))


            //    //.ForMember(d => d.GenerationUserGroup, opt => opt.MapFrom(src => src.GenerationUserGroup))
            //    //.ForAllOtherMembers(opts => opts.Ignore());
            //    ;
            //CreateMap<UpdateFileSpecificationDto, FileSpecification>()
            //    .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            //    .ForMember(d => d.FileName, opt => opt.MapFrom(s => s.FileName))
            //    .ForMember(d => d.FileNumber, opt => opt.MapFrom(s => s.FileNumber))
            //    .ForMember(d => d.Section, opt => opt.MapFrom(s => s.Section))
            //    .ForMember(d => d.Application, opt => opt.MapFrom(s => s.Application))
            //    .ForMember(d => d.DataGroups, opt => opt.MapFrom(s => s.DataGroups))
            //    .ForMember(d => d.Collection, opt => opt.MapFrom(s => s.Collection))
            //    .ForMember(d => d.SupportGroup, opt => opt.MapFrom(s => s.SupportGroup))
            //    .ForMember(d => d.GenerationUserGroup, opt => opt.MapFrom(s => s.GenerationUserGroup))
            //    .ForMember(d => d.ApprovalUserGroup, opt => opt.MapFrom(s => s.ApprovalUserGroup))
            //    .ForMember(d => d.SubmissionUserGroup, opt => opt.MapFrom(s => s.SubmissionUserGroup))
            //    .ForMember(d => d.FileNameFormat, opt => opt.MapFrom(s => s.FileNameFormat))
            //    .ForMember(d => d.ReportAction, opt => opt.MapFrom(s => s.ReportAction)).ReverseMap()
            //    .ForAllOtherMembers(d => d.Ignore()); ;

        }

        public class NameConverter :
            ITypeConverter<string, string>
        {
            public string Convert(string source, string destination, ResolutionContext context)
            {
                return destination.Humanize(LetterCasing.Title);
            }
        }
    }
}