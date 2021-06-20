using System.Web.Mvc;

namespace MeetingManagement.Areas.HeadOfDepartment
{
    public class HeadOfDepartmentAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HeadOfDepartment";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HeadOfDepartment_default",
                "HeadOfDepartment/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}