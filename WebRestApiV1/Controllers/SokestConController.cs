using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using System.Timers;


 


namespace WebRestApiV1.Controllers
{
    public class SokestConController : ApiController
    {

        private static System.Timers.Timer aTimer;
        JObject oJObject = new JObject();
        JObject PJObject = new JObject();
        JObject RJObject = new JObject();
        JObject MSGUserJObject = new JObject();


        [HttpGet]
        public HttpResponseMessage Conect()
        {
            var httpContext = Request.Properties["MS_HttpContext"] as HttpContextBase;

            
            try {

               if (aTimer.Enabled == false)
                {
                    // Create a timer and set a two second interval.
                    aTimer = new System.Timers.Timer();
                    aTimer.Interval = 9000;
                    // Hook up the Elapsed event for the timer. 
                    aTimer.Elapsed += OnTimedEvent;
                    // Have the timer fire repeated events (true is the default)
                    aTimer.AutoReset = true;
                    // Start the timer
                    aTimer.Enabled = true;
                    //Console.WriteLine("Press the Enter key to exit the program at any time... ");
                }

            } catch
            {
                
                aTimer = new System.Timers.Timer();
                aTimer.Interval = 5000;
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEvent;
                // Have the timer fire repeated events (true is the default)
                aTimer.AutoReset = true;
                // Start the timer
                aTimer.Enabled = true;
                //Console.WriteLine("Press the Enter key to exit the program at any time... ");
            }
           


            
            


            if (httpContext.IsWebSocketRequest)
                httpContext.AcceptWebSocketRequest(ControleMSG);

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private static readonly Dictionary<string, WebSocket> Clients = new Dictionary<string, WebSocket>();

        private static List<string> ClientesDados = new List<string>();
        int IMGUsers = 0;

        internal static async Task SendTokenToClient(string socketId, string token)
        {
            
            try
            {
                System.Diagnostics.Debug.WriteLine("enviar ------------------------------------------   ");
                //get the socket
               
                Byte[] bytes;
                bytes = System.Text.Encoding.UTF8.GetBytes("");

                try {
                    bytes = Encoding.UTF8.GetBytes(token);
                }
                catch (Exception eea)
                {
                    System.Diagnostics.Debug.WriteLine(eea);
                }

                //send the token over the socket
                await Clients[socketId].SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch(Exception eeeee )
            {
                System.Diagnostics.Debug.WriteLine(eeeee);

            }
        }




        private async Task ControleMSG(AspNetWebSocketContext context)
        {

            //generate a new ID for this socket connection
            var socketId = Guid.NewGuid().ToString();

            //var socket = context.WebSocket;


            //Gets the current WebSocket object.
            WebSocket webSocket = context.WebSocket;

             
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(socketId)), WebSocketMessageType.Text, true, CancellationToken.None);

            //add the socket to the dictionary
            Clients.Add(socketId, webSocket);
 

            const int maxMessageSize = 10240;

            //Buffer for received bits.
           

            var cancellationToken = new CancellationToken();

            //Checks WebSocket state.
            while (webSocket.State == WebSocketState.Open)
            {
                //System.Diagnostics.Debug.WriteLine("vivo");
                //Reads data.

                var receivedDataBuffer = new ArraySegment<Byte>(new Byte[maxMessageSize]);
                var receivedDataBufferNull = new ArraySegment<Byte>(new Byte[maxMessageSize]);


                WebSocketReceiveResult webSocketReceiveResult =
                  await webSocket.ReceiveAsync(receivedDataBuffer, cancellationToken);

  
                //If input frame is cancelation frame, send close command.
                if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                      String.Empty, cancellationToken);
                }
                else
                {
                    byte[] payloadData = receivedDataBuffer.Array.Where(b => b != 0).ToArray();


                    //Because we know that is a string, we convert it.
                    string receiveString = "";

                    receiveString = System.Text.Encoding.UTF8.GetString(payloadData, 0, payloadData.Length);

                    Byte[] bytes;
                    bytes = System.Text.Encoding.UTF8.GetBytes("");


                    bool MSGUser = true;

                    dynamic RecevideMSG; // = JsonConvert.DeserializeObject(receiveString);
                    //JObject RecevideMSG = new JObject();
                    //RecevideMSG["MSG"] = "";
                    try {
                        RecevideMSG = JsonConvert.DeserializeObject(receiveString);
                        //RecevideMSG = RecevideMSG2;

                   
                 


                   

                    //dynamic newStringMSG = @" { MSG  : '" + RecevideMSG.MSG.ToString() + "' , Time : '" + DateTime.Now.ToString() + "' , USER : '"+ RecevideMSG.Nome.ToString() + "' } ";




                    if (RecevideMSG.MSG == "CREATE" && MSGUser == true)
                    {

                        //Converts string to byte array.
                        //var newString = @" { MSG : 'Hello, " + RecevideMSG.Nome.ToString() + " !', Time :  '"+ DateTime.Now.ToString() + "', USER : 'MASTER'  } ";

                        //JObject R = new JObject();
                        oJObject["MSG"] = @"Hello, " + RecevideMSG.Nome.ToString();
                        oJObject["USER"] = "SERVER";
                        oJObject["NOME"] = RecevideMSG.Nome.ToString();
                        oJObject["IMG"] = ClientesDados.Count;
                        oJObject["Time"] = DateTime.Now.ToString("yyyy/mm/dd hh:mm:ss");



                        RJObject["USER"] = "SERVER";
                        RJObject["CONFIG"] = @"CREATE";
                        RJObject["NOME"] = RecevideMSG.Nome.ToString();
                        RJObject["IMG"] = ClientesDados.Count;
                        RJObject["Time"] = DateTime.Now.ToString("yyyy/mm/dd hh:mm:ss");

                        ClientesDados.Add(RJObject.ToString());

                        try {


                            System.Diagnostics.Debug.WriteLine("NOVO USER  ------------------------------------------   ");
                            foreach (var client in Clients)
                            {
                                foreach (var user in ClientesDados)
                                {
                                    await SendTokenToClient(client.Key, user.ToString());
                                }

                            }

                        } catch (Exception ee) {
                            System.Diagnostics.Debug.WriteLine(ee);
                        }



                        bytes = System.Text.Encoding.UTF8.GetBytes(oJObject.ToString());

                    }


                    
                    if (RecevideMSG.MSG != "CREATE" && MSGUser == true)
                    {
                        PJObject["MSG"] = RecevideMSG.MSG.ToString();
                        PJObject["USER"] = RecevideMSG.Nome.ToString();
                        PJObject["IMG"] = RecevideMSG.IMG.ToString();
                        PJObject["Time"] = DateTime.Now.ToString("yyyy/mm/dd hh:mm:ss");

                        //bytes = System.Text.Encoding.UTF8.GetBytes(oJObject.ToString());

                        //Sends data back.(apena para o usuario)
                        /*JObject oTime = new JObject();
                        oTime["message"] = @"ok";
                        Byte[]  bytes4 = System.Text.Encoding.UTF8.GetBytes(oTime.ToString());
                        await webSocket.SendAsync(new ArraySegment<byte>(bytes4),
                            WebSocketMessageType.Text, true, cancellationToken);*/

                        System.Diagnostics.Debug.WriteLine("MSG SIMPLES  ------------------------------------------   ");

                        foreach (var client in Clients)
                        {
                            await SendTokenToClient(client.Key, PJObject.ToString());
                        }
                    }


                    }
                    catch
                    {
                        MSGUser = false;
                    }





                    receivedDataBuffer = receivedDataBufferNull;
                }
            }

             
        }




        private  void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            //Console.WriteLine("data {0}", e.SignalTime);

            try
            {
                DateTime data = DateTime.Now;

                JObject o = new JObject();
                o["message"] = data.ToString("yyyy/mm/dd hh:mm:ss");
                System.Diagnostics.Debug.WriteLine(data.ToString("yyyy/mm/dd hh:mm:ss"));


                //tem pelomenos um cliente

                if (Clients.Count > 0)
                {
                    //string date = @" { "message' :  '" + e.SignalTime +"'} ";
                    //string jdate = JsonConvert.SerializeObject(date);

                    //e.SignalTime;

                    string json = o.ToString();
                    System.Diagnostics.Debug.WriteLine("CONTATO ------------------------------------------   ");
                    foreach (var client in Clients)
                    {
                        SendTokenToClient1(client.Key, o.ToString());
                    }
                }
                
            }
            catch (Exception eee)
            {
                Console.WriteLine("erro");
                System.Diagnostics.Debug.WriteLine(eee);
            }
            
        }


        internal static async Task SendTokenToClient1(string socketId, string token)
        {

            try
            {
                System.Diagnostics.Debug.WriteLine("enviar ------------------------------------------   ");
                //get the socket

                Byte[] bytes;
                bytes = System.Text.Encoding.UTF8.GetBytes("");

                try
                {
                    bytes = Encoding.UTF8.GetBytes(token);
                }
                catch (Exception eea)
                {
                    System.Diagnostics.Debug.WriteLine("awee");
                    System.Diagnostics.Debug.WriteLine(eea);
                }

                //send the token over the socket
                await Clients[socketId].SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception eeeee)
            {

                System.Diagnostics.Debug.WriteLine(eeeee);

            }
        }



        public void SocketReceveMSG(  dynamic realbyt)
        {
           
            System.Diagnostics.Debug.WriteLine("CHEGOUUUUUUUUUUUUUUUUUUUU   ");
        }





    }
}