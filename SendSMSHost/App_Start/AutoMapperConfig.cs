using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SendSMSHost.Models;
using AutoMapper;

namespace SendSMSHost.App_Start
{
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Sms, SmsDTO>();
                cfg.CreateMap<SmsDTO, Sms>();
                cfg.CreateMap<Contact, ContactDTO>();
                cfg.CreateMap<ContactDTO, Contact>();
                cfg.CreateMap<Status, StatusDTO>();
                cfg.CreateMap<StatusDTO, Status>();
            });
        }
    }
}