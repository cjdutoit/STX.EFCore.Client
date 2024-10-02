// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using STX.EFCore.Client.Infrastructure.Services;

namespace STX.EFCore.Client.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();

            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "STX.EFCore.Client");
        }
    }
}
