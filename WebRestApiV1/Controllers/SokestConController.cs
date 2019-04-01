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
 


namespace WebRestApiV1.Controllers
{
    public class SokestConController : ApiController
    {

        



        [HttpGet]
        public HttpResponseMessage Conect()
        {
            var httpContext = Request.Properties["MS_HttpContext"] as HttpContextBase;

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

                //get the socket
                var socket = Clients[socketId];

                //send the token over the socket
                await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(token)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch(Exception e )
            {
                System.Diagnostics.Debug.WriteLine(e);

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
 

            const int maxMessageSize = 1024;

            //Buffer for received bits.
            var receivedDataBuffer = new ArraySegment<Byte>(new Byte[maxMessageSize]);
            var receivedDataBufferNull = new ArraySegment<Byte>(new Byte[maxMessageSize]);

            var cancellationToken = new CancellationToken();

            //Checks WebSocket state.
            while (webSocket.State == WebSocketState.Open)
            {
                System.Diagnostics.Debug.WriteLine("vivo");

                //Reads data.
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
                    string receiveString =
                      System.Text.Encoding.UTF8.GetString(payloadData, 0, payloadData.Length);

                    Byte[] bytes;
                    bytes = System.Text.Encoding.UTF8.GetBytes("");

                    dynamic RecevideMSG = JsonConvert.DeserializeObject(receiveString);

                    //dynamic newStringMSG = @" { MSG  : '" + RecevideMSG.MSG.ToString() + "' , Time : '" + DateTime.Now.ToString() + "' , USER : '"+ RecevideMSG.Nome.ToString() + "' } ";

                    JObject o = new JObject();
                   

                    if (RecevideMSG.MSG == "CREATE")
                    {
                        System.Diagnostics.Debug.WriteLine("CHEGOUUUUUUUUUUUUUUUUUUUU   ");
                         
                        //Converts string to byte array.
                        //var newString = @" { MSG : 'Hello, " + RecevideMSG.Nome.ToString() + " !', Time :  '"+ DateTime.Now.ToString() + "', USER : 'MASTER'  } ";

                        //JObject R = new JObject();
                        o["MSG"] = @"Hello, " + RecevideMSG.Nome.ToString();
                        o["USER"] = "SERVER";
                        o["NOME"] = RecevideMSG.Nome.ToString();
                        o["IMG"] = ClientesDados.Count;
                        o["Time"] = DateTime.Now.ToString();


                        JObject R = new JObject();
                        R["USER"] = "SERVER";
                        R["CONFIG"] = @"CREATE";
                        R["NOME"] = RecevideMSG.Nome.ToString();
                        R["IMG"] = ClientesDados.Count;
                        R["Time"] = DateTime.Now.ToString();

                        ClientesDados.Add(R.ToString());

                        try {

                            foreach (var client in Clients)
                            {

                                foreach (var user in ClientesDados)
                                {
                                     /*o["MSG"] = @""  ;
                                    o["USER"] = "CONFIG";
                                    o["DADOS"] = user.ToString();
                                    o["Time"] = DateTime.Now.ToString();*/

                                    await SendTokenToClient(client.Key, user.ToString());
                                }

                            }

                        } catch (Exception e) {
                        }



                        bytes = System.Text.Encoding.UTF8.GetBytes(o.ToString());

                    }



                    if (RecevideMSG.MSG != "CREATE")
                    {
                        o["MSG"] = RecevideMSG.MSG.ToString();
                        o["USER"] = RecevideMSG.Nome.ToString();
                        o["IMG"] = RecevideMSG.IMG.ToString();
                        o["Time"] = DateTime.Now.ToString();

                        bytes = System.Text.Encoding.UTF8.GetBytes(o.ToString());

                        //Sends data back.(apena para o usuario)
                        /*await webSocket.SendAsync(new ArraySegment<byte>(bytes),
                          WebSocketMessageType.Text, true, cancellationToken);*/

                        foreach (var client in Clients)
                        {
                            await SendTokenToClient(client.Key, o.ToString());
                        }
                    }

                    receivedDataBuffer = receivedDataBufferNull;
                }
            }

             
        }



        public void SocketReceveMSG(  dynamic realbyt)
        {
           
            System.Diagnostics.Debug.WriteLine("CHEGOUUUUUUUUUUUUUUUUUUUU   ");
        }





    }
}