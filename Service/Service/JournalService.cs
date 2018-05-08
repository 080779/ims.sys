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
    public class JournalService : IJournalService
    {
        public JournalDTO ToDTO(JournalEntity entity)
        {
            JournalDTO dto = new JournalDTO();
            dto.Amount = entity.Amount;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.FromPlatformUserId = entity.FormPlatformUserId;
            dto.FromPlatformUserMobile = entity.FormPlatformUser.Mobile;
            dto.Id = entity.Id;
            dto.InIntegral = entity.InIntegral;
            dto.Integral = entity.Integral;
            dto.IntegralTypeId = entity.IntegralTypeId;
            dto.JournalTypeId = entity.JournalTypeId;
            dto.JournalTypeName = entity.JournalType.Name;
            dto.OutIntegral = entity.OutIntegral;
            dto.PlatformUserId = entity.PlatformUserId;
            dto.PlatformUserMobile = entity.PlatformUser.Mobile;
            dto.Tip = entity.Tip;
            dto.ToPlatformUserId = entity.ToPlatformUserId;
            dto.ToPlatformUserMobile = entity.ToPlatformUser.Mobile;
            return dto;
        }
        public async Task<JournalSearchResult> GetModelList(long? id, long? typeId, string mobile, string code, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                JournalSearchResult result = new JournalSearchResult();
                var journals = dbc.GetAll<JournalEntity>();
                if(id!=null)
                {
                    journals = journals.Where(j=>j.PlatformUserId==id);
                }
                if (typeId != null)
                {
                    journals = journals.Where(j => j.JournalTypeId == typeId);
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    journals = journals.Where(j => j.PlatformUser.Mobile == mobile);
                }
                if (!string.IsNullOrEmpty(code))
                {
                    journals = journals.Where(j => j.PlatformUser.Code == code);
                }
                if(startTime!=null)
                {
                    journals = journals.Where(j => j.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    journals = journals.Where(j => j.CreateTime <= endTime);
                }
                long givingIntegralId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(i => i.Name == "商家积分").Id;
                long useIntegralId = dbc.GetAll<IntegralTypeEntity>().SingleOrDefault(i => i.Name == "消费积分").Id;                
                result.TotalCount = await journals.LongCountAsync();
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == "PlatformUser201805051709360001");
                if(user!=null)
                {
                    long platformUserId = user.Id;
                    result.GivingIntegrals = await journals.Where(j => j.ToIntegralTypeId == givingIntegralId && j.PlatformUserId == platformUserId).SumAsync(j => j.OutIntegral);
                    result.UseIntegrals = await journals.Where(j => j.ToIntegralTypeId == useIntegralId && j.PlatformUserId == platformUserId).SumAsync(j => j.OutIntegral);
                }
                var journalResult= await journals.OrderByDescending(j => j.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Journals = journalResult.Select(j => ToDTO(j)).ToArray();
                return result;
            }
        }
    }
}
