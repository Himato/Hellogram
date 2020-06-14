using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using HelloGram.Persistence;
using HelloGram.Persistence.Exceptions;
using Microsoft.AspNet.Identity;

namespace HelloGram.Controllers.Api
{
    public class ApiHelper : ApiController
    {
        protected readonly Repository Repository;

        public ApiHelper()
        {
            Repository = new Repository();
        }

        protected IHttpActionResult Put(Func<string, dynamic> func)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var output = func(userId);
                Repository.Complete();
                
                return Ok(output.Id);
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
            catch (DbEntityValidationException exception)
            {
                return BadRequest(DbEntityValidationExceptionHandler.GetExceptionMessage(exception));
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        protected IHttpActionResult Put(Action<string> action)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                action(userId);
                Repository.Complete();
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
            catch (DbEntityValidationException exception)
            {
                return BadRequest(DbEntityValidationExceptionHandler.GetExceptionMessage(exception));
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected IHttpActionResult Post(Func<string, dynamic> func)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var output = func(userId);
                Repository.Complete();
                
                return Ok(output.Id);
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
            catch (DbEntityValidationException exception)
            {
                return BadRequest(DbEntityValidationExceptionHandler.GetExceptionMessage(exception));
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        protected IHttpActionResult Delete(Action<string> action)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                action(userId);
                Repository.Complete();
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}