using IRAP.BL.S7Gateway.Entities;

namespace IRAP.BL.S7Gateway
{
    /// <summary>
    /// Tag对象注册事件
    /// </summary>
    /// <param name="group">TagGroup对象</param>
    /// <param name="tag">Tag对象</param>
    public delegate void TagRegisterHandler(CustomGroup group, CustomTag tag);
}