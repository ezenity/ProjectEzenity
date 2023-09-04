using Ezenity_Backend.Models.Common.Accounts;
using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Models.Common
{
    public interface IDeleteResponse
    {
        string Message { get; set; }
        int StatusCode { get; set; }
        DateTime DeletedAt { get; set; }
        IAccountResponse DeletedBy { get; set; }
        string ResourceId { get; set; }
        bool IsSuccess { get; set; }
        List<string> Errors { get; set; }
    }
}
