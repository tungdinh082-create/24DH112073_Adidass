using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace _24DH112073_MyStore.Areas.Admin.Controllers
{
    // Đây là Controller cơ sở
    // TẤT CẢ controller admin khác (Products, Orders...) sẽ kế thừa từ nó
    // NGOẠI TRỪ AuthController
    public class BaseController : Controller
    {
        // Hàm này sẽ chạy TRƯỚC BẤT KỲ Action nào trong Controller kế thừa nó
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra xem Session["AdminUser"] có tồn tại không
            if (Session["AdminUser"] == null)
            {
                // Nếu không có Session (chưa đăng nhập)
                // Chuyển hướng về trang Đăng nhập
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "Auth",
                        action = "Login",
                        area = "Admin"
                    }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}