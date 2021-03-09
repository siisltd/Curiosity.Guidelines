namespace LocalisedLib
{
    public interface ILocalisedService
    {
        string GetSharedLocalizedString();

        string GetPrivateLocalizedString();
    }
}