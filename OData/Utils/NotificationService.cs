using OData.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OData.Utils
{
    public static class NotificationService
    {
        public static void NewUser(ODataDbContext context, UserResource new_user)
        {
            var n = new NotificationResource
            {
                CreatedAt = DateTime.Now,
                HasRead = 0
            };
            n.Text = string.Format("Зарегистрирован новый пользователь, имя: {0}, логин: {1}, возраст: {2} лет", new_user.Fullname, new_user.Name, new_user.Age);
            context.NotificationResources.Add(n);
            context.SaveChanges();
        }
    }
}
