using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TwitchParser
{
    public class UploadVideo
    {
        public void UploadYouTubeVideo(IWebDriver driver, string title, string game, string tags, string streamerName,
            string description, string filePath)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(800));
            
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            try
            {
                driver.Navigate().GoToUrl("http://youtube.com/upload");
                var fileUpload = wait.Until(d => d.FindElement(By.XPath("//input[@type='file']")));
                fileUpload.SendKeys(filePath);
            }
            catch (Exception)
            {
                driver.SwitchTo().Alert().Accept();
                var fileUpload = wait.Until(d => d.FindElement(By.XPath("//input[@type='file']")));
                fileUpload.SendKeys(filePath);
            }
            Console.Write($"\nAttached video {filePath}");

            //Set Title 
            var titlePath = wait.Until(d => d.FindElement(By
                .XPath("/html/body/ytcp-uploads-dialog/tp-yt-paper-dialog/div/ytcp-animatable[1]/ytcp-video-metadata-editor/div/ytcp-video-metadata-editor-basics/div[1]/ytcp-mention-textbox/ytcp-form-input-container/div[1]/div[2]/ytcp-mention-input/div")));
            titlePath.Click();
            titlePath.Clear();
            title = $"{title.LimitLength(97 - streamerName.Length)} / {streamerName}".LimitLength(100);
            titlePath.SendKeys(title);
            
            //Set Description 
            var descriptionPath = wait.Until(d => d.FindElement(By
                .XPath("/html/body/ytcp-uploads-dialog/tp-yt-paper-dialog/div/ytcp-animatable[1]/ytcp-video-metadata-editor/div/ytcp-video-metadata-editor-basics/div[2]/ytcp-mention-textbox/ytcp-form-input-container/div[1]/div[2]/ytcp-mention-input/div")));
            descriptionPath.SendKeys(description);
            
            //Set children allow
            var kidsSection = wait.Until(d => d.FindElement(By.Name("NOT_MADE_FOR_KIDS")));
            kidsSection.FindElement(By.Id("radioLabel")).Click();
                
            // Advanced options
            var advoptions = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"toggle-button\"]")));
            advoptions.Click();
            Thread.Sleep(500);

            //Set Tegs
            var tagsContainer =  wait.Until(d => d.FindElement(By.XPath("//*[@id=\"tags-container\"]")));
            tags = tags.Replace("#", "");
            tagsContainer.FindElement(By.Id("text-input")).SendKeys($"#{game}, {tags}, #{streamerName}");
            
            //Set category 
            var categoryList =  wait.Until(d => d.FindElement(By.XPath("//*[@id=\"category\"]")));
            categoryList.Click();
            driver.FindElement(By.XPath("//*[@id=\"text-item-6\"]")).Click();
            
            var gameName = driver.FindElement(By
                .XPath("//*[@id=\"category-container\"]/ytcp-form-gaming/ytcp-form-autocomplete/ytcp-dropdown-trigger/div/div[2]/input"));
            gameName.Click();
            gameName.SendKeys(game); //Game Name in Category

            try
            {
                var next = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"next-button\"]")));//Next Button
                next.Click();
                next = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"next-button\"]")));//Next Button
                next.Click();
                next = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"next-button\"]")));//Next Button
                next.Click();
                Thread.Sleep(4000);
                var choose = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"privacy-radios\"]/tp-yt-paper-radio-button[3]")));
                choose.Click();
            
                Thread.Sleep(3000);
                var upload = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"done-button\"]")));//Upload Button
                upload.Click();
            }
            catch (Exception e)
            {
                Console.Write("\nVideo on YouTube not uploaded cuz error: \n");
                Console.Write(e.StackTrace);
                return;
            }
            Console.Write("\nVideo on Youtube uploaded without Error");
        }
        
        public void UploadTikTokVideo(IWebDriver driver, string title, string game, string tags ,string streamerName,
            string filePath)
        {
	        driver.SwitchTo().Window(driver.WindowHandles.Last());
            driver.Navigate().GoToUrl("https://tiktok.com/upload/?lang=en");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(800));
            Thread.Sleep(3000);
            
            HandleAlert(driver);
            var fileUpload = wait.Until(d =>
                d.FindElement(By.Id("main")).FindElements(By.Name("upload-btn")).FirstOrDefault());
            HandleAlert(driver);

            Debug.Assert(fileUpload != null, nameof(fileUpload) + " = null");
            fileUpload.SendKeys(filePath);

            var caption = driver.FindElement(By
                .XPath($"//*[@id=\"main\"]/div[2]/div/div[2]/div[3]/div[1]/div[1]/div[2]/div/div[1]/div/div/div/div/div/div"));
            caption.Click();
            
            game = game.Replace(" ", "");
            game = Regex.Replace(game, "[:-]", "", RegexOptions.None);
            tags = Regex.Replace(tags, @",", "", RegexOptions.None);
            string additionalTags = $"#fy #fyp"; 
            title = $"{title.LimitLength(120)} / #{streamerName} #{game.ToLower()} {tags} {additionalTags}".LimitLength(150);
            caption.SendKeys(title);
            
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"main\"]/div[2]/div/div[2]/div[1]/div[1]/div[1]")));
            Thread.Sleep(1000);

            try
            {
                var post = wait.Until(d => d.FindElement(By
                    .XPath("//*[@id=\"main\"]/div[2]/div/div[2]/div[3]/div[6]/button[2]")));
                post.Click();
            }
            catch (Exception e)
            {
                Console.Write("Tik-Tok Error: ");
                Console.WriteLine(e.StackTrace);
            }
            
            File.Delete(filePath);
            driver.Manage().Window.Minimize();
            Console.Write("\nClip on TikTok uploaded\n");
        }
        
        public static void HandleAlert(IWebDriver driver, WebDriverWait wait = null)
        {
            if (wait == null)
            {
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            }

            try
            {
                IAlert alert = wait.Until(drv => {
                    try
                    {
                        return drv.SwitchTo().Alert();
                    }
                    catch (NoAlertPresentException)
                    {
                        return null;
                    }
                });
                alert.Accept();
            }
            catch (WebDriverTimeoutException) { /* Ignore */ }
        }
    }
}
