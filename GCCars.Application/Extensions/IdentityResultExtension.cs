using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace GCCars.Application.Extensions
{
    public static class IdentityResultExtension
    {
        public static string GetErrorsMessage(this IEnumerable<IdentityError> errors)
        {
            return string.Join("; ", errors.GetErrorMessages());
        }

        public static IEnumerable<string> GetErrorMessages(this IEnumerable<IdentityError> errors)
        {
            return errors.Select(r => $"{r.Code} - {r.Description}");
        }
    }
}
