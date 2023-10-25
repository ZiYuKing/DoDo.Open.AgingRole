namespace DoDo.Open.AgingRole
{
    public class AppSetting
    {
        /// <summary>
        /// 机器人唯一标识
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 机器人鉴权Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 机器人响应频道ID
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 管理员一DoDo号
        /// </summary>
        public string AdminDoDoId { get; set; }

        /// <summary>
        /// 管理员二DoDo号
        /// </summary>
        public string AdminDoDo2Id { get; set; }

        /// <summary>
        /// 管理员三DoDo号
        /// </summary>
        public string AdminDoDo3Id { get; set; }

        /// <summary>
        /// 管理员四DoDo号
        /// </summary>
        public string AdminDoDo4Id { get; set; }

        /// <summary>
        /// 管理员五DoDo号
        /// </summary>
        public string AdminDoDo5Id { get; set; }

        /// <summary>
        /// 管理员六DoDo号
        /// </summary>
        public string AdminDoDo6Id { get; set; }

        /// <summary>
        /// 管理员七DoDo号
        /// </summary>
        public string AdminDoDo7Id { get; set; }

        /// <summary>
        /// 管理员八DoDo号
        /// </summary>
        public string AdminDoDo8Id { get; set; }

        /// <summary>
        /// 管理员九DoDo号
        /// </summary>
        public string AdminDoDo9Id { get; set; }

        /// <summary>
        /// 日卡配置
        /// </summary>
        public DayCard DayCard { get; set; }

        /// <summary>
        /// 周卡配置
        /// </summary>
        public WeekCard WeekCard { get; set; }

        /// <summary>
        /// 月卡配置
        /// </summary>
        public MonthCard MonthCard { get; set; }

        /// <summary>
        /// 季卡配置
        /// </summary>
        public MonthCard SeasonCard { get; set; }

        /// <summary>
        /// 年卡配置
        /// </summary>
        public YearCard YearCard { get; set; }

        /// <summary>
        /// 永久卡卡配置
        /// </summary>
        public PermanentCard PermanentCard { get; set; }

        /// <summary>
        /// 查询配置
        /// </summary>
        public Query Query { get; set; }
    }

    public class DayCard
    {
        /// <summary>
        /// 日卡指令
        /// </summary>
        public string Command { get; set; }
    }

    public class WeekCard
    {
        /// <summary>
        /// 周卡指令
        /// </summary>
        public string Command { get; set; }
    }

    public class MonthCard
    {
        /// <summary>
        /// 月卡指令
        /// </summary>
        public string Command { get; set; }
    }

    public class YearCard
    {
        /// <summary>
        /// 年卡指令
        /// </summary>
        public string Command { get; set; }
    }
    public class PermanentCard
    {
        /// <summary>
        /// 永久卡指令
        /// </summary>
        public string Command { get; set; }
    }

    public class Query
    {
        /// <summary>
        /// 查询指令
        /// </summary>
        public string Command { get; set; }
    }
}