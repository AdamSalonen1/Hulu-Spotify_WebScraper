using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Windows.Forms;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace Application
{
    internal class DataLayer
    {
        #region Initializations

        /// <summary>
        /// default constructor
        /// </summary>
        public DataLayer()
        {
            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
            DeleteChromeDriver();
        }
        #endregion
        /// <summary>
        /// Method which opens hulu, signs in, and opens the requested TV show.
        /// </summary>
        public void RunHulu(string tvShow)
        {
            #region Add Websites
            //initialize Hulu's url and credentials
            WebPage hulu = new WebPage(ConfigurationManager.AppSettings["Url"], ConfigurationManager.AppSettings["emailAddress"], ConfigurationManager.AppSettings["pass"]);
            #endregion

            ChromeOptions options = new ChromeOptions();
            options.AddExtension(ConfigurationManager.AppSettings["ExtensionPath"]);
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtention", false);
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            WebDriver driver = new ChromeDriver(options);
            

            #region HuluProcesses
            //Open the website
            driver.Url = hulu.Url;
            Actions act = new Actions(driver);

            SendKeys.SendWait("^{w}");


            //sign into hulu
            SendToElement("xpath", "//input[@data-automationid='email-field']", hulu.email, driver);
            SendToElement("xpath", "//input[@data-automationid='password-field']", hulu.pass, driver);

            Sleep(1);
            //click the login button
            ClickElement("xpath", "//button[@data-automationid='login-button']", driver);


            //click on the profile.
            EmailIdentifier(driver);

            //click on search bar and type in show I want
            ClickElement("xpath", "//*[@id=\"__next\"]/header/nav/div/a", driver);
            SendToElement("xpath", "//*[@id=\"__next\"]/div[1]/div[2]/div/div[1]/div/div/input", tvShow, driver);

            ClickElement("xpath", "//input[@aria-label='Search']", driver);
            try
            {
                ClickElement("xpath", "//button[@class='FullTextSearch__Entry cu-search-button FullTextSearch__Entry--Selected']", driver);
            }
            catch
            {
                act.MoveToElement(driver.FindElement(By.XPath("//button[@class='FullTextSearch__Entry cu-search-button FullTextSearch__Entry--Selected']"))).Click().Perform();
            }

            //click on the show
            try
            {
                ClickElement("xpath", "//span[@aria-label='Item 1 of 30']", driver);
            }
            catch
            {
                act.MoveToElement(driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[2]/div/div[2]/div/div/div[1]/div[2]/div/div/ul/li[1]/div/figure/div/div/button/div/span[2]"))).Click();
            }

            //click the play button
            try
            {
                ClickElement("xpath", "/html/body/div[2]/div[1]/div[2]/div/div/div[2]/div[1]/div/div/div/div[3]/div/div/div/div[6]/div/div[1]/a", driver);
            }
            catch
            {
                ClickElement("xpath", "/html/body/div[2]/div[1]/div[2]/div/div/div[2]/div[1]/div/div/div/div[3]/div/div/div/div[6]/div/div[1]/a/svg/path", driver);
            }

            //fullscreen
            ClickElement("xpath", "//div[@aria-label='FULL SCREEN']", driver);
            #endregion Processes
        }

        public void RunSpotify(bool shuffle, string playlist)
        {
            WebPage spotify = new WebPage("https://accounts.spotify.com/en/login?continue=https", ConfigurationManager.AppSettings["SpotifyEmailAddress"], ConfigurationManager.AppSettings["SpotifyPass"]);
            spotify.playlist = playlist;

            ChromeOptions options = new ChromeOptions();
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtention", false);
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            WebDriver driver = new ChromeDriver(options);
            Actions act = new Actions(driver);
            
            
            //open Spotify
            driver.Url = spotify.Url;

            //enter login info
            SendToElement("id", "login-username", spotify.email, driver);
            SendToElement("id", "login-password", spotify.pass, driver);

            //click on the login button
            ClickElement("id", "login-button", driver);

            //Click on the webPlayer icon
            ClickElement("xpath", "/html/body/div/div/div[2]/div/div/button[2]", driver);

            //Click on the search button
            ClickElement("xpath", "//button[@data-testid='web-player-link']", driver);

            //try to click the little cookies text box that covers the media controls
            ClickElement("xpath", "/html/body/div[14]/div[3]/div/div[2]/button", driver);

            //click on the search bar and enter info you want to play
            ClickElement("xpath", "/html/body/div[4]/div/div[2]/nav/div[1]/ul/li[2]/a", driver);
            SendToElement("xpath", "/html/body/div[4]/div/div[2]/div[1]/header/div[3]/div/div/form/input", spotify.playlist, driver);

            //click on the actual playlist
            ClickElement("xpath", "/html/body/div[4]/div/div[2]/div[3]/div[1]/div[2]/div[2]/div/div/div[2]/main/div[2]/div/div/section[1]/div[2]/div/div/div/div[4]", driver);

            //if the user wants to shuffle
            if (shuffle)
            {
                //if shuffle butten is already pressed
                if (driver.FindElement(By.XPath("//button[@data-testid='control-button-shuffle']")).GetAttribute("aria-checked").Equals("true"))
                {
                    //Click the big green button and skip
                    ClickElement("xpath", "//span[@class='IconWrapper__Wrapper-sc-1hf1hjl-0 dZGDpi']", driver);
                    Sleep(1);

                    ClickElement("xpath", "//button[@aria-label='Next']", driver);
                }
                //if the shuffle button has not been pressed
                else if (driver.FindElement(By.XPath("//button[@data-testid='control-button-shuffle']")).GetAttribute("aria-checked").Equals("false"))
                {
                    //press the shuffle button and then the big green button, then skip
                    ClickElement("xpath", "//button[@data-testid='control-button-shuffle']", driver);

                    ClickElement("xpath", "//span[@class='IconWrapper__Wrapper-sc-1hf1hjl-0 dZGDpi']", driver);
                    Sleep(1);

                    ClickElement("xpath", "//button[@aria-label='Next']", driver);
                }
            }
            //otherwise, the user does not want to shuffle
            else
            {
                //if the shuffle button has been pressed
                if (driver.FindElement(By.XPath("//button[@data-testid='control-button-shuffle']")).GetAttribute("aria-checked").Equals("true"))
                {
                    //untick the shuffle button and click on the first song in the album/playlist
                    ClickElement("xpath", "//button[@data-testid='control-button-shuffle']", driver);
                    Sleep(1);

                    //act.MoveToElement(driver.FindElement(By.XPath("//span[text()='1']"))).Click().Perform();
                    ClickElement("xpath", "//span[text()='1']", driver);
                }
                //if the shuffle button has not been pressed
                else if (driver.FindElement(By.XPath("//button[@data-testid='control-button-shuffle']")).GetAttribute("aria-checked").Equals("false"))
                {

                    //click on the first song in the album/playlist
                    //act.MoveToElement(driver.FindElement(By.XPath("//span[text()='1']"))).Click().Perform();
                    ClickElement("xpath", "//span[text()='1']", driver);
                }
            }

        }

        #region Helper methods
        /// <summary>
        /// Helper method which tells the system to wait
        /// </summary>
        /// <param name="i"></param>
        public static void Sleep(int i)
        {
            string sleepString = i + "000";
            System.Threading.Thread.Sleep(Int32.Parse(sleepString));
        }

        /// <summary>
        /// Helper method which automatically clickson an element as soon as it is valid
        /// 20 second timeout
        /// </summary>
        /// <param name="by"></param>
        /// <param name="elementName"></param>
        /// <param name="driver"></param>
        public static void ClickElement(string by, string elementName, WebDriver driver)
        {
            if (by.ToLower().Equals("xpath"))
            {
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(elementName)));
                element.Click();
            }
            else if (by.ToLower().Equals("id"))
            {
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(elementName)));
                element.Click();
            }
            else if (by.ToLower().Equals("name"))
            {
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name(elementName)));
                element.Click();
            }
        }

        public static void SendToElement(string by, string elementName, string keys, WebDriver driver)
        {
            if (by.ToLower().Equals("xpath"))
            {
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(elementName)));
                element.SendKeys(keys);
            }
            else if (by.ToLower().Equals("id"))
            {
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(elementName)));
                element.SendKeys(keys);
            }
            else if (by.ToLower().Equals("name"))
            {
                var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name(elementName)));
                element.SendKeys(keys);
            }
        }

        internal bool TestElement(string by, string elementName, WebDriver driver)
        {
            if (by.ToLower().Equals("xpath"))
            {
                try
                {
                    var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0));
                    var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(elementName)));
                    return true;
                }
                catch { return false; }
            }
            else if (by.ToLower().Equals("id"))
            {
                try
                {
                    var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0));
                    var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(elementName)));
                    return true;
                }
                catch { return false; }
            }
            else if (by.ToLower().Equals("name"))
            {
                try
                {
                    var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0));
                    var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.Name(elementName)));
                    return true;
                }
                catch { return false; }
            }
            else return false;
        }

        internal void EmailIdentifier(WebDriver driver)
        {
            try
            {
                if(TestElement("xpath", "//*[@id=\"__next\"]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/a/span/div/div[1]", driver) == false)
                {
                    if(TestElement("xpath", "//input[@aria-label='Enter digit 1 of the code you received, or paste the entire code.']", driver) == false)
                    {
                        EmailIdentifier(driver);
                    }
                    else
                    {
                        Sleep(5);
                        Gmail g = new Gmail();
                        string huluVerificationCode = g.GetHuluVerificationCode();

                        SendToElement("xpath", "//input[@aria-label='Enter digit 1 of the code you received, or paste the entire code.']", huluVerificationCode, driver);

                        ClickElement("xpath", "//*[@id=\"__next\"]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/a/span/div/div[1]", driver);
                    }
                }
                else
                {
                    ClickElement("xpath", "//*[@id=\"__next\"]/div[1]/div/div[2]/div/div[2]/div/div/div[2]/a/span/div/div[1]", driver);
                }
            }
            catch { Console.WriteLine("Recursion Error"); }
        }
        #endregion

        internal void DeleteChromeDriver()
        {
            try
            {
                System.IO.File.Delete(ConfigurationManager.AppSettings["ChromeDriverPath"]);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}