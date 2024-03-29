﻿using System.Text.RegularExpressions;
using DoDo.Open.Sdk.Models;
using DoDo.Open.Sdk.Models.ChannelMessages;
using DoDo.Open.Sdk.Models.Events;
using DoDo.Open.Sdk.Models.Members;
using DoDo.Open.Sdk.Models.Messages;
using DoDo.Open.Sdk.Models.Roles;
using DoDo.Open.Sdk.Services;


namespace DoDo.Open.AgingRole
{
    public class BotEventProcessService : EventProcessService
    {
        private readonly OpenApiService _openApiService;
        private readonly OpenApiOptions _openApiOptions;
        private readonly AppSetting _appSetting;

        public BotEventProcessService(OpenApiService openApiService, AppSetting appSetting)
        {
            _openApiService = openApiService;
            _openApiOptions = openApiService.GetBotOptions();
            _appSetting = appSetting;
        }

        public override void Connected(string message)
        {
            _openApiOptions.Log?.Invoke($"连接状态: {message}");

            #region 时效身份组初始化

            if (!Directory.Exists($"{Environment.CurrentDirectory}\\data"))
            {
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\data");
            }

            #endregion

        }

        public override void Disconnected(string message)
        {
            _openApiOptions.Log?.Invoke($"断开连接: {message}");
        }

        public override void Reconnected(string message)
        {
            _openApiOptions.Log?.Invoke($"重新连接: {message}");
        }

        public override void Exception(string message)
        {
            _openApiOptions.Log?.Invoke($"异常: {message}");
        }

        public override void Received(string message)
        {
            _openApiOptions.Log?.Invoke($"收到信息: {message}");
        }

