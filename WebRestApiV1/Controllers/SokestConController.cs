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



        private async Task ControleMSG(AspNetWebSocketContext context)
        {
             var socket = context.WebSocket;
            

            //Gets the current WebSocket object.
            WebSocket webSocket = context.WebSocket;



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


               /* System.Diagnostics.Debug.WriteLine("vivo");

                byte[] payloadData2 = receivedDataBuffer.Array.Where(b => b != 0).ToArray();
                string receiveStringString =
                     System.Text.Encoding.UTF8.GetString(payloadData2, 0, payloadData2.Length);


                /*dynamic RecevideMSG = JsonConvert.DeserializeObject(receiveStringString);

                if (RecevideMSG["CREATE"] != null )
                {
                    System.Diagnostics.Debug.WriteLine("CHEGOUUUUUUUUUUUUUUUUUUUU   ");
                }
                */
               


               //SocketReceveMSG(RecevideMSG);



                //If input frame is cancelation frame, send close command.
                if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                      String.Empty, cancellationToken);
                }
                else
                {
                    byte[] payloadData = receivedDataBuffer.Array.Where(b => b != 0).ToArray();

                    byte[] bufferString = Encoding.UTF8.GetBytes(receivedDataBuffer.ToString());

                    string s = System.Text.Encoding.UTF8.GetString(bufferString, 0, bufferString.Length);


                    //Because we know that is a string, we convert it.
                    string receiveString =
                      System.Text.Encoding.UTF8.GetString(payloadData, 0, payloadData.Length);

                    Byte[] bytes;
                    bytes = System.Text.Encoding.UTF8.GetBytes("");

                    dynamic RecevideMSG = JsonConvert.DeserializeObject(receiveString);

                    //dynamic newStringMSG = @" { MSG  : '" + RecevideMSG.MSG.ToString() + "' , Time : '" + DateTime.Now.ToString() + "' , USER : '"+ RecevideMSG.Nome.ToString() + "' } ";

                    JObject o = new JObject();
                    o["MSG"] = RecevideMSG.MSG.ToString();
                    o["USER"] = RecevideMSG.Nome.ToString();
                    o["Time"] = DateTime.Now.ToString();

                    bytes = System.Text.Encoding.UTF8.GetBytes(o.ToString());

                    if (RecevideMSG.MSG == "CREATE")
                    {
                        System.Diagnostics.Debug.WriteLine("CHEGOUUUUUUUUUUUUUUUUUUUU   ");


                        //Converts string to byte array.
                        var newString = @" { MSG : 'Hello, " + RecevideMSG.Nome.ToString() + " !', Time :  '"+ DateTime.Now.ToString() + "', USER : 'MASTER'  } ";

                        JObject R = new JObject();
                        R["MSG"] = @"Hello, " + RecevideMSG.Nome.ToString();
                        R["USER"] = RecevideMSG.Nome.ToString();
                        R["Time"] = DateTime.Now.ToString();



                        bytes = System.Text.Encoding.UTF8.GetBytes(R.ToString());

                    }


                  


                    //Sends data back.
                    await webSocket.SendAsync(new ArraySegment<byte>(bytes),
                      WebSocketMessageType.Text, true, cancellationToken);

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