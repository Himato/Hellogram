using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace HelloGram.Persistence.Exceptions
{
    public static class DbEntityValidationExceptionHandler
    {
        public static string GetExceptionMessage(DbEntityValidationException exception)
        {
            return exception.EntityValidationErrors.FirstOrDefault()?.ValidationErrors.FirstOrDefault()?.ErrorMessage ?? "";
        }
    }
}