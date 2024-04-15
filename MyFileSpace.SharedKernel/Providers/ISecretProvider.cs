using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSpace.SharedKernel.Providers
{
    public interface ISecretProvider
    {
        Task<string> GetSecret(string secretName);
    }
}
