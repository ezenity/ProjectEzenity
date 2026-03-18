using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Domain.Entities.Vault;

public class MissionRequirement
{
    public Guid Id { get; set; }

    public Guid MissionId { get; set; }
    public Mission Mission { get; set; } = null!;

    public MissionRequirementType RequirementType { get; set; }

    public string? Value { get; set; } // store as string for flexibility
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

}
