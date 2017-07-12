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

namespace Humb.Test
{
    [TestClass]
    public class UserControllerTest
    {
        public IRepository<User> _userRepository;
        public IRepository<BlockUser> _blockedUsersRepository;
        private IUserService us;
        [TestInitialize]
        public void Initialize()
        {
            var userRepMock = new Mock<Repository<User>>();
            userRepMock.Setup(m => m.GetAll()).Returns(new[]
            {
                new User {NameSurname = "burakCinar", Password = "asdf", Email = "burak" },
                new User {NameSurname = "osman mahmutogly", Password = "asdf", Email = "osmn" },
                new User {NameSurname = "nediyemmahmtur mu", Password = "asdf", Email = "mahmut" }
            }.AsQueryable());
            _userRepository = userRepMock.Object;

            var blockedUsersMock = new Mock<Repository<BlockUser>>();
            _blockedUsersRepository = blockedUsersMock.Object;
             us = new UserService(_userRepository, _blockedUsersRepository);
        }
        [TestMethod]
        public void CanAddObjectsToDb()
        {
            us.CreateUser("brak", "beni", "bura");
            _userRepository.Count();
            _userRepository.Save();
            Assert.IsInstanceOfType(_userRepository.FindSingleBy(x => x.NameSurname == "bura"), typeof(User));
        }
        [TestMethod]
        public void GetUserByEmail()
        {
            Assert.AreEqual(_userRepository.FindSingleBy(x=>x.Email == "burak"), us.GetUser("burak"));            
        }
        [TestMethod]
        public void CanBlockUser()
        {
            User u1 = us.GetUser("burak");
            User u2 = us.GetUser("osmn");
            us.BlockUser(u1.Id, u2.Id);
            Assert.AreEqual(_blockedUsersRepository.FindSingleBy(x => x.FromUserId == u1.Id), u1);
        }
    }
}
