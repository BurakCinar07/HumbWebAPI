using Humb.Core.Entities;
using Humb.Core.Interfaces.RepositoryInterfaces;
using Humb.Core.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Service.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUserService _userService;
        private readonly IRepository<Feedback> _feedbackRepository;
        public FeedbackService(IUserService userService, IRepository<Feedback> feedbackRepo)
        {
            _userService = userService;
            _feedbackRepository = feedbackRepo;
        }

        public void AddFeedback(string email, string feedback)
        {
            Feedback fb = new Feedback()
            {
                CreatedAt = DateTime.Now,
                IsChecked = false,
                Text = feedback,
                UserId = _userService.GetUserId(email)
            };
            _feedbackRepository.Insert(fb);
        }
    }
}

