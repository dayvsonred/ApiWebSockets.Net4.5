using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SuperWebSocket;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebRestApiV1.Controllers
{
    public class SocketServerController 
    {

        private static WebSocketServer wsServer;
        private static Timer aTimer;


        static void Main()
        {
            RunAsync().Wait();
        }


        static async Task RunAsync()
        {
            Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... "); Console.WriteLine("......................................... ");
        }

        static void Main2(string[] args)
        {



            // Create a timer and set a two second interval.
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 2000;
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;
            // Start the timer
            aTimer.Enabled = true;
            Console.WriteLine("Press the Enter key to exit the program at any time... ");



            wsServer = new WebSocketServer();
            int port = 8088;
            //wsServer.Setup(port);


            var setupComplete = wsServer.Setup(new ServerConfig
            {
                Name = "NecroWebSocket",
                Ip = "localhost",
                Port = port,
                Mode = SocketMode.Tcp,
                
               
            });

            if (setupComplete == false)
            {
                Console.WriteLine(@"Failed to start WebSocketServer on port : {0}", port);
                return;
            }




            wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            wsServer.NewDataReceived += WsServer_NewDataReceived;
            wsServer.SessionClosed += WsServer_SessionClosed;
            wsServer.Start();
            Console.WriteLine("Server is running on port " + port + ". Press ENTER to exit....");
            //Console.ReadKey();
            //wsServer.Stop();




        }




        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("data {0}", e.SignalTime);

            //Console.WriteLine("con {0}", 2 );
            try
            {

                //string date = @" { "message' :  '" + e.SignalTime +"'} ";
                //string jdate = JsonConvert.SerializeObject(date);


                JArray array = new JArray();
                array.Add("Manual text");
                array.Add(new DateTime(2000, 5, 23));

                JObject o = new JObject();
                o["message"] = e.SignalTime;

                string json = o.ToString();



                foreach (var s in wsServer.GetAllSessions())
                {
                    s.Send(json);
                }

            }
            catch
            {
                Console.WriteLine("erro");
            }


        }

        private static void WsServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            //throw new NotImplementedException();
            Console.WriteLine("SessionClosed");
        }

        private static void WsServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            //throw new NotImplementedException();
            Console.WriteLine("NewDataReceived");
        }

        private static void WsServer_NewMessageReceived(WebSocketSession session, string value)
        {
            //throw new NotImplementedException();
            Console.WriteLine("NewMessageReceived: " + value);


            JObject o = new JObject();
            o["Resevide"] = value;
            string json = o.ToString();

            session.Send(json);


            if (value == "Hello server")
            {
                session.Send("Hello client");
            }

            if (value == "dayvson como esta")
            {
                session.Send("Estou bem Obrigado");
            }


            string str = value;
            str = str.Replace("\"", "");
            if (str == "dayvson")
            {

                Console.WriteLine("NewMessageReceived: OKOKOKKK ");
                JObject o2 = new JObject();
                o2["ResevideAll"] = value;
                string json2 = o2.ToString();


                foreach (var s in wsServer.GetAllSessions())
                {
                    s.Send(json2);
                }
            }


        }

        private static void WsServer_NewSessionConnected(WebSocketSession session)
        {
            //throw new NotImplementedException();
            Console.WriteLine("NewSessionConnected");

        }






    }
}