        public override async void ChannelMessageEvent<T>(EventSubjectOutput<EventSubjectDataBusiness<EventBodyChannelMessage<T>>> input)
        {
            try
            {
                var eventBody = input.Data.EventBody;
                var dodoId = eventBody.DodoSourceId;
                var nickName = eventBody.Member.NickName;
                

                if (eventBody.MessageBody is MessageBodyText messageBodyText)
                {
                    var content = messageBodyText.Content.Replace(" ", "");
                    var defaultReply = $"";//<@!{ eventBody.DodoSourceId}>
                    var reply = defaultReply;
                    
                    #region 时效身份组

                    var dataPath = $"{Environment.CurrentDirectory}\\data\\{eventBody.IslandSourceId}.txt";

                    if (Regex.IsMatch(content, _appSetting.WeekCard.Command) || Regex.IsMatch(content, _appSetting.MonthCard.Command) || Regex.IsMatch(content, _appSetting.DayCard.Command) || Regex.IsMatch(content, _appSetting.YearCard.Command) || Regex.IsMatch(content, _appSetting.PermanentCard.Command)
                        || Regex.IsMatch(content, _appSetting.SeasonCard.Command))
                    {
                        var isAdmin = Regex.IsMatch(dodoId, _appSetting.AdminDoDoId) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo2Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo3Id) ||
                            Regex.IsMatch(dodoId, _appSetting.AdminDoDo4Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo5Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo6Id) ||
                            Regex.IsMatch(dodoId, _appSetting.AdminDoDo7Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo8Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo9Id);

                        if (isAdmin)
                        {
                            var day = 0;
                            var dayShow = "";
                            var keyWord = "";

                            if (Regex.IsMatch(content, _appSetting.DayCard.Command))
                            {
                                day = 1;
                                dayShow = "一天";
                                keyWord = @"^.*<@!(\d+)>(.*)$";
                            }
                            else if (Regex.IsMatch(content, _appSetting.WeekCard.Command))
                            {
                                day = 7;
                                dayShow = "一周";
                                keyWord = @"^.*<@!(\d+)>(.*)$";
                            }
                            else if (Regex.IsMatch(content, _appSetting.MonthCard.Command))
                            {
                                day = 30;
                                dayShow = "一个月";
                                keyWord = @"^.*<@!(\d+)>(.*)$";
                            }
                            else if (Regex.IsMatch(content, _appSetting.SeasonCard.Command))
                            {
                                day = 90;
                                dayShow = "三个月";
                                keyWord = @"^.*<@!(\d+)>(.*)$";
                            }
                            else if (Regex.IsMatch(content, _appSetting.YearCard.Command))
                            {
                                day = 366;
                                dayShow = "一年";
                                keyWord = @"^.*<@!(\d+)>(.*)$";
                            }
                            else if (Regex.IsMatch(content, _appSetting.PermanentCard.Command))
                            {
                                day = 999999;
                                dayShow = "永久";
                                keyWord = @"^.*<@!(\d+)>(.*)$";
                            }

                            var matchResult = Regex.Match(content, keyWord);
                            var otherDoDoId = matchResult.Groups[1].Value;
                            var roleName = matchResult.Groups[2].Value;

                            if (!string.IsNullOrWhiteSpace(otherDoDoId) && !string.IsNullOrWhiteSpace(roleName))
                            {
                                var otherMemberInfo = await _openApiService.GetMemberInfoAsync(new GetMemberInfoInput
                                {
                                    IslandSourceId = eventBody.IslandSourceId,
                                    DodoSourceId = otherDoDoId
                                });

                                if (otherMemberInfo != null)
                                {
                                    var roleList = await _openApiService.GetRoleListAsync(new GetRoleListInput
                                    {
                                        IslandSourceId = eventBody.IslandSourceId
                                    });
                                    var role = roleList.FirstOrDefault(x => x.RoleName == roleName);
                                    if (role != null)
                                    {
                                        var result = await _openApiService.SetRoleMemberAddAsync(new SetRoleMemberAddInput
                                        {
                                            IslandSourceId = eventBody.IslandSourceId,
                                            DodoSourceId = otherDoDoId,
                                            RoleId = role.RoleId
                                        });

                                        if (result)
                                        {
                                            DateTime expirationTime;

                                            var oldEntityExpirationTime = DataHelper.ReadValue<DateTime>(dataPath, otherDoDoId, role.RoleId);

                                            if (oldEntityExpirationTime > DateTime.MinValue)
                                            {
                                                if (oldEntityExpirationTime > DateTime.Now)
                                                {
                                                    expirationTime = oldEntityExpirationTime.AddDays(day);
                                                }
                                                else
                                                {
                                                    expirationTime = DateTime.Now.AddDays(day);
                                                }

                                                DataHelper.WriteValue(dataPath, otherDoDoId, role.RoleId, expirationTime);
                                            }
                                            else
                                            {
                                                expirationTime = DateTime.Now.AddDays(day);

                                                DataHelper.WriteValue(dataPath, otherDoDoId, role.RoleId, expirationTime);
                                            }

                                            reply += "\n**操作成功**";
                                            reply += $"\n成功为<@!{otherDoDoId}> 添加【{roleName}】身份组，时长" +
                                                $"增加{dayShow}\n到期时间为：**{expirationTime}**";
                                        }
                                        else
                                        {
                                            reply += "\n**操作失败**";
                                            reply += $"\n赋予身份组失败，请核实本机器人是否具有管理该身份组权限！";
                                        }
                                    }
                                    else
                                    {
                                        reply += "\n**操作失败**";
                                        reply += $"\n本群不存在【{roleName}】身份组！";
                                    }
                                }
                                else
                                {
                                    reply += "\n**操作失败**";
                                    reply += "\n该用户未在本群内!";
                                }
                            }
                            else
                            {
                                reply += "\n**操作失败**";
                                reply += "\n您发送的指令格式有误！";
                            }
                        }
                        else
                        {
                            reply += "\n**操作失败**";
                            reply += "\n您未拥有【超级管理员】身份组，无权限为用户提供时效身份组！";
                        }
                    }
                    else if (Regex.IsMatch(content, _appSetting.Query.Command))
                    {
                        var regex = Regex.Match(content, $"<@!(.*?)>");
                        var targetDoDoId = regex.Groups[1].Value;
                        var isAdmin = Regex.IsMatch(dodoId, _appSetting.AdminDoDoId) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo2Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo3Id) ||
                            Regex.IsMatch(dodoId, _appSetting.AdminDoDo4Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo5Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo6Id) ||
                            Regex.IsMatch(dodoId, _appSetting.AdminDoDo7Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo8Id) || Regex.IsMatch(dodoId, _appSetting.AdminDoDo9Id);
                        if (!string.IsNullOrWhiteSpace(targetDoDoId))
                        {
                            var memberInfo = await _openApiService.GetMemberInfoAsync(new GetMemberInfoInput
                            {
                                IslandSourceId = eventBody.IslandSourceId,
                                DodoSourceId = targetDoDoId
                            });
                            nickName = memberInfo?.NickName;
                        }
                        else
                        {
                            targetDoDoId = dodoId;
                        }

                        if (targetDoDoId == dodoId || isAdmin)
                        {
                            var memberRoleList = await _openApiService.GetMemberRoleListAsync(new GetMemberRoleListInput
                            {
                                IslandSourceId = eventBody.IslandSourceId,
                                DodoSourceId = targetDoDoId
                            });

                            var list = DataHelper.ReadKeys(dataPath, targetDoDoId);

                            if (list.Count > 0)
                            {
                                reply = $"**{nickName}** 你当前是";
                                foreach (var item in list)
                                {
                                    reply += $"尊贵的【{memberRoleList.FirstOrDefault(x => x.RoleId == item)?.RoleName ?? item}】用户\n到期时间：**{DataHelper.ReadValue<string>(dataPath, targetDoDoId, item)}**";
                                }
                            }
                            else
                            {
                                reply += "\n**查询结果**";
                                reply += $"\n未查询到{(targetDoDoId == dodoId ? "您" : "对方")}的信息，可能{(targetDoDoId == dodoId ? "您" : "对方")}的权限已到期！！";
                            }
                        }
                        else
                        {
                            reply += "\n**查询失败**";
                            reply += "\n您未拥有【超级管理员】身份组，无权限查询他人的信息！！";
                        }
                    }

                    #endregion

                    if (reply != defaultReply)
                    {
                        await _openApiService.SetChannelMessageSendAsync(new SetChannelMessageSendInput<MessageBodyText>
                        {
                            ChannelId = _appSetting.ChannelId,
                            MessageBody = new MessageBodyText
                            {
                                Content = reply
                            }
                        });
                    }

                }
            }
            catch (Exception e)
            {
                Exception(e.Message);
            }
        }
    }
}
