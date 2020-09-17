namespace Pengqian.NetworkDisk.Infrastructure.Enum
{
    public enum Permissions
    {
        /// <summary>
        /// 没有权限
        /// </summary>
        None = 0,

        /// <summary>
        /// 系统权限
        /// </summary>
        System = 1 << 1,

        /// <summary>
        /// demo
        /// </summary>
        Demo = 1 << 2
    }
}