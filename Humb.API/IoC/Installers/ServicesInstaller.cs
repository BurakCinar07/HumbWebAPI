using Castle.MicroKernel.Registration;
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
using Humb.Core.Interfaces;
using Humb.Service.Services.EmailService;
using Humb.Service.Services.EmailService.EmailDispatchers;
using Humb.Service.Services.PushNotificationService;

namespace Humb.API.IoC.Installers
{
    public class ServicesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IUserService>().ImplementedBy<UserService>().LifestyleSingleton());
            container.Register(Component.For<IBookTransactionService>().ImplementedBy<BookTransactionService>().LifestyleSingleton());
            container.Register(Component.For<IBookInteractionService>().ImplementedBy<BookInteractionService>().LifestyleSingleton());
            container.Register(Component.For<IEmailService>().ImplementedBy<EmailService>().LifestyleSingleton());
            container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).LifestylePerWebRequest());
            container.Register(Component.For<IDbContext>().ImplementedBy<EFDbContext>().LifestylePerWebRequest());
            container.Register(Component.For<IEmailGeneratorFactory>().ImplementedBy<EmailGeneratorFactory>().LifestyleSingleton());
            container.Register(Component.For<IEmailSender>().ImplementedBy<SmtpEmailSender>().LifestyleSingleton());
        }
    }
}