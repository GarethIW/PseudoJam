using System;

namespace YouAreTheVillain
{
#if WINDOWS || XBOX 
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (YouAreTheVillain game = new YouAreTheVillain())
            {
                game.Run();
            }
        }
    }
#endif
#if WINRT
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var factory = new MonoGame.Framework.GameFrameworkViewSource<YouAreTheVillain>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
        }
    }
#endif
}

