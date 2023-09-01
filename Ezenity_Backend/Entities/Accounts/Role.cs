using Ezenity_Backend.Entities.Common;

namespace Ezenity_Backend.Entities.Accounts
{
    public class Role : IRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
