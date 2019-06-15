namespace UI.FGUI
{
    /// <summary>
    /// 组件模式接口
    /// </summary>
    public interface IComponent
    {
        void Operation();
        void Add(IComponent component);
        // void Remove(IComponent component);
        IComponent GetChild(string comId);
    }
}