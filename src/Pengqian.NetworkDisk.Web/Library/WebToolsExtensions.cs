using System;
using System.Security.Claims;
using Pengqian.NetworkDisk.Infrastructure.Enum;
using Pengqian.NetworkDisk.Public.ViewModel;

namespace Pengqian.NetworkDisk.Web.Library
{
    public static class WebToolsExtensions
    {
        public static VmUserInfo GetUserInfo(this ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated) return null;
            var userInfo = new VmUserInfo();
            foreach (var claim in principal.Claims)
            {
                var property = typeof(VmUserInfo).GetProperty(claim.Type);
                if (property == null) continue;
                if (property.PropertyType == typeof(DateTime?))
                {
                    property.SetValue(userInfo, DateTime.Parse(claim.Value));
                }
                else if (property.PropertyType == typeof(Permissions?))
                {
                    if (Enum.TryParse(claim.Value, out Permissions permissions))
                    {
                        property.SetValue(userInfo, permissions);
                    }
                }
                else
                {
                    typeof(VmUserInfo).GetProperty(claim.Type)?.SetValue(userInfo, claim.Value);
                }
            }
            return userInfo;
        }
    }
}