namespace Pengqian.NetworkDisk.Infrastructure
{
    public class DbOption
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; } = 27017;

        /// <summary>
        /// 连接用户
        /// </summary>
        public string ConnUser { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";

        //public string ConnectString { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Db { get; set; } = "NetworkDisk";
    }

    public static class DbTools
    {
        /// <summary>
        /// 默认Db配置
        /// </summary>
        public static readonly DbOption DefaultOption = new DbOption();
    }
}