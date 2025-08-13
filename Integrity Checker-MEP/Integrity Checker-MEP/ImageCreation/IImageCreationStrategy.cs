using Autodesk.Navisworks.Api.Clash;

namespace Integrity_Checker_MEP.ImageCreation
{
    public interface IImageCreationStrategy
    {
        void MakeImage(ClashResult clashResult, string directoryPath, string testName);
    }
}
