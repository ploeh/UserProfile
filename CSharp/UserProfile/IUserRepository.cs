namespace Ploeh.Samples.UserProfile
{
    public interface IUserRepository
    {
        bool TryReadIconId(int userId, out string iconId);
    }
}