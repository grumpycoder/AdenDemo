﻿using AdenDemo.Web.Helpers;
using AdenDemo.Web.Models;
using AdenDemo.Web.ViewModels;
using AutoMapper;

namespace AdenDemo.Web.Data.Profiles
{
    public class WorkItemProfile : Profile
    {
        public WorkItemProfile()
        {

            CreateMap<WorkItem, WorkItemHistoryDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.AssignedDate, opt => opt.MapFrom(s => s.AssignedDate))
                .ForMember(d => d.AssignedUser, opt => opt.MapFrom(s => s.AssignedUser))
                .ForMember(d => d.WorkItemState, opt => opt.MapFrom(s => s.WorkItemState))
                .ForMember(d => d.CompletedDate, opt => opt.MapFrom(s => s.CompletedDate))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.Action, opt => opt.MapFrom(s => s.WorkItemAction))
                .ForAllOtherMembers(d => d.Ignore())
                ;

            CreateMap<WorkItem, AssignmentDto>()
                .ForMember(d => d.WorkItemAction, opt => opt.MapFrom(s => s.WorkItemAction.GetDisplayName()))
                .ForMember(d => d.AssignedUser, opt => opt.MapFrom(s => s.AssignedUser))
                .ForMember(d => d.WorkItemId, opt => opt.MapFrom(s => s.Id))
                .ForAllOtherMembers(d => d.Ignore());

        }
    }
}