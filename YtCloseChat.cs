using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp1
{
    public static class YtCloseChat
    {
        private const string CloseChatXPath =
            "/html/body/yt-live-chat-app/div/yt-live-chat-renderer/tp-yt-iron-pages/div/yt-live-chat-header-renderer/div[4]/yt-button-renderer/yt-button-shape/button";

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

            // O chat volta depois de entrar na tela inteira, então fecha de novo
            CloseChat(driver, wait);

            // Mantém o programa rodando
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

                    // Quando a live acaba o YouTube vai para a próxima e o chat abre de novo
                    CloseChatIfOpen(driver);
                }
                catch (WebDriverException)
                {
                    // O navegador foi fechado manualmente
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
                // O chat da live fica dentro de um iframe, então entra nele primeiro
                driver.SwitchTo().Frame(wait.Until(d => d.FindElement(By.Id("chatframe"))));

                IWebElement closeButton = wait.Until(d =>
                    d.FindElement(By.XPath(CloseChatXPath)));
                closeButton.Click();
                Console.WriteLine("Close button clicked");
            }
            catch (WebDriverException)
            {
                Console.WriteLine("Close chat button not found!");
            }

            // Sai do iframe do chat, o player fica na página do vídeo
            driver.SwitchTo().DefaultContent();
        }

        // Checagem rápida, sem esperar, usada pelo laço de monitoramento
        private static void CloseChatIfOpen(IWebDriver driver)
        {
            try
            {
                var chatFrame = driver.FindElements(By.Id("chatframe"));
                if (chatFrame.Count == 0) return;

                driver.SwitchTo().Frame(chatFrame[0]);
                var closeButton = driver.FindElements(By.XPath(CloseChatXPath));
                if (closeButton.Count > 0 && closeButton[0].Displayed)
                {
                    closeButton[0].Click();
                    Console.WriteLine("Chat opened again, closed it.");
                }
            }
            catch (WebDriverException)
            {
                // A página mudou durante a checagem (ex.: próxima live carregando), tenta de novo na próxima volta
            }
            finally
            {
                try { driver.SwitchTo().DefaultContent(); } catch (WebDriverException) { }
            }
        }
    }
}
