namespace Nop.Services
{
    public interface ICustomService
    {
        InjectType Inject { get; set; }
    }
    public enum InjectType
    {
        Scopped=0,
        Transit=1,
        SingleTon=2
    }
}
