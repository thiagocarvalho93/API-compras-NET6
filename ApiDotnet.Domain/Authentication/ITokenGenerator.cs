using ApiDotnet.Domain.Entities;

namespace ApiDotnet.Domain.Authentication
{
    public interface ITokenGenerator
    {
        dynamic Generator(User user);
    }
}