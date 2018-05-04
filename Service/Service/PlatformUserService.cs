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

        public async Task<bool> ProvideAsync(long userId, long toUserId, long Integral, string typeName,string toTypeName, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var type = await dbc.IntegralTypes.SingleOrDefaultAsync(i => i.Name == typeName);
                var toType = await dbc.IntegralTypes.SingleOrDefaultAsync(i => i.Name == toTypeName);
                if (type == null)
                {
                    return false;
                }
                if(toType==null)
                {
                    return false;
                }
                var user = await dbc.PlatformUsers.SingleOrDefaultAsync(p=>p.Id==userId);
                if(user==null)
                {
                    return false;
                }
                var toUser = await dbc.PlatformUsers.SingleOrDefaultAsync(p => p.Id == toUserId);
                if(toUser==null)
                {
                    return false;
                }
                if(user.Code == "CodePlatformUser201805051709360001" && type.Name =="平台积分")
                {
                    if(toType.Name== "平台积分")
                    {
                        toUser.PlatformIntegral = toUser.PlatformIntegral + Integral;
                        JournalEntity journal = new JournalEntity();
                        journal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == description).Id;
                        journal.Description = description;
                        journal.InIntegral = Integral;
                        journal.Integral = toUser.PlatformIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        journal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        dbc.Journals.Add(journal);
                    }
                    else if(toType.Name == "商家积分" && toUser.PlatformUserType.Name == "商家")
                    {
                        if(user.PlatformIntegral < Integral)
                        {
                            return false;
                        }
                        user.PlatformIntegral = user.PlatformIntegral - Integral;
                        JournalEntity journal = new JournalEntity();
                        journal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == description).Id;
                        journal.Description = description;
                        journal.OutIntegral = Integral;
                        journal.Integral = user.PlatformIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        journal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        dbc.Journals.Add(journal);

                        toUser.GivingIntegral = toUser.GivingIntegral + Integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        toJournal.Description = description;
                        toJournal.InIntegral = Integral;
                        toJournal.Integral = toUser.GivingIntegral;
                        toJournal.IntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        toJournal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == description).Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else if (toType.Name == "消费积分" && toUser.PlatformUserType.Name == "商家")
                    {
                        if (user.PlatformIntegral < Integral)
                        {
                            return false;
                        }
                        user.PlatformIntegral = user.PlatformIntegral - Integral;
                        JournalEntity journal = new JournalEntity();
                        journal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == description).Id;
                        journal.Description = description;
                        journal.OutIntegral = Integral;
                        journal.Integral = user.PlatformIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        journal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + Integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        toJournal.Description = description;
                        toJournal.InIntegral = Integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        toJournal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == description).Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else if (toType.Name == "消费积分" && toUser.PlatformUserType.Name == "客户")
                    {
                        if (user.PlatformIntegral < Integral)
                        {
                            return false;
                        }
                        user.PlatformIntegral = user.PlatformIntegral - Integral;
                        JournalEntity journal = new JournalEntity();
                        journal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == description).Id;
                        journal.Description = description;
                        journal.OutIntegral = Integral;
                        journal.Integral = user.PlatformIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        journal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + Integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        toJournal.Description = description;
                        toJournal.InIntegral = Integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == description).Id;
                        toJournal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == "空类型").Id;
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
                        if (user.GivingIntegral < Integral)
                        {
                            return false;
                        }
                        user.GivingIntegral = user.GivingIntegral - Integral;
                        JournalEntity journal = new JournalEntity();
                        journal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        journal.Description = description;
                        journal.OutIntegral = Integral;
                        journal.Integral = user.GivingIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        journal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == description).Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + Integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        toJournal.Description = description;
                        toJournal.InIntegral = Integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == description).Id;
                        toJournal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        dbc.Journals.Add(toJournal);
                    }
                    else if (toType.Name == "消费积分" && toUser.PlatformUserType.Name == "客户")
                    {
                        if (user.GivingIntegral < Integral)
                        {
                            return false;
                        }
                        user.GivingIntegral = user.GivingIntegral - Integral;
                        JournalEntity journal = new JournalEntity();
                        journal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        journal.Description = description;
                        journal.OutIntegral = Integral;
                        journal.Integral = user.GivingIntegral;
                        journal.IntegralTypeId = type.Id;
                        journal.ToPlatformUserId = toUser.Id;
                        journal.PlatformUserId = user.Id;
                        journal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        journal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == description).Id;
                        dbc.Journals.Add(journal);

                        toUser.UseIntegral = toUser.UseIntegral + Integral;
                        JournalEntity toJournal = new JournalEntity();
                        toJournal.ChangeTypeId = dbc.ChangeTypes.SingleOrDefault(c => c.Description == "空类型").Id;
                        toJournal.Description = description;
                        toJournal.InIntegral = Integral;
                        toJournal.Integral = toUser.UseIntegral;
                        toJournal.IntegralTypeId = toType.Id;
                        toJournal.ToPlatformUserId = toUser.Id;
                        toJournal.PlatformUserId = toUser.Id;
                        toJournal.JournalUserTypeId = dbc.JournalUserTypes.SingleOrDefault(c => c.Description == description).Id;
                        toJournal.JournalTypeId = dbc.JournalTypes.SingleOrDefault(c => c.Description == "空类型").Id;
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
                return true; ;
            }
        }
    }
}
