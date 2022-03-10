﻿using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Models;

namespace BoardMan.Web
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<DbPlan, Plan>();
            
            CreateMap<DbSubscription, Subscription>()                
                .ForMember(x => x.PlanName, x => x.MapFrom(d => d.PaymentTrasaction.Plan.Name));
            
            CreateMap<DbPaymentTransaction, PaymentTransaction>()
                .ForMember(x => x.TransactedBy, x => x.MapFrom(d => d.TransactedBy.UserName));

            CreateMap<PaymentTransaction, DbPaymentTransaction>()
                .ForMember(x => x.Plan, x => x.Ignore())
                .ForMember(x => x.PlanDiscount, x => x.Ignore())
                .ForMember(x => x.TransactedBy, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());

            CreateMap<DbBillingDetails, BillingDetails>()
                .ForMember(x => x.IsAnonymousUser, x => x.Ignore())
                .ForMember(x => x.Password, x => x.Ignore())
                .ForMember(x => x.ConfirmPassword, x => x.Ignore());

            CreateMap<BillingDetails, DbBillingDetails>()
                .ForMember(x => x.PaymentTransactionId, x => x.Ignore())
                .ForMember(x => x.PaymentTransaction, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());
        }
    }

}
