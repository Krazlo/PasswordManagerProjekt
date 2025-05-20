using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwM_UI.Utility
{
    public class VaultDetails
    {
        public string VaultName { get; set; }
        public string SharedPath { get; set; }

        public override bool Equals(object obj)
        {
            return obj is VaultDetails details &&
                   VaultName == details.VaultName &&
                   SharedPath == details.SharedPath;
        }

        //"Magic Numbers" er autogenerede hashing multipliers
        public override int GetHashCode()
        {
            int hashCode = -1670917873;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(VaultName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SharedPath);
            return hashCode;
        }
    }
}
