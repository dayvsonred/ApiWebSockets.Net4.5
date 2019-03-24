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



        private async Task ControleMSG(AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;

            //Gets the current WebSocket object.
            WebSocket webSocket = context.WebSocket;



            const int maxMessageSize = 1024;

            //Buffer for received bits.
            var receivedDataBuffer = new ArraySegment<Byte>(new Byte[maxMessageSize]);

            var cancellationToken = new CancellationToken();

            //Checks WebSocket state.
            while (webSocket.State == WebSocketState.Open)
            {
                System.Diagnostics.Debug.WriteLine("vivo");

                //Reads data.
                WebSocketReceiveResult webSocketReceiveResult =
                  await webSocket.ReceiveAsync(receivedDataBuffer, cancellationToken);


                System.Diagnostics.Debug.WriteLine("vivo");

                byte[] payloadData2 = receivedDataBuffer.Array.Where(b => b != 0).ToArray();
                string receiveStringString =
                     System.Text.Encoding.UTF8.GetString(payloadData2, 0, payloadData2.Length);

                SocketReceveMSG(webSocketReceiveResult.ToString(), receiveStringString);



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

                    //Converts string to byte array.
                    var newString = String.Format("Hello, " + receiveString + " ! Time {0}", DateTime.Now.ToString());
                    Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newString);

                    //Sends data back.
                    await webSocket.SendAsync(new ArraySegment<byte>(bytes),
                      WebSocketMessageType.Text, true, cancellationToken);
                }
            }










            /*while (true)
            {
                await Task.Delay(2000);
                
                if (socket.State == WebSocketState.Open)
                {
                    //var noticia = noticiais[random.Next(0, noticiais.Count - 1)];
                    JObject o = new JObject();
                    o["message"] = "aaaaaaaaaaaaa";
                    string json = o.ToString();
                    
                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));

                    System.Diagnostics.Debug.WriteLine("vivo");
                   
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }*/





        }


        public void SocketReceveMSG(string  n , string realbyt)
        {
            System.Diagnostics.Debug.WriteLine("CHEGOUUUUUUUUUUUUUUUUUUUU {0}  ", realbyt);
        }





    }
}