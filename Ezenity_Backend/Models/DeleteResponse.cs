using Ezenity_Backend.Models.Accounts;
using System;
using System.Collections.Generic;

namespace Ezenity_Backend.Models
{
    public class DeleteResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public DateTime DeletedAt { get; set; }
        public AccountResponse DeletedBy { get; set; }
        public string ResourceId { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }

        public DeleteResponse()
        {
            Errors = new List<string>();
        }
    }
}
