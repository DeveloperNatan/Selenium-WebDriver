using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp1
{

    public static class YtSkipAd
    {
        public static void Run()
        {
            Console.WriteLine("Starting application...");
            const string chromeBinary = "/var/lib/flatpak/app/com.google.Chrome/current/active/files/extra/chrome";
            string profileDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "selenium-chrome-profile");

            var options = new ChromeOptions { BinaryLocation = chromeBinary };
            options.AddArgument($"--user-data-dir={profileDir}");
            options.AddArgument("--profile-directory=Default");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");

            IWebDriver driver = new ChromeDriver(options);
            Console.WriteLine("ChromeDriver started.");

        }
    }
}