using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp1
{
    public static class YtCloseChat
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

            driver.Navigate().GoToUrl("https://www.youtube.com/watch?v=P-gvEgSSo1Y");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            CloseChat(driver, wait);

            try
            {
                IWebElement fullscreenButton = wait.Until(d =>
                    d.FindElement(By.CssSelector("#movie_player .ytp-fullscreen-button")));
                fullscreenButton.Click();
                Console.WriteLine("Fullscreen enabled.");
            }
            catch (WebDriverException)
            {
                Console.WriteLine("Fullscreen button not found!");
            }

            // The chat comes back after entering fullscreen, so close it again
            CloseChat(driver, wait);

            //keep program running 
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

        private static void CloseChat(IWebDriver driver, WebDriverWait wait)
        {
            Console.WriteLine("Looking for the button close chat");
            try
            {
                // The live chat is inside an iframe, so switch into it first
                driver.SwitchTo().Frame(wait.Until(d => d.FindElement(By.Id("chatframe"))));

                IWebElement closeButton = wait.Until(d =>
                    d.FindElement(By.XPath("/html/body/yt-live-chat-app/div/yt-live-chat-renderer/tp-yt-iron-pages/div/yt-live-chat-header-renderer/div[4]/yt-button-renderer/yt-button-shape/button")));
                closeButton.Click();
                Console.WriteLine("Close button clicked");
            }
            catch (WebDriverException)
            {
                Console.WriteLine("Close chat button not found!");
            }

            // Leave the chat iframe, the player is on the video page
            driver.SwitchTo().DefaultContent();
        }
    }
}
