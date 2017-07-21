﻿using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Humb.Core.Interfaces.ServiceInterfaces;
using Humb.Service.Services;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Data;
using Humb.Service.Services.EmailService.EmailDispatchers;
using Humb.Service.Services.EmailService;

namespace Humb.API.IoC.Installers
{
    public class ServicesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IUserService>().ImplementedBy<UserService>().LifestylePerWebRequest());
            container.Register(Component.For<IBookTransactionService>().ImplementedBy<BookTransactionService>().LifestylePerWebRequest());
            container.Register(Component.For<IBookInteractionService>().ImplementedBy<BookInteractionService>().LifestylePerWebRequest());
            container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).LifestylePerWebRequest());
            container.Register(Component.For<IDbContext>().ImplementedBy<EFDbContext>().LifestylePerWebRequest());
        }
    }
}