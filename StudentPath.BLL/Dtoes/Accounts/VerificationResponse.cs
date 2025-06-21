using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes.Accounts
{
    public class VerificationResponse
    {
        [JsonPropertyName("match")]
        public bool Match { get; set; }
    }
}
