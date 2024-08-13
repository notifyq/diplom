using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace api_CodeFlow.Model
{
    public partial class StatusType
    {
        public StatusType()
        {
            Statuses = new HashSet<Status>();
        }

        public int StatusTypeId { get; set; }
        public string StatusTypeName { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Status> Statuses { get; set; }
    }
}
