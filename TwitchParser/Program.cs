using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TwitchParser
{
    class Program
    {
        private static void Main()
        {
            string game = Games.Gta5;
            string replase = "[\\:|?*<>\"/]";
            string cleanTitle =
                "[<>]\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff]|[<>]";
            string description = Description.GetIndigoDescription(game);
            string tags = "#gta5moments, #gta5bestmoments, #лучшиемоментыгта, #ragemp, # #twitch, #twitchclipsdaily, #topclips, #twitchbestmoments";
            string url = $"https://api.twitch.tv/kraken/clips/top?game={game}&period=day&limit=30&language=ru";
            int waitMinutes = 60;
            int clipInArray = 0;
            var driver = new ChromeDriver();
            var uploadVideo = new UploadVideo();
            var client = new WebClient();
            
            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            while (true)
            {
                var clipResponse = JsonConvert.DeserializeObject<ClipResponse>(GetClip(url));
                
                Debug.Assert(clipResponse != null, nameof(clipResponse) + " = null");
                while (clipInArray < 24 && clipInArray <= clipResponse.Clips.Length)
                {
                    Console.Write($"Clip in Array: {clipInArray}\n");
                    string steamerName = clipResponse.Clips[clipInArray].Broadcaster.Display_Name;
                    string title = clipResponse.Clips[clipInArray].Title.ToUpper();
                    title = Regex.Replace(title, cleanTitle,"");
                    string fileName = Regex.Replace(title, replase, "", RegexOptions.None);
                    Console.Write($"Clip name: {title} URL: {clipResponse.Clips[clipInArray].Url}\n");
                    
                    string savePath = $@"D:\Clips\{fileName}.mp4";
                    string[] videoUrl = clipResponse.Clips[clipInArray].Thumbnails.Tiny.Split("-preview-");
                    client.DownloadFile($"{videoUrl[0]}.mp4", savePath);
                    Console.Write("Video downloaded \n");
                    
                    uploadVideo.UploadYouTubeVideo(driver, title, game, tags, steamerName, description, savePath);
                    uploadVideo.UploadTikTokVideo(driver, title, game, tags, steamerName, savePath);
                    
                    clipInArray++;
                    Console.Write("\nTime Now: "+DateTime.Now);
                    Console.Write($"\nWait {waitMinutes} min. \n");
                    Thread.Sleep(waitMinutes*60000);
                }
                clipInArray = 0;
            }
        }

        private static string GetClip(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url);
            
            webRequest.Method = WebRequestMethods.Http.Get;
            webRequest.Headers[HttpRequestHeader.Accept] = "application/vnd.twitchtv.v5+json";
            webRequest.Headers.Add("Client-ID"," qtaizfmiwkpvk2km0mcbimsixp4fnl");
            
            HttpWebResponse webResponse = (HttpWebResponse) webRequest.GetResponse();

            string readData;
            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                readData = streamReader.ReadToEnd();
            }

            return readData;
        }
    }
}