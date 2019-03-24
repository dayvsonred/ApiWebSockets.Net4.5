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
    public class PublicadorController : ApiController
    {


        private static IList<string> noticiais;
        private static Random random;

        static PublicadorController()
        {
            noticiais = new List<string>()
            {
                "You can buy a fingerprint reader keyboard for your Surface Pro 3",
                "Microsoft’s new activity tracker is the $249 Microsoft Band",
                "Microsoft’s Lumia 950 is the new flagship Windows phone",
                "Windows 10 will start rolling out to phones in December",
                "Microsoft’s Panos Panay is pumped about everything(2012–present)"
            };

            random = new Random();
        }

        [HttpGet]
        public HttpResponseMessage Assinar()
        {
            var httpContext = Request.Properties["MS_HttpContext"] as HttpContextBase;

            if (httpContext.IsWebSocketRequest)
                httpContext.AcceptWebSocketRequest(EnviarNoticias);

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private async Task EnviarNoticias(AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;

            


            while (true)
            {
                await Task.Delay(2000);

                

                if (socket.State == WebSocketState.Open)
                {
                    var noticia = noticiais[random.Next(0, noticiais.Count - 1)];
                    JObject o = new JObject();
                    o["message"] = noticia;
                    string json = o.ToString();


                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
                     
                    System.Diagnostics.Debug.WriteLine("vivo");

                    

                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }
        }

        private Task<WebSocketReceiveResult> SocketReceveMSG(ArraySegment<byte> arg1, CancellationToken arg2)
        {
            throw new NotImplementedException();
        }
    }
}