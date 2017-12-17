using Microsoft.AspNetCore.Mvc;
using baka.Models;
using baka.Models.Entity;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace baka.Controllers
{
    [Route("/api/users")]
    public class UsersApiController : BakaController
    {
        [Route("from-email/{email}")]
        public async Task<IActionResult> GetUserFromEmail(string email)
        {
            AuthModel model = Authorize("su_full");
            if (!model.Authorized)
            {
                Response.StatusCode = 401;

                return Json(new
                {
                    success = false,
                    error = model.Reason,
                    code = 401
                });
            }

            try
            {
                using (var context = new BakaContext())
                {
                    BakaUser return_usr = await context.Users.Include(x => x.Links).Include(x => x.Files).FirstOrDefaultAsync(x => x.Email == email);

                    if (return_usr == null)
                        return NotFound(new { success = false, error = "404 Not Found", code = 404 });

                    return Json(new
                    {
                        id = return_usr.Id,
                        username = return_usr.Username,
                        name = return_usr.Name,
                        email = return_usr.Email,
                        upload_limit = return_usr.UploadLimitMB,
                        initial_ip = return_usr.InitialIp,
                        timestamp = return_usr.Timestamp.ToFileTimeUtc().ToString(),
                        token = return_usr.Token,
                        deleted = return_usr.Deleted,
                        disabled = return_usr.Disabled,
                        account_type = return_usr.AccountType,
                        links = return_usr.Links.ToList(),
                        files = return_usr.Files.ToList(),
                    });
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;

                if (!Globals.Config.IsDebug)
                    return Json(new
                    {
                        success = false,
                        error = "500 Internal Server Error",
                        code = 500
                    });

                return Json(new
                {
                    success = false,
                    error = "500 Internal Server Error",
                    code = 500,
                    exception = e.ToString(),
                });
            }
        }

        [Route("from-email/{id}")]
        public async Task<IActionResult> GetUserFromId(int email)
        {
            AuthModel model = Authorize("su_full");
            if (!model.Authorized)
            {
                Response.StatusCode = 401;

                return Json(new
                {
                    success = false,
                    error = model.Reason,
                    code = 401
                });
            }

            try
            {
                using (var context = new BakaContext())
                {
                    BakaUser return_usr = await context.Users.Include(x => x.Links).Include(x => x.Files).FirstOrDefaultAsync(x => x.Id == id);

                    if (return_usr == null)
                        return NotFound(new { success = false, error = "404 Not Found", code = 404 });

                    return Json(new
                    {
                        id = return_usr.Id,
                        username = return_usr.Username,
                        name = return_usr.Name,
                        email = return_usr.Email,
                        upload_limit = return_usr.UploadLimitMB,
                        initial_ip = return_usr.InitialIp,
                        timestamp = return_usr.Timestamp.ToFileTimeUtc().ToString(),
                        token = return_usr.Token,
                        deleted = return_usr.Deleted,
                        disabled = return_usr.Disabled,
                        account_type = return_usr.AccountType,
                        links = return_usr.Links.ToList(),
                        files = return_usr.Files.ToList(),
                    });
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;

                if (!Globals.Config.IsDebug)
                    return Json(new
                    {
                        success = false,
                        error = "500 Internal Server Error",
                        code = 500
                    });

                return Json(new
                {
                    success = false,
                    error = "500 Internal Server Error",
                    code = 500,
                    exception = e.ToString(),
                });
            }
        }

        [Route("create-user")]
        [AcceptVerbs("POST")]
        public async Task<IActionResult> CreateUser([FromBody] NewUserModel details)
        {
            try
            {
                AuthModel model = Authorize("su_full");
                if (!model.Authorized)
                {
                    Response.StatusCode = 401;

                    return Json(new
                    {
                        success = false,
                        error = model.Reason,
                        code = 401
                    });
                }

                BakaUser return_usr;

                using (var context = new BakaContext())
                {
                    var usr = new BakaUser()
                    {
                        Name = details.Name,
                        Username = details.Username,
                        Email = details.Email,
                        InitialIp = null,
                        Timestamp = DateTime.Now,
                        UploadLimitMB = details.UploadLimit,
                        Deleted = false,
                        Disabled = false,
                        AccountType = "su_upload"
                    };

                    usr.Token = Globals.GenerateToken(usr);

                    await context.Users.AddAsync(usr);
                    await context.SaveChangesAsync();

                    return_usr = usr;
                }

                return Json(new
                {
                    id = return_usr.Id,
                    username = return_usr.Username,
                    name = return_usr.Name,
                    initial_ip = return_usr.InitialIp,
                    timestamp = return_usr.Timestamp.ToFileTimeUtc().ToString(),
                    token = return_usr.Token,
                    deleted = return_usr.Deleted,
                    disabled = return_usr.Disabled,
                    email = return_usr.Email,
                    upload_limit = return_usr.UploadLimitMB,
                });
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;

                if (!Globals.Config.IsDebug)
                    return Json(new
                    {
                        success = false,
                        error = "500 Internal Server Error",
                        code = 500
                    });

                return Json(new
                {
                    success = false,
                    error = "500 Internal Server Error",
                    code = 500,
                    exception = e.ToString(),
                });
            }
        }

        [Route("{token}")]
        [AcceptVerbs("GET")]
        public async Task<IActionResult> GetUserInfo(string token)
        {
            try
            {
                AuthModel model = Authorize("su_full");
                if (!model.Authorized)
                {
                    Response.StatusCode = 401;

                    return Json(new
                    {
                        success = false,
                        error = model.Reason,
                        code = 401
                    });
                }

                using (var context = new BakaContext())
                {
                    BakaUser return_usr = await context.Users.Include(x => x.Links).Include(x => x.Files).FirstOrDefaultAsync(x => x.Token == token);

                    if (return_usr == null)
                        return NotFound(new { success = false, error = "404 Not Found", code = 404 });

                    return Json(new
                    {
                        id = return_usr.Id,
                        username = return_usr.Username,
                        name = return_usr.Name,
                        email = return_usr.Email,
                        upload_limit = return_usr.UploadLimitMB,
                        initial_ip = return_usr.InitialIp,
                        timestamp = return_usr.Timestamp.ToFileTimeUtc().ToString(),
                        token = return_usr.Token,
                        deleted = return_usr.Deleted,
                        disabled = return_usr.Disabled,
                        account_type = return_usr.AccountType,
                        links = return_usr.Links.ToList(),
                        files = return_usr.Files.ToList(),
                    });
                }
            }
            catch
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    error = "500 Internal Server Error",
                    code = 500
                });
            }
        }

        [Route("{token}")]
        [Route("delete/{token}")]
        [AcceptVerbs("POST")]
        public async Task<IActionResult> DeleteUser(string token)
        {
            try
            {
                AuthModel model = Authorize("su_full");
                if (!model.Authorized)
                {
                    Response.StatusCode = 401;

                    return Json(new
                    {
                        success = false,
                        error = model.Reason,
                        code = 401
                    });
                }
                using (var context = new BakaContext())
                {
                    BakaUser return_usr = await context.Users.FirstOrDefaultAsync(x => x.Token == token);

                    if (return_usr == null)
                        return NotFound(new { success = false, error = "404 Not Found", code = 404 });

                    if (!Globals.Config.PreserveDeletedFiles)
                        context.Users.Remove(return_usr);
                    else
                        return_usr.Deleted = true;

                    await context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        code = 200,
                        deleted = true
                    });
                }


            }
            catch
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    error = "500 Internal Server Error",
                    code = 500
                });
            }
        }

        [Route("disable/{token}")]
        [AcceptVerbs("POST")]
        public async Task<IActionResult> DisableUser(string token)
        {
            try
            {
                AuthModel model = Authorize("su_full");
                if (!model.Authorized)
                {
                    Response.StatusCode = 401;

                    return Json(new
                    {
                        success = false,
                        error = model.Reason,
                        code = 401
                    });
                }

                using (var context = new BakaContext())
                {
                    BakaUser return_usr = await context.Users.FirstOrDefaultAsync(x => x.Token == token);

                    if (return_usr == null)
                        return NotFound(new { success = false, error = "404 Not Found", code = 404 });

                    return_usr.Disabled = true;
                    await context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        code = 200,
                        disabled = true
                    });
                }
            }
            catch
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    error = "500 Internal Server Error",
                    code = 500
                });
            }
        }

        [Route("reset-token/{token}")]
        [AcceptVerbs("POST")]
        public async Task<IActionResult> ResetUserToken(string token)
        {
            try
            {
                AuthModel model = Authorize("su_full");
                if (!model.Authorized)
                {
                    Response.StatusCode = 401;

                    return Json(new
                    {
                        success = false,
                        error = model.Reason,
                        code = 401
                    });
                }

                using (var context = new BakaContext())
                {
                    BakaUser return_usr = await context.Users.FirstOrDefaultAsync(x => x.Token == token);

                    if (return_usr == null)
                        return NotFound(new
                        {
                            success = false,
                            error = "404 Not Found",
                            code = 404
                        });

                    return_usr.Token = Globals.GenerateToken(return_usr);

                    await context.SaveChangesAsync();

                    return Json(new
                    {
                        new_token = return_usr.Token
                    });
                }
            }
            catch
            {
                Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    error = "500 Internal Server Error",
                    code = 500
                });
            }
        }
    }
}