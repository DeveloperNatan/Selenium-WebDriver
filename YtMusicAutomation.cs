using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp1
{
    public static class YtMusicAutomation
    {
        public static void Run(){
            Console.WriteLine("Starting application...");

// Google Chrome binary installed via Flatpak (instead of Chromium/Chrome for Testing)
            const string chromeBinary = "/var/lib/flatpak/app/com.google.Chrome/current/active/files/extra/chrome";

// Dedicated automation profile. Chrome (136+) blocks automation on the default profile,
// so we use a separate one: log in to it once and the session stays saved.
            string profileDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "selenium-chrome-profile");

            var options = new ChromeOptions { BinaryLocation = chromeBinary };
            options.AddArgument($"--user-data-dir={profileDir}");
            options.AddArgument("--profile-directory=Default");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddExcludedArgument("enable-automation");

            IWebDriver driver = new ChromeDriver(options);
            Console.WriteLine("ChromeDriver started.");

            driver.Navigate().GoToUrl("https://music.youtube.com/watch?v=w0T1aDx9GXw");
            Console.WriteLine("Navigated to: https://music.youtube.com/");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            Console.WriteLine("WebDriverWait created with 30 seconds.");

            Console.WriteLine("Looking for the play button...");
            try
            {
                IWebElement Play = wait.Until(d =>
                    d.FindElement(By.Id("play-pause-button")));
                Console.WriteLine("Play button found.");

                Play.Click();
                Console.WriteLine("Play button clicked.");
            }
            catch
                (WebDriverTimeoutException)
            {
                Console.WriteLine("Play button not found, continuing without clicking.");
            }

// Keeps the program running with no time limit, until Ctrl+C or until the Chrome window is closed.
            var shutdown = new ManualResetEventSlim(false);
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                shutdown.Set();
            };

            Console.WriteLine("Running. Press Ctrl+C to exit.");
            while (!shutdown.Wait(TimeSpan.FromSeconds(2)))
            {
                try
                {
                    if (driver.WindowHandles.Count == 0) break;
                }
                catch (WebDriverException)
                {
                    // Browser was closed manually
                    break;
                }
            }

            try
            {
                driver.Quit();
            }
            catch (WebDriverException)
            {
            }

            Console.WriteLine("Execution finished.");
        }
    }
}