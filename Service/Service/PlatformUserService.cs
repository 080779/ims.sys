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
    public class PlatformUserService : IPlatformUserService
    {
        public PlatformUserDTO ToDTO(PlatformUserEntity entity)
        {
            PlatformUserDTO dto = new PlatformUserDTO();
            dto.Code = entity.Code;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.ErrorCount = entity.ErrorCount;
            dto.ErrorTime = entity.ErrorTime;
            dto.GivingIntegral = entity.GivingIntegral;
            dto.Id = entity.Id;
            dto.IsEnabled = entity.IsEnabled;
            dto.Mobile = entity.Mobile;
            dto.PlatformIntegral = entity.PlatformIntegral;
            dto.PlatformUserTypeId = entity.PlatformUserTypeId;
            dto.PlatformUserTypeName = entity.PlatformUserType.Name;
            dto.UseIntegral = entity.UseIntegral;
            return dto;
        }

        public async Task<long> AddAsync(string typeName, string mobile, string code, string password, string tradePassword)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var type = await dbc.PlatformUserTypes.SingleOrDefaultAsync(p => p.Name == typeName);
                if(type==null)
                {
                    return -1;
                }
                PlatformUserEntity user = new PlatformUserEntity();
                if(typeName=="平台")
                {
                    user.PlatformUserTypeId = type.Id;
                    user.Mobile = mobile;
                    user.Code = code;
                    user.Password = password;
                    user.TradePassword = tradePassword;
                }
                else if(typeName=="商家")
                {
                    user.PlatformUserTypeId = type.Id;
                    user.Mobile = mobile;
                    user.Code = code;
                    user.Salt = CommonHelper.GetCaptcha(4);
                    user.Password = CommonHelper.GetMD5(password + user.Salt);
                    user.TradePassword = CommonHelper.GetMD5(tradePassword + user.Salt);
                }
                else
                {
                    user.PlatformUserTypeId = type.Id;
                    user.Mobile = mobile;
                    user.Code = code;
                    user.Salt = CommonHelper.GetCaptcha(4);
                    user.Password = password;
                    user.TradePassword = CommonHelper.GetMD5(tradePassword + user.Salt);
                }
                dbc.PlatformUsers.Add(user);
                await dbc.SaveChangesAsync();
                return user.Id;
            }
        }

        public async Task<bool> Del(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if(user==null)
                {
                    return false;
                }
                user.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<PlatformUserDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user= await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (user == null)
                {
                    return null;
                }
                return ToDTO(user);
            }
        }

        public async Task<PlatformUserDTO> GetModelAsync(string type,string str)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                PlatformUserEntity user;
                if (type=="mobile")
                {
                    user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == str);
                }
                else
                {
                    user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Code == str);
                }
                if(user==null)
                {
                    return null;
                }
                return ToDTO(user);
            }
        }

        public async Task<PlatformUserSearchResult> GetModelListAsync(string mobile, string code,string type, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                PlatformUserSearchResult result = new PlatformUserSearchResult();
                var users=dbc.GetAll<PlatformUserEntity>();
                if(!string.IsNullOrEmpty(mobile))
                {
                    users = users.Where(u=>u.Mobile==mobile);
                }
                if (!string.IsNullOrEmpty(code))
                {
                    users = users.Where(u => u.Code == code);
                }
                if (!string.IsNullOrEmpty(type))
                {
                    users = users.Where(u => u.PlatformUserType.Name == type);
                }
                if (startTime!=null)
                {
                    users = users.Where(u => u.CreateTime>=startTime);
                }
                if (endTime != null)
                {
                    users = users.Where(u => u.CreateTime <= endTime);
                }
                result.TotalCount = users.LongCount();
                var usersRes = await users.OrderByDescending(u => u.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.PlatformUsers = usersRes.Select(u => ToDTO(u)).ToArray();
                return result;
            }
        }

        public async Task<bool> Frozen(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (user == null)
                {
                    return false;
                }
                user.IsEnabled = false;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> CheckLoginAsync(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == mobile);
                if(user==null)
                {
                    return -1;
                }
                if (user.PlatformUserType.Name != "商家")
                {
                    return -2;
                }
                if (user.ErrorCount >= 5)
                {
                    return -3;
                }
                if (user.IsEnabled==false)
                {
                    return -4;
                }
                if (user.Password != CommonHelper.GetMD5(password + user.Salt))
                {
                    return -5;
                }
                return user.Id;
            }
        }

        public async Task<bool> CheckTradePasswordAsync(long id, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Id == id);
                if (user == null)
                {
                    return false;
                }                
                if (user.TradePassword != CommonHelper.GetMD5(password + user.Salt))
                {
                    return false;
                }
                return true;
            }
        }

        public async Task<bool> IsExist(string type, string str)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                PlatformUserEntity entity;
                if (type == "mobile")
                {
                    entity = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Mobile == str);
                    if(entity==null)
                    {
                        return false;
                    }
                }
                else
                {
                    entity = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Code == str);
                    if (entity == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public async Task<bool> ProvideAsync(long userId, long toUserId, long integral, string typeName,string toTypeName, string description,string tip)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var type = await dbc.GetAll<IntegralTypeEntity>().SingleOrDefaultAsync(i => i.Name == typeName);
                var toType = await dbc.GetAll<IntegralTypeEntity>().SingleOrDefaultAsync(i => i.Name == toTypeName);
                if (type == null)
                {
                    return false;
                }
                if(toType==null)
                {
                    return false;
                }
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p=>p.Id==userId);
                if(user==null)
                {
                    return false;
                }
                var toUser = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Id == toUserId);
                if(toUser==null)
                {
                    return false;
                }
                if(user.Code == "CodePlatformUser201805051709360001" && type.Name =="平台积分")
                {
                    if(toType.Name== "平台积分")
                    {
                        toUser.PlatformIntegral = toUser.PlatformIntegral + integral;
                        JournalEntity journal = new JournalEntity();
                        journal.Description = description;
                        journal.InIntegral = integral;
                        journal.Integral = toUser.PlatformIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToIntegralTypeId = toType.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.FormPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.Tip = tip;
                        journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(journal);
                    }
                    else if(toType.Name == "商家积分" && toUser.PlatformUserType.Name == "商家")
                    {
                        if(user.PlatformIntegral < integral)
                        {
                            return false;
                        }
                        user.PlatformIntegral = user.PlatformIntegral - integral;
                        JournalEntity journal = new JournalEntity();
                        journal.Description = description;
                        journal.OutIntegral = integral;
                        journal.Integral = user.PlatformIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToIntegralTypeId = toType.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.FormPlatformUserId = user.Id;
                        journal.PlatformUserId = user.Id;
                        journal.Tip = tip;
                        journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(journal);

                        toUser.GivingIntegral = toUser.GivingIntegral + integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.Description = description;
                        toJournal.InIntegral = integral;
                        toJournal.Integral = toUser.GivingIntegral;
                        toJournal.IntegralTypeId = type.Id;
                        toJournal.ToIntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.FormPlatformUserId = user.Id;
                        toJournal.Tip = tip;
                        toJournal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else if (toType.Name == "消费积分" && toUser.PlatformUserType.Name == "商家")
                    {
                        if (user.PlatformIntegral < integral)
                        {
                            return false;
                        }
                        user.PlatformIntegral = user.PlatformIntegral - integral;
                        JournalEntity journal = new JournalEntity();
                        journal.Description = description;
                        journal.OutIntegral = integral;
                        journal.Integral = user.PlatformIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToIntegralTypeId = toType.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.FormPlatformUserId = user.Id;
                        journal.Tip = tip;
                        journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.Description = description;
                        toJournal.InIntegral = integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = type.Id;
                        toJournal.ToIntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.FormPlatformUserId = user.Id;
                        toJournal.Tip = tip;
                        toJournal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else if (toType.Name == "消费积分" && toUser.PlatformUserType.Name == "客户")
                    {
                        if (user.PlatformIntegral < integral)
                        {
                            return false;
                        }
                        user.PlatformIntegral = user.PlatformIntegral - integral;
                        JournalEntity journal = new JournalEntity();
                        journal.Description = description;
                        journal.OutIntegral = integral;
                        journal.Integral = user.PlatformIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToIntegralTypeId = toType.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.FormPlatformUserId = user.Id;
                        journal.Tip = tip;
                        journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.Description = description;
                        toJournal.InIntegral = integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = type.Id;
                        toJournal.ToIntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.FormPlatformUserId = user.Id;
                        toJournal.Tip = tip;
                        toJournal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else
                    {
                        return false;
                    }
                }
                else if(type.Name == "商家积分" && user.PlatformUserType.Name=="商家")
                {
                    if(toType.Name=="消费积分" && toUser.PlatformUserType.Name=="商家" && user!=toUser)
                    {
                        if (user.GivingIntegral < integral)
                        {
                            return false;
                        }
                        user.GivingIntegral = user.GivingIntegral - integral;
                        JournalEntity journal = new JournalEntity();
                        journal.Description = description;
                        journal.OutIntegral = integral;
                        journal.Integral = user.GivingIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToIntegralTypeId = toType.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.FormPlatformUserId = user.Id;
                        journal.Tip = tip;
                        journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.Description = description;
                        toJournal.InIntegral = integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = type.Id;
                        toJournal.ToIntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.FormPlatformUserId = user.Id;
                        toJournal.Tip = tip;
                        toJournal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else if (toType.Name == "消费积分" && toUser.PlatformUserType.Name == "客户")
                    {
                        if (user.GivingIntegral < integral)
                        {
                            return false;
                        }
                        user.GivingIntegral = user.GivingIntegral - integral;
                        JournalEntity journal = new JournalEntity();
                        journal.Description = description;
                        journal.OutIntegral = integral;
                        journal.Integral = user.GivingIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToIntegralTypeId = toType.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.FormPlatformUserId = user.Id;
                        journal.Tip = tip;
                        journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.Description = description;
                        toJournal.InIntegral = integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = type.Id;
                        toJournal.ToIntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.FormPlatformUserId = user.Id;
                        toJournal.Tip = tip;
                        toJournal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else
                    {
                        return false;
                    }
                }
                else if(user.PlatformUserType.Name=="客户" && type.Name=="消费积分")
                {
                    if(toUser.PlatformUserType.Name=="商家")
                    {
                        if (user.UseIntegral < integral)
                        {
                            return false;
                        }
                        user.UseIntegral = user.UseIntegral - integral;
                        JournalEntity journal = new JournalEntity();
                        journal.Description = description;
                        journal.OutIntegral = integral;
                        journal.Integral = user.UseIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToIntegralTypeId = type.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.FormPlatformUserId = user.Id;
                        journal.Tip = tip;
                        journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.Description = description;
                        toJournal.InIntegral = integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = type.Id;
                        toJournal.ToIntegralTypeId = type.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.FormPlatformUserId = user.Id;
                        toJournal.Tip = tip;
                        toJournal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> TakeOutAsync(long userId, long integral, string typeName, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var type = await dbc.GetAll<IntegralTypeEntity>().SingleOrDefaultAsync(i => i.Name == typeName);
                if(type==null)
                {
                    return false;
                }
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Id == userId);
                if (user == null)
                {
                    return false;
                }
                if(user.PlatformUserType.Name=="商家" && type.Name=="商家积分")
                {
                    if(user.GivingIntegral<integral)
                    {
                        return false;
                    }
                    user.GivingIntegral = user.GivingIntegral - integral;
                    JournalEntity journal = new JournalEntity();
                    journal.Description = description;
                    journal.OutIntegral = integral;
                    journal.Integral = user.GivingIntegral;
                    journal.IntegralTypeId = type.Id;
                    journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                    journal.PlatformUserId = user.Id;
                    journal.FormPlatformUserId = user.Id;
                    journal.ToPlatformUserId = user.Id;
                    journal.ToIntegralTypeId = type.Id;
                    dbc.Journals.Add(journal);
                }
                else if(user.PlatformUserType.Name == "商家" && type.Name == "消费积分")
                {
                    if (user.UseIntegral < integral)
                    {
                        return false;
                    }
                    user.UseIntegral = user.UseIntegral - integral;
                    JournalEntity journal = new JournalEntity();
                    journal.Description = description;
                    journal.OutIntegral = integral;
                    journal.Integral = user.UseIntegral;
                    journal.IntegralTypeId = type.Id;
                    journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                    journal.PlatformUserId = user.Id;
                    journal.FormPlatformUserId = user.Id;
                    journal.ToPlatformUserId = user.Id;
                    journal.ToIntegralTypeId = type.Id;
                    dbc.Journals.Add(journal);
                }
                else if(user.PlatformUserType.Name == "客户" && type.Name == "消费积分")
                {
                    if (user.UseIntegral < integral)
                    {
                        return false;
                    }
                    user.UseIntegral = user.UseIntegral - integral;
                    JournalEntity journal = new JournalEntity();
                    journal.Description = description;
                    journal.OutIntegral = integral;
                    journal.Integral = user.UseIntegral;
                    journal.IntegralTypeId = type.Id;
                    journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == description).Id;
                    journal.PlatformUserId = user.Id;
                    journal.FormPlatformUserId = user.Id;
                    journal.ToPlatformUserId = user.Id;
                    journal.ToIntegralTypeId = type.Id;
                    dbc.Journals.Add(journal);
                }
                else
                {
                    return false;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> TakeCashApplyAsync(long userId, long integral, string typeName, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var type = await dbc.GetAll<IntegralTypeEntity>().SingleOrDefaultAsync(i => i.Name == typeName);
                if (type == null)
                {
                    return false;
                }
                var user = await dbc.GetAll<PlatformUserEntity>().SingleOrDefaultAsync(p => p.Id == userId);
                if (user == null)
                {
                    return false;
                }
                if (user.PlatformUserType.Name == "商家" && type.Name == "商家积分")
                {
                    if (user.GivingIntegral < integral)
                    {
                        return false;
                    }
                    TakeCashEntity takeCash = new TakeCashEntity();
                    takeCash.Description = description;
                    takeCash.Amount = integral * (Convert.ToDecimal(dbc.GetAll<SettingEntity>().SingleOrDefault(s => s.Description == "商家积分提现比率").Name));
                    takeCash.Integral = integral;
                    takeCash.IntegralTypeId = type.Id;
                    takeCash.PlatformUserId = user.Id;
                    takeCash.StateId = dbc.GetAll<StateEntity>().SingleOrDefault(s => s.Description == "未转账").Id;
                    dbc.TakeCashes.Add(takeCash);
                }
                else if (user.PlatformUserType.Name == "商家" && type.Name == "消费积分")
                {
                    if (user.UseIntegral < integral)
                    {
                        return false;
                    }
                    TakeCashEntity takeCash = new TakeCashEntity();
                    takeCash.Description = description;
                    takeCash.Amount = integral * (Convert.ToDecimal(dbc.GetAll<SettingEntity>().SingleOrDefault(s => s.Description == "消费积分提现比率").Name));
                    takeCash.Integral = integral;
                    takeCash.IntegralTypeId = type.Id;
                    takeCash.PlatformUserId = user.Id;
                    takeCash.StateId = dbc.GetAll<StateEntity>().SingleOrDefault(s => s.Description == "未转账").Id;
                    dbc.TakeCashes.Add(takeCash);
                }
                else if (user.PlatformUserType.Name == "客户" && type.Name == "消费积分")
                {
                    if (user.UseIntegral < integral)
                    {
                        return false;
                    }
                    TakeCashEntity takeCash = new TakeCashEntity();
                    takeCash.Description = description;
                    takeCash.Amount = integral * (Convert.ToDecimal(dbc.GetAll<SettingEntity>().SingleOrDefault(s => s.Description == "消费积分提现比率").Name));
                    takeCash.Integral = integral;
                    takeCash.IntegralTypeId = type.Id;
                    takeCash.PlatformUserId = user.Id;
                    takeCash.StateId = dbc.GetAll<StateEntity>().SingleOrDefault(s => s.Description == "未转账").Id;
                    dbc.TakeCashes.Add(takeCash);
                }
                else
                {
                    return false;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> TakeCashConfirmAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var takeCash = await dbc.GetAll<TakeCashEntity>().SingleOrDefaultAsync(i => i.Id == id);
                if(takeCash==null)
                {
                    return false;
                }
                takeCash.StateId = dbc.GetAll<StateEntity>().SingleOrDefault(s => s.Description == "已转账").Id;
                var user = dbc.GetAll<PlatformUserEntity>().SingleOrDefault(p => p.Id == takeCash.PlatformUserId);
                if(user==null)
                {
                    return false;
                }
                if(takeCash.IntegralType.Name=="商家积分" && user.PlatformUserType.Name=="商家")
                {
                    user.GivingIntegral = user.GivingIntegral - takeCash.Integral;

                    JournalEntity journal = new JournalEntity();
                    journal.Description = takeCash.Description;
                    journal.OutIntegral = takeCash.Integral;
                    journal.Integral = user.GivingIntegral;
                    journal.IntegralTypeId = takeCash.IntegralTypeId;
                    journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == takeCash.Description).Id;
                    journal.PlatformUserId = user.Id;
                    journal.FormPlatformUserId = user.Id;
                    journal.ToPlatformUserId = user.Id;
                    journal.ToIntegralTypeId = takeCash.IntegralTypeId;
                    journal.Amount = takeCash.Amount;
                    dbc.Journals.Add(journal);

                }
                else if(takeCash.IntegralType.Name == "消费积分" && user.PlatformUserType.Name == "商家")
                {
                    user.UseIntegral = user.UseIntegral - takeCash.Integral;

                    JournalEntity journal = new JournalEntity();
                    journal.Description = takeCash.Description;
                    journal.OutIntegral = takeCash.Integral;
                    journal.Integral = user.UseIntegral;
                    journal.IntegralTypeId = takeCash.IntegralTypeId;
                    journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == takeCash.Description).Id;
                    journal.PlatformUserId = user.Id;
                    journal.FormPlatformUserId = user.Id;
                    journal.ToPlatformUserId = user.Id;
                    journal.ToIntegralTypeId = takeCash.IntegralTypeId;
                    journal.Amount = takeCash.Amount;
                    dbc.Journals.Add(journal);
                }
                else if(takeCash.IntegralType.Name == "消费积分" && user.PlatformUserType.Name == "客户")
                {
                    user.UseIntegral = user.UseIntegral - takeCash.Integral;

                    JournalEntity journal = new JournalEntity();
                    journal.Description = takeCash.Description;
                    journal.OutIntegral = takeCash.Integral;
                    journal.Integral = user.UseIntegral;
                    journal.IntegralTypeId = takeCash.IntegralTypeId;
                    journal.JournalTypeId = dbc.GetAll<JournalTypeEntity>().SingleOrDefault(j => j.Description == takeCash.Description).Id;
                    journal.PlatformUserId = user.Id;
                    journal.FormPlatformUserId = user.Id;
                    journal.ToPlatformUserId = user.Id;
                    journal.ToIntegralTypeId = takeCash.IntegralTypeId;
                    journal.Amount = takeCash.Amount;
                    dbc.Journals.Add(journal);
                }
                else
                {
                    return false;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
