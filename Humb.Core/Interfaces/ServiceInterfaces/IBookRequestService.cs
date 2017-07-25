using Humb.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Interfaces.ServiceInterfaces
{
    public interface IBookRequestService
    {
        bool CanSendRequest(int bookId, int requestingUserId, int respondingUserId);
        bool CanAnswerRequest(int bookId, int requestingUserId, int respondingUserId);
        void AddRequest(int bookId, int requestingUserId, int respondingUserID, int requestType);
        IEnumerable<BookRequestDTO> GetBookRequests(int bookId);
        IEnumerable<BookRequestDTO> GetUsersBookRequests(int bookId, int userId);
    }
}
