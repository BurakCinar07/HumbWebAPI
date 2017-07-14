using System;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Entities;
using Moq;
using System.Linq;
using Humb.API.Controllers;
using Humb.Service.Services;
using Humb.Data;
using Humb.Core.Interfaces.ServiceInterfaces;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq.Language.Flow;
using NUnit.Framework;
using Humb.Service.Providers;

namespace Humb.Test
{
    [TestFixture]
    public class UserControllerTest
    {
        public IRepository<User> _userRepository;
        public IRepository<Book> _bookRepository;

        public IRepository<BlockUser> _blockedUsersRepository;
        public IRepository<BookInteraction> _bookInteractionRepository;
        public IRepository<BookTransaction> _bookTransactionRepository;

        public IRepository<ForgottenPassword> _passwordRepository;

        private IUserService us;
        List<User> users;
        [SetUp]

        public void Initialize()
        {
            users = new List<User>{
                new User {Id=0, Email = "asdfsadf", Password = "sdfsdczxcv", NameSurname = "burakkk" },
                new User {Id=1, Email = "zxcv", Password = "ret", NameSurname = "cinarr" },
                new User {Id=2, Email = "qwert", Password = "xcvb", NameSurname = "burakcinar" },
                new User {Id=3, Email = "tutyut", Password = "sdf1234143sdczxcv", NameSurname = "mahmut" },
                new User {Id = 4, Email = "burakcinar07@gmail.com", Password = "1243", NameSurname = "burakCinar", FcmToken = "sdfasdf" }
            };

            Mock<IRepository<User>> mockUser = new Mock<IRepository<User>>();
            mockUser.Setup(m => m.GetAll()).Returns(users.AsQueryable());
            mockUser.Setup(m => m.Count()).Returns(users.Count());
            mockUser.Setup(r => r.GetById(It.IsAny<int>())).Returns((int i) => users.Where(u => u.Id == i).Single());
            mockUser.Setup(r => r.Insert(It.IsAny<User>())).Callback((User u) => users.Add(u));
            mockUser.Setup(r => r.FindBy(It.IsAny<Expression<Func<User, bool>>>())).Returns((Expression<Func<User, bool>> predicate) => users.AsQueryable().Where(predicate));
            mockUser.Setup(r => r.FindSingleBy(It.IsAny<Expression<Func<User, bool>>>())).Returns((Expression<Func<User, bool>> predicate) => users.AsQueryable().FirstOrDefault(predicate));
            mockUser.Setup(r => r.Delete(It.IsAny<User>())).Callback((User u) => users.Remove(u));
            _userRepository = mockUser.Object;
            Mock<IRepository<BlockUser>> mockBlock = new Mock<IRepository<BlockUser>>();
            _blockedUsersRepository = mockBlock.Object;
            Mock<IRepository<ForgottenPassword>> mockPassword = new Mock<IRepository<ForgottenPassword>>();
            _passwordRepository = mockPassword.Object;
            Mock<IRepository<BookInteraction>> mockInteractions = new Mock<IRepository<BookInteraction>>();
            _bookInteractionRepository = mockInteractions.Object;
            Mock<IRepository<BookTransaction>> mockTransactions = new Mock<IRepository<BookTransaction>>();
            _bookTransactionRepository = mockTransactions.Object;
            Mock<IRepository<Book>> mockBook = new Mock<IRepository<Book>>();
            _bookRepository = mockBook.Object;
            us = new UserService(mockUser.Object, _bookRepository, mockBlock.Object, mockPassword.Object, _bookTransactionRepository, _bookInteractionRepository, new EmailFactory(new SmtpEmailDispatcher()));
        }
        //[Test]
        public void ValueNullTest()
        {
            try
            {
                us.CreateUser("asdf", "qer", "qwer");
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(ArgumentNullException), e.GetType());
            }
        }
        //[Test]
        public void GetTotalUserCountTest()
        {
            Assert.AreEqual(4, us.GetTotalUserCount());
        }
        //[TestCase("burak", "1")]
        //[TestCase("", "")]        
        //[TestCase("qwert", "1")]
        public void ChangeUserPasswordTest(string email, string pass)
        {
            try
            {
                User u = users.FirstOrDefault(x => x.Email == email);

            }
            catch (Exception e)
            {
                Assert.Fail("null exception" + e.Message);

            }
        }
        [Test]        
        public void IsVerificationEmailSend()
        {
            us.ForgotPasswordRequest("burakcinar07@gmail.com");
        }
        //[Test]
        public void TransformDTO()
        {
            var x = us.GetFcmToken(4);
        }

    }
}
