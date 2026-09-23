using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp1
{
    public static class YtMusicAutomation
    {
        public static void Run(){
            Console.WriteLine("Starting application...");

// Executável do Google Chrome instalado via Flatpak (em vez do Chromium/Chrome for Testing)
            const string chromeBinary = "/var/lib/flatpak/app/com.google.Chrome/current/active/files/extra/chrome";

// Perfil dedicado para automação. O Chrome (136+) bloqueia automação no perfil padrão,
// então usamos um separado: faça login nele uma vez e a sessão fica salva.
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

// Mantém o programa rodando sem limite de tempo, até Ctrl+C ou até a janela do Chrome ser fechada.
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
    }
}