using System.Web.Mvc;

namespace MeetingManagement.Areas.BCN_Khoa
{
    public class BCN_KhoaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BCN_Khoa";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BCN_Khoa_default",
                "BCN_Khoa/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}