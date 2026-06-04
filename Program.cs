using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

Console.WriteLine("Iniciando aplicação...");

IWebDriver driver = new ChromeDriver();
Console.WriteLine("ChromeDriver iniciado.");

driver.Navigate().GoToUrl("https://music.youtube.com/");
Console.WriteLine("Navegou para: https://music.youtube.com/");

var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
Console.WriteLine("WebDriverWait criado com 30 segundos.");

Console.WriteLine("Procurando botão de busca...");
IWebElement ButtonSearch = wait.Until(d => d.FindElement(By.XPath(
    "/html/body/ytmusic-app/ytmusic-app-layout/ytmusic-nav-bar/div[2]/ytmusic-search-box/div/div[1]/yt-icon-button[1]/button"
)));
Console.WriteLine("Botão de busca encontrado.");

ButtonSearch.Click();
Console.WriteLine("Clique no botão de busca realizado.");

Console.WriteLine("Procurando input de busca...");
IWebElement InputSearch = wait.Until(d =>
    d.FindElement(By.XPath(
        "/html/body/ytmusic-app/ytmusic-app-layout/ytmusic-nav-bar/div[2]/ytmusic-search-box/div/div[1]/input")));
Console.WriteLine("Input de busca encontrado.");

InputSearch.SendKeys("Gabriela alee");
Console.WriteLine("Texto 'Gabriela alee' digitado.");

InputSearch.SendKeys(Keys.Enter);
Console.WriteLine("Tecla Enter enviada.");

Console.WriteLine("Procurando botão de play...");
IWebElement Play = wait.Until(d => 
    d.FindElement(By.XPath(
        "/html/body/ytmusic-app/ytmusic-app-layout/div[5]/ytmusic-search-page/ytmusic-tabbed-search-results-renderer/div[2]/ytmusic-section-list-renderer/div[2]/ytmusic-card-shelf-renderer/div[2]/div[2]/div[1]/div/div[2]/div[2]/yt-button-renderer[1]/yt-button-shape/button/yt-touch-feedback-shape/div[2]")));
Console.WriteLine("Botão de play encontrado.");

Play.Click();
Console.WriteLine("Clique no botão de play realizado.");

Console.WriteLine("Fim da execução.");