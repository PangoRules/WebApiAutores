namespace WebApiAutores.Services;

public interface IService
{
    void DoTask();
}

public class ServiceA : IService
{
    public void DoTask()
    {
        throw new NotImplementedException();
    }
}

public class ServiceB : IService
{
    public void DoTask()
    {
        throw new NotImplementedException();
    }
}
