using Ezenity_Backend.Models.Common;
using Ezenity_Backend.Models.Common.Accounts;
using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Models
{
    public class DeleteResponse : IDeleteResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public DateTime DeletedAt { get; set; }
        public IAccountResponse DeletedBy { get; set; }
        public string ResourceId { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }

        public DeleteResponse()
        {
            Errors = new List<string>();
        }
    }
}
