using ModniteServer.API.OAuth;

namespace ModniteServer.API
{
    /// <summary>
    /// Provides a base implementation of an API controller.
    /// </summary>
    public abstract class Controller : ControllerCore
    {
        protected bool Authorize()
        {
            return true;

            //string authorization = Request.Headers["Authorization"];

            //if (authorization != null && authorization.StartsWith("bearer "))
            //{
            //    string token = authorization.Split(' ')[1];

            //    if (OAuthManager.IsTokenValid(token))
            //        return true;
            //}

            //Response.StatusCode = 403;
            //return false;
        }
    }
}