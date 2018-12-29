namespace Ploeh.Samples.UserProfile
{
    public interface IUserRepository
    {
        Maybe<string> ReadIconId(int userId);
    }
}