using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Humb.Core.Interfaces.ServiceInterfaces;

namespace Humb.API.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            if (userService == null)
                throw new ArgumentNullException("userService is null");
            this._userService = userService;
        }

        [HttpGet]
        public string RegisterUser()
        {
            _userService.CreateUser("bab", "asdfasdf", "dsafsdgsdf");
            return null;
        }
        [HttpGet]
        public string BlockUser()
        {
            _userService.BlockUser(1, 2);
            return null;
        }
        [HttpGet]
        public int GetTotalUserCount()
        {
            return _userService.GetTotalUserCount();
        }
        [HttpGet]
        public string ChangeUserPassword()
        {
            return _userService.ChangeUserPassword("bab", "121334");
        }

    }
}
