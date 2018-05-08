using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class AdminService : IAdminService
    {
        public AdminDTO ToDTO(AdminEntity entity)
        {
            AdminDTO dto = new AdminDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Mobile = entity.Mobile;
            dto.IsEnabled = entity.IsEnabled;
            dto.PermissionIds = entity.Permissions.Select(p => p.Id).ToArray();
            return dto;
        }
        public async Task<long> AddAsync(string mobile, string description, string password, long[] permissionIds)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AdminEntity entity = new AdminEntity();
                entity.Mobile = mobile;
                entity.Description = description;
                entity.Salt = CommonHelper.GetCaptcha(4);
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                dbc.Admins.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }
        public async Task<bool> UpdateAsync(long id, string mobile, string description, string password, long[] permissionIds)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if(entity==null)
                {
                    return false;
                }
                entity.Mobile = mobile;
                entity.Description = description;
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<AdminDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<AdminSearchResult> GetModelListAsync(string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AdminSearchResult result = new AdminSearchResult();
                var admins = dbc.GetAll<AdminEntity>();
                if(!string.IsNullOrEmpty(mobile))
                {
                    admins = admins.Where(a => a.Mobile == mobile);
                }
                if(startTime!=null)
                {
                    admins = admins.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    admins = admins.Where(a => a.CreateTime <= endTime);
                }
                result.TotalCount = await admins.LongCountAsync();
                var adminsResult= await admins.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Admins = adminsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public bool HasPermission(long id, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var admin = dbc.GetAll<AdminEntity>().SingleOrDefault(a => a.Id == id);
                if(admin==null)
                {
                    return false;
                }
                return admin.Permissions.Any(p => p.Description.Contains(description));
            }
        }
    }
}
