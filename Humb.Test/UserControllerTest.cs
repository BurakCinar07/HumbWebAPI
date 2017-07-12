using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace Humb.Test
{
    [TestClass]
    public class UserControllerTest
    {
        public IRepository<User> _userRepository;
        public IRepository<BlockUser> _blockedUsersRepository;
        public IRepository<ForgottenPassword> _passwordRepository;

        private IUserService us;
        List<User> users;
        [TestInitialize]
        public void Initialize()
        {
            users = new List<User>{
                new User {Id=0, Email = "asdfsadf", Password = "sdfsdczxcv", NameSurname = "burakkk" },
                new User {Id=1, Email = "zxcv", Password = "ret", NameSurname = "cinarr" },
                new User {Id=2, Email = "qwert", Password = "xcvb", NameSurname = "burakcinar" },
                new User {Id=3, Email = "tutyut", Password = "sdf1234143sdczxcv", NameSurname = "mahmut" }
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
            us = new UserService(mockUser.Object, mockBlock.Object, mockPassword.Object);

        }
        [TestMethod]
        public void CanAddObjectsToDb()
        {
            var x = _userRepository.FindSingleBy(y => y.NameSurname.StartsWith("b"));
            _userRepository.Delete(x);
        }
        [TestMethod]
        public void CanChangePassword()
        {
            var pas = us.ChangeUserPassword("zxcv", "1");
        }






    }
}
